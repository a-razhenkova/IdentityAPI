# :closed_lock_with_key: How to Authenticate

## :computer: Client Authentication
  
### Single-Factor Authentication
  
  An **access token** can be obtained using basic authentication with:\
  `POST /api/v1/token`
  
  > [!IMPORTANT]
  > External clients are required to activate a **subscription** via:\
  > `POST /api/v1/clients/{key}/subscriptions`
  
  > [!NOTE]
  > The **access token** can be validated via:\
  > `POST /api/v1/token/status`.

---

## :iphone: User Authentication
  
### Single-Factor Authentication
  
  **Access and refresh tokens** can be obtained using:\
  `POST /api/v2/token`
  
### Multi-Factor Authentication
  
  1. An **OTP** can be requested via:\
    `POST /api/v1/otp`
  
  2. The **OTP** can be exchanged for **access and refresh tokens** via:\
     `POST /api/v3/token`
  
  > [!IMPORTANT]
  > To authenticate with **OTP**, users are required to verify their email via:\
  > `POST /api/v1/email/verification/{token}`
  
  > [!NOTE]
  > The **access token** can be validated via:\
  > `POST /api/v1/token/status`.
  
  > [!TIP]
  > The **access token** can be refreshed via:\
  > `PUT /api/v2/token`
  
---

## :gear: Complete API Documentation

* [Postman collection](https://documenter.getpostman.com/view/34315168/2sBXqNmJmh)
* [Swagger](/assets/swagger.json)

---

# :triangular_ruler: Architecture & Design

![Design Diagram](/assets/design-diagram.png)

> SDK: .NET Core 10\
> Databases: `SQL Server 2022`\
> ORM: `Entity Framework Core`\
> Caching: `Redis`\
> Message Brokers: `RabbitMQ`\
> Additional Libraries: `Serilog`, `Polly`, `AutoMapper`, `GoogleAuthenticator`\
> Tests: `xUnit`, `FluentAssertions`, `Bogus`

> [!IMPORTANT]
> Database creation and schema management are handled through `Entity Framework Core` migrations at runtime.

> [!IMPORTANT]
> Initialization scripts are automatically executed using `DbUp` at runtime.

> [!CAUTION]
> The API requires running RabbitMQ broker.

> [!TIP]
> The application supports updating `appsettings.json` at runtime without requiring a restart.

---

## Architecture

![Architecture Diagram](/assets/architecture-diagram.png)

> [!NOTE]
> The application follows **Clean Architecture**.

> [!NOTE]
> The data access is managed with **Repository** and **Unit of Work** patterns.

### API
| Functionality | Description | Reference |
| --- | --- | --- |
| AES | Used to encrypt client subscription contracts when saving them and decrypt them when downloading. | [CreateAndAesEncryptAsync()](/Shared/Extensions/FileExtensions.cs), [ReadAndAesDecryptAllBytesAsync()](/Shared/Extensions/FileExtensions.cs) |
| Strategy Pattern | Used for handling tokens. | [SecurityTokenHandler.cs](/Application/Security/Tokens/SecurityTokenHandler.cs) |
| Data Mapping | Used `AutoMapper` for `request`/`response` mappings.<br/>Used manual mapping for `entity` mappings. | [Request/Response Mappings](/WebApi/Mappers)<br/>[Entity Mappings](/Application/Mappers) |
| RabbitMQ | Used for asynchronous communication with [`Notify API`](https://github.com/a-razhenkova/NotifyAPI) for sending notifications to users. Added resilience using `Polly`. | [RabbitMqService.cs](/Infrastructure/MessageBrokers/RabbitMqService.cs)<br/>[RabbitMqExtensions](/Infrastructure/MessageBrokers/RabbitMqExtensions.cs)<br/>[RabbitMqEventAttribute.cs](/Application/RabbitMQ/Attributes/RabbitMqEventAttribute.cs) |
| Rate Limiter | Used with fixed window counter, configurable by [`appsettings::Security::RateLimiter`](/WebApi/appsettings.json). | [AddRateLimiter()](/WebApi/Setup/WebAppBuilderExtensions.cs) |
| Logger | Global exception logging is handled [here](/WebApi/Middlewares/ExceptionHandlingMiddleware.cs).<br/>Global HTTP request and response logging is handled [here](WebApi/Middlewares/HttpMessageLoggingMiddleware.cs). | [LoggerSetup.cs](/WebApi/Setup/LoggerSetup.cs) |

### Caching
| Functionality | Description | Reference |
| --- | --- | --- |
| Redis | Distributed caching. | [RedisService.cs](/Infrastructure/Cache/RedisService.cs), [RedisKeyBuilder.cs](/Application/Redis/RedisKeyBuilder.cs) |

### Database
| Functionality | Description | Reference |
| --- | --- | --- |
| Repository Pattern | The full list of repositories can be viewed [here](/Infrastructure/Database/Repositories). | [Repository.cs](/Infrastructure/Database/Repository.cs) |
| Unit Of Work Pattern | Manages all access to the database. | [UnitOfWork.cs](/Infrastructure/Database/UnitOfWork.cs) |
| Migrations | Automatically executes pending [migrations](/Infrastructure/Database/Migrations) at startup. This functionality is configurable by [`appsettings::Database::IsDbMigrationAllowed`](/WebApi/appsettings.json). | [ApplyDbPendingMigrationsAsync()](/WebApi/Setup/ControllersSetup.cs) |
| DbUp | Automatically executes pending [scripts](/Infrastructure/Database/Scripts) at startup. This functionality is configurable by [`appsettings::Database::IsDbUpAllowed`](/WebApi/appsettings.json). | [ApplyDbPendingMigrationsAsync()](/WebApi/Setup/ControllersSetup.cs) |

---

## Client Single-Factor Authentication Steps
1. Client credentials are received in the `Authorization` header using the format:  
   `Basic <base64_encoded_key>:<base64_encoded_secret>`
2. The credentials from the header are decoded.
3. A database query is executed to fetch client data using the provided `key`.
4. If the `key` exists, the `client status` is validated.
5. If the `client status` is acceptable, the system checks for an active `subscription`.
6. If there is an active `subscription`, the stored `secret` is compared with the provided secret.
7. If the `secret` is valid, the `failed login attempt counter` is reset.
8. An `access token` (JWT) is generated, scoped to the `client ID` and the applications the client is allowed to access.

> [!WARNING]
> Invalid `key` or `secret` results in HTTP status code `401 Unauthorized`.

> [!WARNING]
> Invalid `client status` or missing active `subscription` results in HTTP status code `403 Forbidden`.

> [!CAUTION]
> If the `failed login attempt counter` exceeds the allowed limit, the `client status` is updated to `BLOCKED`.

---

## User Single-Factor Authentication Steps
1. User `username` and `password` are received in the request body.
2. A database query is executed to fetch user data using the provided `username`.
3. If the `username` exists, the `user status` is validated.
4. If the `user status` is acceptable, the provided `password` is hashed using the same method as the stored one.
5. The hashed `password` is compared with the stored password.
6. If the passwords match, the `failed login attempt counter` is reset.
7. An `access token` (JWT) is generated, scoped to the `user ID`, `username`, `user role` and `user status`.
8. A `refresh token` (JWT) is generated, scoped to the `user ID`.

> [!NOTE]
> An event is sent when a login attempt is made through a new IP address.

> [!WARNING]
> Invalid `username` or `password` results in HTTP status code `401 Unauthorized`.

> [!WARNING]
> Invalid `user status` results in HTTP status code `403 Forbidden`.

> [!CAUTION]
> If the `failed login attempt counter` exceeds the allowed limit, the `user status` is updated to `BLOCKED` and an alert for login attempt is registered.

---

## User Multi-Factor Authentication Steps
1. User `username` and `password` are received in the request body.
2. A database query is executed to fetch user data using the provided `username`.
3. If the `username` exists, the `user status` is validated.
4. If the `user status` is acceptable, the provided `password` is hashed using the same method as the stored one.
5. The hashed `password` is compared with the stored password.
6. If the passwords match, the `failed login attempt counter` is reset.
7. A `one-time password` is generated and saved to `Redis`.
8. A message is registered to send the `one-time password` to the user via the preferred notification provider.
9. The user receives the `one-time password` and enters it.
10. An attempt is made to fetch the `one-time password` from `Redis`.
11. If the `one-time password` is valid and not expired, it is deleted from `Redis`.
12. An `access token` (JWT) is generated, scoped to the `user ID`, `username`, `user role` and `user status`.
13. A `refresh token` (JWT) is generated, scoped to the `user ID`.

> [!NOTE]
> Each `one-time password` is stored in `Redis` with an absolute expiration.

> [!WARNING]
> Invalid `username` or `password` or `one-time password` results in HTTP status code `401 Unauthorized`.

> [!WARNING]
> Invalid `user status` results in HTTP status code `403 Forbidden`.

> [!CAUTION]
> A `failed login attempt counter` is maintained in `Redis` per `one-time password`. If the counter exceeds the allowed limit, the `one-time password` is deleted.

---

## Testing
* [Unit Tests](/UnitTests/Tests)
* [Integration Tests](/IntegrationTests/Tests)

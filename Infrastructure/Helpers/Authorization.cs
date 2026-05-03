namespace Infrastructure
{
    public class Authorization
    {
        public Authorization(string? authorization)
        {
            if (string.IsNullOrWhiteSpace(authorization))
                throw new UnauthorizedException();

            Schema = GetSchema(authorization);
            Value = authorization.TrimStart($"{Schema.ToString()} ");
        }

        public AuthorizationSchema Schema { get; init; }
        public string Value { get; init; }

        private AuthorizationSchema GetSchema(string authorization)
        {
            if (authorization.BeginsWith(AuthorizationSchema.Basic.ToString()))
            {
                return AuthorizationSchema.Basic;
            }
            else if (authorization.BeginsWith(AuthorizationSchema.Bearer.ToString()))
            {
                return AuthorizationSchema.Bearer;
            }
            else
            {
                throw new UnauthorizedException();
            }
        }
    }
}
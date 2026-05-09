using Microsoft.IdentityModel.Tokens;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Application
{
    public class SecurityToken
    {
        protected readonly SecuritySettings _settings;

        public SecurityToken(string token, SecuritySettings settings, string key)
            : this(settings, key)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException();

            Value = token;
        }

        protected SecurityToken(SecuritySettings settings, string key)
        {
            _settings = settings;

            ValidationParams = new()
            {
                RequireSignedTokens = true,
                SaveSigninToken = true,
                LogValidationExceptions = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidIssuer = settings.TokenIssuer,
                ValidateAudience = true,
                ValidAudience = settings.TokenAudience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        }

        protected string Value { get; set; } = string.Empty;
        public TokenValidationParameters ValidationParams { get; init; }

        public async Task<TokenValidationResult> ValidateAsync()
        {
            if (string.IsNullOrWhiteSpace(Value))
                throw new InvalidOperationException();

            return await new JwtSecurityTokenHandler().ValidateTokenAsync(Value, ValidationParams);
        }

        public string? GetClaim(Enum claim)
        {
            JwtSecurityToken? jwt = Decode();

            string? claimValue = null;

            if (jwt is not null)
                claimValue = GetClaim(jwt, claim);

            return claimValue;
        }

        public string? GetClaim(JwtSecurityToken token, Enum claim)
        {
            return token.Claims
                .Where(c => c.Type == claim.GetDescription())
                .Select(c => c.Value)
                .SingleOrDefault();
        }

        public JwtSecurityToken? Decode()
        {
            if (string.IsNullOrWhiteSpace(Value))
                throw new InvalidOperationException();

            JwtSecurityToken? jwt = null;

            try
            {
                new JwtSecurityTokenHandler().ValidateToken(Value, ValidationParams, out Microsoft.IdentityModel.Tokens.SecurityToken validatedToken);
                jwt = (JwtSecurityToken)validatedToken;
            }
            catch (Exception)
            {
                // continue
            }

            return jwt;
        }
    }
}
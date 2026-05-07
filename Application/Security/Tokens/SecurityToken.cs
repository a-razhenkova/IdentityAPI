using Microsoft.IdentityModel.Tokens;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Application
{
    public class SecurityToken
    {
        private string _token = string.Empty;
        protected readonly SecuritySettings _settings;

        public SecurityToken(string token, SecuritySettings settings, string key)
            : this(settings, key)
        {
            _token = token;
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

        public TokenValidationParameters ValidationParams { get; init; }

        public async Task<TokenValidationResult> ValidateAsync()
        {
            if (string.IsNullOrWhiteSpace(_token))
                throw new InvalidOperationException();

            return await new JwtSecurityTokenHandler().ValidateTokenAsync(_token, ValidationParams);
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
            if (string.IsNullOrWhiteSpace(_token))
                throw new InvalidOperationException();

            JwtSecurityToken? jwt = null;

            try
            {
                new JwtSecurityTokenHandler().ValidateToken(_token, ValidationParams, out Microsoft.IdentityModel.Tokens.SecurityToken validatedToken);
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
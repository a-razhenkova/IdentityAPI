using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Application
{
    public class SecurityTokenHandler
    {
        private readonly ISecurityToken _token;
        private readonly SecuritySettings _settings;

        public SecurityTokenHandler(ISecurityToken token, SecuritySettings settings)
        {
            _token = token;
            _settings = settings;
        }

        public string Create()
        {
            DateTime currentTimestamp = DateTime.UtcNow;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_token.TokenSettings.Key));
            var header = new JwtHeader(new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            var payload = new JwtPayload(issuer: _settings.TokenIssuer,
                audience: _settings.TokenAudience,
                claims: _token.CreateClaims(),
                notBefore: currentTimestamp,
                expires: currentTimestamp.AddSeconds(_token.TokenSettings.LifetimeInSeconds),
                issuedAt: currentTimestamp);

            var token = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
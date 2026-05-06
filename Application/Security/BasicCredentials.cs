using Shared;
using System.Text;

namespace Application
{
    public class BasicCredentials
    {
        public BasicCredentials(Authorization authorization)
        {
            if (authorization.Schema is not AuthorizationSchema.Basic)
                throw new BadRequestException("Invalid token format.");

            DecodeCredentials(authorization.Value);
        }

        public string Key { get; private set; } = string.Empty;
        public string Secret { get; private set; } = string.Empty;

        private void DecodeCredentials(string encodedCredentials)
        {
            string decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));

            string[] credentials = decodedCredentials.Split(":");

            Key = credentials[0];
            Secret = credentials[1];
        }
    }
}
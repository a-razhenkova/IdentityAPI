using System.Security.Cryptography;

namespace Application
{
    public class RandomKey
    {
        private byte[] _key;

        public RandomKey(int size)
        {
            if (size <= 0)
                throw new InvalidOperationException();

            _key = new byte[size];
        }

        public byte[] Create()
        {
            RandomNumberGenerator.Fill(_key);
            return _key;
        }

        public string CreateToBase64()
        {
            Create();
            return Convert.ToBase64String(_key);
        }
    }
}
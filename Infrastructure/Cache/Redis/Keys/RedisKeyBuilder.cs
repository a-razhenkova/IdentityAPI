using Infrastructure.Redis;

namespace Infrastructure
{
    public class RedisKeyBuilder
    {
        private const string KeyPrefix = "auth_api";

        private readonly RedisKey _key;
        private readonly object[]? _keyIds;

        public RedisKeyBuilder(RedisKey key, object[]? keyIds = null)
        {
            _key = key;
            _keyIds = keyIds;
        }

        public virtual string BuildKey()
        {
            string keySuffix = CreateKeySuffix(_key);
            string keyName = _key.GetDescription();
            return $"{KeyPrefix}:{keyName}:{keySuffix}";
        }

        private string CreateKeySuffix(RedisKey key)
        {
            switch (key)
            {
                case RedisKey.OneTimePassword:
                    {
                        return GetRequiredId<string>(TwoFactorAuthIds.ExternalUserId);
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        private TId GetRequiredId<TId>(Enum value)
            => GetId<TId>(value) ?? throw new InvalidOperationException();

        private TId? GetId<TId>(Enum value)
        {
            int index = Convert.ToInt32(value);
            return (TId)(_keyIds?[index]);
        }
    }
}
using Application;
using Bogus;
using Domain;

namespace UnitTests.Mocks
{
    public class UserFaker : Faker<User>
    {
        public UserFaker()
        {
            RuleFor(s => s.Id, f => f.Random.Long());
            RuleFor(s => s.PublicId, f => f.Random.Uuid().ToString());
            RuleFor(s => s.Username, f => f.Random.String2(UserConstants.UsernameMinLength, UserConstants.UsernameMaxLength));
            RuleFor(s => s.Role, f => f.PickRandom<UserRoles>());
            RuleFor(s => s.OtpKey, UserOtpKey.Create());
            RuleFor(s => s.Email, f => f.Internet.Email());
            RuleFor(s => s.IsVerified, f => true);
            RuleFor(s => s.RegistrationTimestamp, f => DateTime.Now);
            RuleFor(s => s.Status, (f, u) => new UserStatus()
            {
                Id = f.Random.Long(),
                UserId = u.Id,
                Value = UserStatuses.Active,
                Reason = UserStatusReasons.None,
                Note = f.Random.String(),
                User = u
            });
        }
    }
}
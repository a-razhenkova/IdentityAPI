using Domain;

namespace Application
{
    public static class UserMapper
    {
        public static UserDto MapToDto(this User user)
           => new UserDto()
           {
               Id = user.PublicId,
               Username = user.Username,
               Role = user.Role,
               Email = user.Email,
               RegistrationTimestamp = user.RegistrationTimestamp,
               Status = new UserStatusDto()
               {
                   Value = user.Status.Value,
                   Reason = user.Status.Reason
               }
           };

        public static IEnumerable<UserDto> MapToDto(this IEnumerable<User> users)
        {
            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var userDto = user.MapToDto();
                result.Add(userDto);
            }

            return result;
        }

        public static User Map(this User user, CreateUserCommand command)
        {
            user.Username = command.Username;
            user.Role = command.Role;
            user.OtpKey = UserOtpKey.Create();
            user.Email = command.Email;
            user.IsVerified = false;
            user.Status = new UserStatus()
            {
                Value = UserStatuses.Restricted,
                Reason = UserStatusReasons.NewUser
            };
            user.Password = UserPassword.Create(command.Password);
            user.Login = new Login();
            return user;
        }

        public static User Map(this User user, UpdateUserCommand command)
        {
            user.Username = command.Username;
            user.Role = command.Role;
            user.UpdateStatus(command.Status.Value, command.Status.Reason, command.Status.Note);
            return user;
        }
    }
}
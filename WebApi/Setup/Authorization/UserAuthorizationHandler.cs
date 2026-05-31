using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Shared;
using System.Security.Claims;

namespace WebApi
{
    public class UserAuthorizationHandler : IAuthorizationHandler
    {
        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach (var requirement in context.PendingRequirements)
            {
                if (requirement is RolesAuthorizationRequirement rolesAuthorizationRequirement)
                    ValidateUserRole(context, rolesAuthorizationRequirement);
            }
        }

        private static void ValidateUserRole(AuthorizationHandlerContext context, RolesAuthorizationRequirement requirement)
        {
            string? userRole = context.User.FindFirstValue(TokenClaim.UserRole.GetDescription());
            bool.TryParse(context.User.FindFirstValue(TokenClaim.IsInternalClient.GetDescription()), out bool isInternalClient);

            if (isInternalClient)
            {
                context.Succeed(requirement);
            }
            else if (!string.IsNullOrWhiteSpace(userRole))
            {
                foreach (var allowedRoles in requirement.AllowedRoles)
                {
                    string[] roles = allowedRoles.Split(",", StringSplitOptions.TrimEntries);

                    if (roles.Any(src => src.Equals(userRole)))
                        context.Succeed(requirement);
                }
            }
        }
    }
}
using Domain;
using Microsoft.AspNetCore.Authorization;
using Shared;

namespace WebApi
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        public AuthorizeUserAttribute(params UserRoles[] roles)
        {
            Roles = string.Join(",", roles.Select(src => src.GetDescription()));
        }
    }
}
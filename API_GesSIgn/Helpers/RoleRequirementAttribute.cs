using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

public class RoleRequirementAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _roleName;

    public RoleRequirementAttribute(string roleName)
    {
        _roleName = roleName;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var roleClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        if (roleClaim == null || roleClaim.Value != _roleName)
        {
            context.Result = new ForbidResult();
        }
    }
}

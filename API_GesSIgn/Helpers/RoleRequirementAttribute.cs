using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;

/// <summary>
/// attribut pour vérifier le rôle de l'utilisateur
/// </summary>
public class RoleRequirementAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string[] _roleNames;

    /// <summary>
    /// Juste un rôle est nécessaire
    /// J'ai laissé le string pour ne pas casser le code existant
    /// </summary>
    /// <param name="roleName"></param>
    public RoleRequirementAttribute(string roleName)
    {
        _roleNames = [roleName];
    }

    /// <summary>
    /// Plusieurs rôles sont nécessaires
    /// </summary>
    /// <param name="roleNames"></param>
    public RoleRequirementAttribute(string[] roleNames)
    {
        _roleNames = roleNames;
    }

    /// <summary>
    /// verification du role
    /// </summary>
    /// <param name="context"></param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var roleClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        if (roleClaim == null ||!_roleNames.Contains(roleClaim.Value))
        {
            context.Result = new ForbidResult();
        }
    }
}

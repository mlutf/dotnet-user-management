using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class PermissionAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _permission;

    public PermissionAuthorizeAttribute(string permission) => _permission = permission;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity!.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var permissions = user.Claims
            .Where(c => c.Type == "permissions")
            .Select(c => c.Value)
            .ToList();

        if (!permissions.Contains(_permission))
        {
            context.Result = new ForbidResult();
        }
    }
}

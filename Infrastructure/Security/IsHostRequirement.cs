using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Persistence;

namespace Infrastructure.Security
{
  public class IsHostRequirement : IAuthorizationRequirement { }

  public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
  {
    private readonly IHttpContextAccessor httpContext;
    private readonly DataContext dbContext;
    public IsHostRequirementHandler(IHttpContextAccessor HttpContext, DataContext Context) { this.dbContext = Context; this.httpContext = HttpContext; }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
    {
      if (context.Resource is AuthorizationFilterContext authContext)
      {
        var currentUserName = httpContext.HttpContext.User?.Claims?.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        var activityID = Guid.Parse(authContext.RouteData.Values["id"].ToString());
        var activity = dbContext.Activities.FindAsync(activityID).Result;
        var host = activity.UserActivities.FirstOrDefault(x => x.IsHost);

        if (host?.AppUser?.UserName == currentUserName) context.Succeed(requirement); else context.Fail();
      }

      return Task.CompletedTask;
    }
  }
}
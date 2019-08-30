using System.Linq;
using System.Security.Claims;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Security
{
  public class UserAccessor : IUserAccessor
  {
    private readonly IHttpContextAccessor httpContextAccessor;
    public UserAccessor(IHttpContextAccessor HttpContextAccessor) => this.httpContextAccessor = HttpContextAccessor;

    public string GetCurrentUsername()
    {
      var username = httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
      return username;
    }
  }
}
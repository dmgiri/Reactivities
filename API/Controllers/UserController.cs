using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.User;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
  public class UserController : BaseController
  {
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<User>> Login(Login.Query query) => await Mediator.Send(query);

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(Register.Command command) => await Mediator.Send(command);

    [HttpGet]
    public async Task<ActionResult<User>> CurrentUser() => await Mediator.Send(new CurrentUser.Query());
  }
}
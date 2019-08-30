using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.User;

namespace API.Controllers
{
  public class UserController : BaseController
  {
    [HttpPost("login")]
    public async Task<ActionResult<User>> Login(Login.Query query) => await Mediator.Send(query);
  }
}
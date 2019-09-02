using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Profiles;

namespace API.Controllers
{
  public class ProfilesController: BaseController
  {
    [HttpGet("{username}")]
    public async Task<ActionResult<Profile>> Get(string username) => await Mediator.Send(new Details.Query{Username = username});
  }
}

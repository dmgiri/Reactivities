using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Profiles;
using MediatR;

namespace API.Controllers
{
  public class ProfilesController: BaseController
  {
    [HttpGet("{username}")]
    public async Task<ActionResult<Profile>> Get(string username) => await Mediator.Send(new Details.Query{Username = username});

    [HttpPut]
    public async Task<ActionResult<Unit>> Edit(Edit.Command command) => await Mediator.Send(command);
  }
}

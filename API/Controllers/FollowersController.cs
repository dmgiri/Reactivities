using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Application.Profiles;
using Application.Followers;

namespace API.Controllers
{
  [Route("api/profiles")]
  public class FollowersController : BaseController
  {
    [HttpPost("{username}/follow")]
    public async Task<ActionResult<Unit>> Follow(string username) => await Mediator.Send(new Add.Command{Username = username});

    [HttpDelete("{username}/follow")]
    public async Task<ActionResult<Unit>> Unfollow(string username) => await Mediator.Send(new Delete.Command{Username = username});

    [HttpGet("{username}/follow")]
    public async Task<ActionResult<List<Profile>>> GetFollowings(string username, string predicate) => 
      await Mediator.Send(new List.Query{Username = username, Predicate = predicate});
  }
}
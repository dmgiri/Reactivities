using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Domain;
using Application.Photos;
using MediatR;

namespace API.Controllers
{
  public class PhotosController : BaseController
  {
    [HttpPost]
    public async Task<ActionResult<Photo>> Add([FromForm] Add.Command command) => await Mediator.Send(command);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Unit>> Delete(string id) => await Mediator.Send(new Delete.Command{ID = id});

    [HttpPost("{id}/setmain")]
    public async Task<ActionResult<Unit>> SetMain(string id) => await Mediator.Send(new SetMain.Command{ID = id});
  }
}
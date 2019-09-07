using System;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Activities;

namespace API.Controllers
{
  public class ActivitiesController : BaseController
  {
    [HttpGet]
    public async Task<ActionResult<List.ActivitiesEnvelope>> List(int? limit, int? offset, bool isGoing, bool isHost, DateTime? startDate) => 
      await Mediator.Send(new List.Query(limit, offset, isGoing, isHost, startDate));

    [HttpGet("{id}")]
    public async Task<ActionResult<ActivityDto>> Details(Guid id) => await Mediator.Send(new Details.Query{ID = id});

    [HttpPost]
    public async Task<ActionResult<Unit>> Create(Create.Command command) => await Mediator.Send(command);

    [HttpPut("{id}")]
    [Authorize(Policy = "IsActivityHost")]
    public async Task<ActionResult<Unit>> Edit(Guid id, Edit.Command command) { command.ID = id; return await Mediator.Send(command); } 

    [HttpDelete("{id}")]
    [Authorize(Policy = "IsActivityHost")]
    public async Task<ActionResult<Unit>> Delete(Guid id) => await Mediator.Send(new Delete.Command{ID = id});

    [HttpPost("{id}/attend")]
    public async Task<ActionResult<Unit>> Attend(Guid id) => await Mediator.Send(new Attend.Command{ID = id});

    [HttpDelete("{id}/attend")]
    public async Task<ActionResult<Unit>> Unattend(Guid id) => await Mediator.Send(new Unattend.Command{ID = id});
  }
}

// notes:
// [Authorize(Policy = "IsActivityHost")] means: this route (edit/delete) can only be accessed if and when the user is hosting this activity.
// see Infrastructure/Security/IsHostRequirement for the implementation and Startup for the Authorization policy configuration.
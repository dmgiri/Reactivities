using System;
using MediatR;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Domain;
using Application.Activities;


namespace API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ActivitiesController : ControllerBase
  {
    private readonly IMediator mediator;
    public ActivitiesController(IMediator Mediator) { this.mediator = Mediator; }

    [HttpGet]
    public async Task<ActionResult<List<Activity>>> List() => await mediator.Send(new List.Query());

    [HttpGet("{id}")]
    public async Task<ActionResult<Activity>> Details(Guid id) => await mediator.Send(new Details.Query{ID = id});

    [HttpPost]
    public async Task<ActionResult<Unit>> Create(Create.Command command) => await mediator.Send(command);

    [HttpPut("{id}")]
    public async Task<ActionResult<Unit>> Edit(Guid id, Edit.Command command) { command.ID = id; return await mediator.Send(command); } 

    [HttpDelete("{id}")]
    public async Task<ActionResult<Unit>> Delete(Guid id) => await mediator.Send(new Delete.Command{ID = id});
  }
}
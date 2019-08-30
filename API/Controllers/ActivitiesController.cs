using System;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Domain;
using Application.Activities;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
  public class ActivitiesController : BaseController
  {
    [HttpGet]
    public async Task<ActionResult<List<Activity>>> List() => await Mediator.Send(new List.Query());

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Activity>> Details(Guid id) => await Mediator.Send(new Details.Query{ID = id});

    [HttpPost]
    public async Task<ActionResult<Unit>> Create(Create.Command command) => await Mediator.Send(command);

    [HttpPut("{id}")]
    public async Task<ActionResult<Unit>> Edit(Guid id, Edit.Command command) { command.ID = id; return await Mediator.Send(command); } 

    [HttpDelete("{id}")]
    public async Task<ActionResult<Unit>> Delete(Guid id) => await Mediator.Send(new Delete.Command{ID = id});
  }
}
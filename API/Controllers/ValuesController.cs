using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ValuesController : ControllerBase
  {
    public DataContext context { get; }
    public ValuesController(DataContext Context) { this.context = Context; }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Value>>> Get() => await context.Values.ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Value>> Get(int id) => await context.Values.FindAsync(id);

    // POST api/values
    [HttpPost]
    public void Post([FromBody] string value) { }

    // PUT api/values/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value) { }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public void Delete(int id) { }
  }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Persistence;

namespace Application.Activities
{
  public class Delete
  {
    public class Command : IRequest { public Guid ID { get; set; } }
    
    public class Handler : IRequestHandler<Command>
    {
      private readonly DataContext context;
      public Handler(DataContext Context) { this.context = Context; }
    
      public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
      {
          var activity = await context.Activities.FindAsync(request.ID);
          if (activity == null) throw new Exception("Activity not found");

          context.Remove(activity);
          var success = await context.SaveChangesAsync() > 0;
          if (!success) throw new Exception("Problem saving changes"); else return Unit.Value;
       }
     }      
  }
}
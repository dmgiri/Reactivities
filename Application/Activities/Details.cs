using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
  public class Details
  {
    public class Query : IRequest<Activity> { public Guid ID { get; set; } }

    public class Handler : IRequestHandler<Query, Activity>
    {
      private readonly DataContext context;
      public Handler(DataContext Context) { this.context = Context; }

      public async Task<Activity> Handle(Query request, CancellationToken cancellationToken)  
      { 
        var activity = await context.Activities.FindAsync(request.ID);
        if (activity == null) throw new RestException(HttpStatusCode.NotFound, new {activity = "Not Found"});
        return activity;
      }
    }      
  }
}
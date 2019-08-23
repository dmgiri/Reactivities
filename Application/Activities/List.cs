using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Domain;
using Persistence;

namespace Application.Activities
{
  public class List
  {
    public class Query : IRequest<List<Activity>> { }

    public class Handler : IRequestHandler<Query, List<Activity>>
    {
      private readonly DataContext context;
      public Handler(DataContext Context) { this.context = Context; }

      public async Task<List<Activity>> Handle(Query request, CancellationToken cancellationToken) => await context.Activities.ToListAsync();
    }

  }
}
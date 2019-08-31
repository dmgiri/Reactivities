using System;
using MediatR;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Persistence;
using Application.Errors;
using AutoMapper;

namespace Application.Activities
{
  public class Details
  {
    public class Query : IRequest<ActivityDto> { public Guid ID { get; set; } }

    public class Handler : IRequestHandler<Query, ActivityDto>
    {
      private readonly DataContext context;
      private readonly IMapper mapper;
      public Handler(DataContext Context, IMapper Mapper) { this.context = Context; this.mapper = Mapper; }

      public async Task<ActivityDto> Handle(Query request, CancellationToken cancellationToken)
      {
        var activity = await context.Activities.FindAsync(request.ID);

        if (activity == null) throw new RestException(HttpStatusCode.NotFound, new { activity = "Not Found" });
        return mapper.Map<Activity, ActivityDto>(activity);
      }
    }
  }
}
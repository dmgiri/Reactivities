using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Domain;
using Persistence;
using AutoMapper;

namespace Application.Activities
{
  public class List
  {
    public class Query : IRequest<List<ActivityDto>> { }

    public class Handler : IRequestHandler<Query, List<ActivityDto>>
    {
      private readonly DataContext context;
      private readonly IMapper mapper;
      public Handler(DataContext Context, IMapper Mapper) { this.context = Context; this.mapper = Mapper; }

      public async Task<List<ActivityDto>> Handle(Query request, CancellationToken cancellationToken) {

        var activities =  await context.Activities.ToListAsync();
        return mapper.Map<List<Activity>, List<ActivityDto>>(activities);
      }
    }
  }
}
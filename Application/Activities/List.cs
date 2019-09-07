using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Domain;
using Persistence;
using System;
using Application.Interfaces;

namespace Application.Activities
{
  public class List
  {
    public class ActivitiesEnvelope
    {
      public List<ActivityDto> Activities { get; set; }
      public int ActivityCount { get; set; }
    }

    public class Query : IRequest<ActivitiesEnvelope>
    {
      public int? Limit { get; set; }
      public int? Offset { get; set; }
      public bool IsGoing { get; set; }
      public bool IsHost { get; set; }
      public DateTime? StartDate { get; set; }

      public Query(int? limit, int? offset, bool isGoing, bool isHost, DateTime? startDate)
      {
        this.Limit = limit; this.Offset = offset;
        this.IsHost = isHost; this.IsGoing = isGoing; this.StartDate = startDate ?? DateTime.Now;
      }
    }

    public class Handler : IRequestHandler<Query, ActivitiesEnvelope>
    {
      private readonly DataContext context; private readonly IMapper mapper; private readonly IUserAccessor userAccessor;
      public Handler(DataContext Context, IMapper Mapper, IUserAccessor UserAccessor)
      { this.userAccessor = UserAccessor; this.context = Context; this.mapper = Mapper; }

      public async Task<ActivitiesEnvelope> Handle(Query request, CancellationToken cancellationToken)
      {
        var username = userAccessor.GetCurrentUsername();

        var queryable = context.Activities
          .Where(x => x.Date >= request.StartDate)
          .OrderBy(x => x.Date)
          .AsQueryable();

        if (request.IsGoing && !request.IsHost) { queryable = queryable.Where(x => x.UserActivities.Any(a => a.AppUser.UserName == username)); }
        if (request.IsHost && !request.IsGoing) 
          { queryable = queryable.Where(x => x.UserActivities.Any(a => a.AppUser.UserName == username && a.IsHost)); }

        var activities = await queryable.Skip(request.Offset ?? 0).Take(request.Limit ?? 3).ToListAsync();

        return new ActivitiesEnvelope
        {
          Activities = mapper.Map<List<Activity>, List<ActivityDto>>(activities),
          ActivityCount = queryable.Count()
        };
      }
    }
  }
}
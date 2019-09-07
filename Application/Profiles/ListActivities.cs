using System;
using MediatR;
using System.Net;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Errors;

namespace Application.Profiles
{
  public class ListActivities
  {
    public class Query : IRequest<List<UserActivityDto>>
    {
      public string Username { get; set; }
      public string Predicate { get; set; }
    }

    public class Handler : IRequestHandler<Query, List<UserActivityDto>>
    {
      private readonly DataContext context;
      public Handler(DataContext Context) { context = Context; }

      public async Task<List<UserActivityDto>> Handle(Query request, CancellationToken cancellationToken)
      {
        var user = await context.Users.SingleOrDefaultAsync(x => x.UserName == request.Username);
        if (user == null) throw new RestException(HttpStatusCode.NotFound, new { User = "Not found" });

        var queryable = user.UserActivities.OrderBy(a => a.Activity.Date).AsQueryable();

        switch (request.Predicate)
        {
          case "past": queryable = queryable.Where(a => a.Activity.Date <= DateTime.Now); break;
          case "hosting": queryable = queryable.Where(a => a.IsHost); break;
          default: queryable = queryable.Where(a => a.Activity.Date >= DateTime.Now); break; // future
        }

        var activities = queryable.ToList();
        var activitiesToReturn = new List<UserActivityDto>();

        foreach (var activity in activities)
        {
          var userActivity = new UserActivityDto
          {
            ID = activity.Activity.ID,
            Title = activity.Activity.Title,
            Category = activity.Activity.Category,
            Date = activity.Activity.Date
          };

          activitiesToReturn.Add(userActivity);
        }

        return activitiesToReturn;
      }
    }
  }
}

// notes:
// IQueryables cannot be async-ed
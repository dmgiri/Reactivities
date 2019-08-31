using System;
using MediatR;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain;
using Persistence;
using Application.Errors;
using Application.Interfaces;

namespace Application.Activities
{
  public class Attend
  {
    public class Command : IRequest { public Guid ID { get; set; } }

    public class Handler : IRequestHandler<Command>
    {
      private readonly DataContext context;
      private readonly IUserAccessor userAccessor;
      public Handler(DataContext Context, IUserAccessor UserAccessor) { this.context = Context; this.userAccessor = UserAccessor; }
    
      public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
      {
        var activity = await context.Activities.FindAsync(request.ID);
        if (activity == null) throw new RestException(HttpStatusCode.NotFound, new {Activity = "Could not find activity"});

        var username = userAccessor.GetCurrentUsername();
        var user = await context.Users.SingleOrDefaultAsync(u => u.UserName == username);

        var attendance = await context.UserActivities.SingleOrDefaultAsync(x => x.ActivityID == activity.ID && x.AppUserID == user.Id);
        if (attendance != null) throw new RestException(HttpStatusCode.BadRequest, new {Attendance = "Already attending this activity"});

        attendance = new UserActivity { AppUser = user, Activity = activity, DateJoined = DateTime.Now, IsHost = false };
        context.UserActivities.Add(attendance);

        var success = await context.SaveChangesAsync() > 0;
        if (!success) throw new Exception("Problem saving changes"); else return Unit.Value;
      }
    }
  }
}

// notes:
// this class handles the event that a user wishes to attend an activity (of which he/she is not the host)
// input: activity.ID and (logged in) user
// output: a new entry in the UserActivities table
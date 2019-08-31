using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
  public class Unattend
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
        if (attendance == null) return Unit.Value;
        if (attendance.IsHost) throw new RestException(HttpStatusCode.BadRequest, new {Attendance = "You cannot remove yourself as host"});
        context.UserActivities.Remove(attendance);

        var success = await context.SaveChangesAsync() > 0;
        if (!success) throw new Exception("Problem saving changes"); else return Unit.Value;
      }
    }
  }
}
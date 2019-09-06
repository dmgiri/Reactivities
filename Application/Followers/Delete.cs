using System;
using MediatR;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Errors;
using Application.Interfaces;

namespace Application.Followers
{
  public class Delete
  {
    public class Command : IRequest { public string Username { get; set; } }

    public class Handler : IRequestHandler<Command>
    {
      private readonly DataContext context;
      private readonly IUserAccessor userAccessor;
      public Handler(DataContext Context, IUserAccessor UserAccessor) { this.userAccessor = UserAccessor; this.context = Context; }

      public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
      {
        var username = userAccessor.GetCurrentUsername();
        var observer = await context.Users.SingleOrDefaultAsync(x => x.UserName == username);

        var target = await context.Users.SingleOrDefaultAsync(x => x.UserName == request.Username);
        if (target == null) throw new RestException(HttpStatusCode.NotFound, new { User = "Not found" });
        
        var following = await context.Followings.SingleOrDefaultAsync(x => x.ObserverID == observer.Id && x.TargetID == target.Id);
        if (following == null) throw new RestException(HttpStatusCode.BadRequest, new { User = "You are not following this user"});

        context.Followings.Remove(following);

        var success = await context.SaveChangesAsync() > 0;
        if (!success) throw new Exception("Problem saving changes"); else return Unit.Value;
      }
    }      
  }
}
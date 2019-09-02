using System;
using MediatR;
using System.Net;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Errors;
using Application.Interfaces;

namespace Application.Photos
{
  public class SetMain
  {
    public class Command : IRequest { public string ID { get; set; } }

    public class Handler : IRequestHandler<Command>
    {
      private readonly DataContext context;
      private readonly IUserAccessor userAccessor;
      public Handler(DataContext Context, IUserAccessor UserAccessor) { this.userAccessor = UserAccessor; this.context = Context; }


      public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
      {
        var username = userAccessor.GetCurrentUsername();
        var user = await context.Users.SingleOrDefaultAsync(x => x.UserName == username);

        var photo = user.Photos.FirstOrDefault(x => x.ID == request.ID);
        if (photo == null) throw new RestException(HttpStatusCode.NotFound, new {Photo = "Photo not found"});

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain == true);
        currentMain.IsMain = false; photo.IsMain = true;

        var success = await context.SaveChangesAsync() > 0;
        if (!success) throw new Exception("Problem saving changes"); else return Unit.Value;
      }
    }
  }
}
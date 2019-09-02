using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Persistence;
using Application.Errors;
using System.Net;

namespace Application.Photos
{
  public class Delete
  {
    public class Command : IRequest { public string ID { get; set; } }

    public class Handler : IRequestHandler<Command>
    {
      private readonly DataContext context;
      private readonly IUserAccessor userAccessor;
      private readonly IPhotoAccessor photoAccessor;
      public Handler(DataContext Context, IUserAccessor UserAccessor, IPhotoAccessor PhotoAccessor)
      { this.photoAccessor = PhotoAccessor; this.userAccessor = UserAccessor; this.context = Context; }

      public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
      {
        var username = userAccessor.GetCurrentUsername();
        var user = await context.Users.SingleOrDefaultAsync(x => x.UserName == username);

        var photo = user.Photos.FirstOrDefault(x => x.ID == request.ID);
        if (photo == null) throw new RestException(HttpStatusCode.NotFound, new {Photo = "Photo not found"});
        if (photo.IsMain) throw new RestException(HttpStatusCode.BadRequest, new {Photo = "You cannot delete your main photo"});

        var result = photoAccessor.DeletePhoto(photo.ID);       // delete photo from Cloudinary
        if (result == null) throw new Exception("Problem deleting photo");
        user.Photos.Remove(photo);                              // delete photo data from db

        var success = await context.SaveChangesAsync() > 0;
        if (!success) throw new Exception("Problem saving changes"); else return Unit.Value;
      }
    }
  }
}
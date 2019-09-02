using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Domain;
using Persistence;
using Application.Interfaces;

namespace Application.Photos
{
  public class Add
  {
    public class Command : IRequest<Photo> { public IFormFile File { get; set; } }

    public class Handler : IRequestHandler<Command, Photo>
    {
      private readonly DataContext context;
      private readonly IUserAccessor userAccessor;
      private readonly IPhotoAccessor photoAccessor;
      public Handler(DataContext Context, IUserAccessor UserAccessor, IPhotoAccessor PhotoAccessor)
      { this.photoAccessor = PhotoAccessor; this.userAccessor = UserAccessor; this.context = Context; }

      public async Task<Photo> Handle(Command request, CancellationToken cancellationToken)
      {
        var photoUploadResult = photoAccessor.AddPhoto(request.File);
        var username = userAccessor.GetCurrentUsername();
        var user = context.Users.SingleOrDefault(x => x.UserName == username);

        var photo = new Photo { ID = photoUploadResult.PublicID, Url = photoUploadResult.Url };
        if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;
        user.Photos.Add(photo);
        
        var success = await context.SaveChangesAsync() > 0;
        if (!success) throw new Exception("Problem saving photo"); else return photo;
      }
    }
  }
}
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Domain;


namespace Application.User
{
  public class CurrentUser
  {
    public class Query : IRequest<User> { }

    public class Handler : IRequestHandler<Query, User>
    {
      private readonly UserManager<AppUser> userManager;
      private readonly IJwtGenerator jwtGenerator;
      private readonly IUserAccessor userAccessor;
      public Handler(UserManager<AppUser> UserManager, IJwtGenerator JwtGenerator, IUserAccessor UserAccessor)
      { this.userAccessor = UserAccessor; this.jwtGenerator = JwtGenerator; this.userManager = UserManager;  }

      public async Task<User> Handle(Query request, CancellationToken cancellationToken)
      {
        var username = userAccessor.GetCurrentUsername();
        var user = await userManager.FindByNameAsync(username);

        return new User 
        { 
          DisplayName = user.DisplayName, 
          Username = user.UserName, 
          Token = jwtGenerator.CreateToken(user), 
          Image = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        };
      }
    }
  }
}
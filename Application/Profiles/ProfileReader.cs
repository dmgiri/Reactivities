using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Errors;
using Application.Interfaces;

namespace Application.Profiles
{
  public class ProfileReader : IProfileReader
  {
    private readonly DataContext context;
    private readonly IUserAccessor userAccessor;
    public ProfileReader(DataContext Context, IUserAccessor UserAccessor) { this.userAccessor = UserAccessor; this.context = Context; }

    public async Task<Profile> ReadProfile(string username)
    {
      var user = await context.Users.SingleOrDefaultAsync(x => x.UserName == username);
      if (user == null) throw new RestException(HttpStatusCode.NotFound, new { User = "Not found"});

      var currentUsername = userAccessor.GetCurrentUsername();
      var currentUser = await context.Users.SingleOrDefaultAsync(x => x.UserName == currentUsername);

      var profile = new Profile
      {
        DisplayName = user.DisplayName,
        Username = user.UserName,
        Image = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
        Photos = user.Photos,
        Bio = user.Bio,
        FollowersCount = user.Followers.Count(),
        FollowingCount = user.Followings.Count()
      };

      if (currentUser.Followings.Any(x => x.TargetID == user.Id)) profile.IsFollowed = true;

      return profile;
    }
  }
}
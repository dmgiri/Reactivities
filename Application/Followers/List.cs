using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Domain;
using Persistence;
using Application.Profiles;

namespace Application.Followers
{
  public class List
  {
    public class Query : IRequest<List<Profile>> 
    { 
      public string Username { get; set; }
      public string Predicate { get; set; }
    }

    public class Handler : IRequestHandler<Query, List<Profile>>
    {
      private readonly DataContext context;
      private readonly IProfileReader profileReader;
      public Handler(DataContext Context, IProfileReader ProfileReader) { this.profileReader = ProfileReader; this.context = Context;
      }

      public async Task<List<Profile>> Handle(Query request, CancellationToken cancellationToken)
      {
        var queryable = context.Followings.AsQueryable();
        var userFollowings = new List<UserFollowing>();
        var profiles = new List<Profile>();

        switch(request.Predicate)
        {
          case "followers":
          {
            userFollowings = await queryable.Where(x => x.Target.UserName == request.Username).ToListAsync();
            foreach(var follower in userFollowings) { profiles.Add(await profileReader.ReadProfile(follower.Observer.UserName)); }
            break;
          }

          case "followings":
          {
            userFollowings = await queryable.Where(x => x.Observer.UserName == request.Username).ToListAsync();
            foreach(var follower in userFollowings) { profiles.Add(await profileReader.ReadProfile(follower.Target.UserName)); }
            break;
          }
        }

        return profiles;
      }
    }
  }
}
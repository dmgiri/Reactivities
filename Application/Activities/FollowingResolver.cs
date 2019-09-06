using AutoMapper;
using System.Linq;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Domain;
using Persistence;

namespace Application.Activities
{
  public class FollowingResolver : IValueResolver<UserActivity, AttendeeDto, bool>
  {
    private readonly DataContext Context;
    private readonly IUserAccessor userAccessor;
    public FollowingResolver(DataContext Context, IUserAccessor UserAccessor) { this.userAccessor = UserAccessor; this.Context = Context; }


    public bool Resolve(UserActivity source, AttendeeDto destination, bool destMember, ResolutionContext context)
    {
      var currentUsername = userAccessor.GetCurrentUsername();
      var currentUser = Context.Users.SingleOrDefaultAsync(x => x.UserName == currentUsername).Result;

      return (currentUser.Followings.Any(x => x.TargetID == source.AppUserID));
    }
  }
}
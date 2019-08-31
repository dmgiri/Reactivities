using System;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Activities
{
  public class Create
  {
    public class Command : IRequest
    {
      public Guid ID { get; set; }
      public string Title { get; set; }
      public string Description { get; set; }
      public string Category { get; set; }
      public DateTime Date { get; set; }
      public string City { get; set; }
      public string Venue { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
      public CommandValidator()
      {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Category).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.Venue).NotEmpty();
      }
    }

    public class Handler : IRequestHandler<Command>
    {
      private readonly DataContext context;
      private readonly IUserAccessor userAccessor;
      public Handler(DataContext Context, IUserAccessor UserAccessor) { this.context = Context; this.userAccessor = UserAccessor; }

      public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
      {
        var activity = new Activity 
        {
          ID = request.ID, Title = request.Title, Description = request.Description, Category = request.Category, Date = request.Date,
          City = request.City, Venue = request.Venue
        };
        context.Activities.Add(activity);

        var username = userAccessor.GetCurrentUsername();
        var user = await context.Users.SingleOrDefaultAsync(u => u.UserName == username);
        var attendee = new UserActivity { AppUser = user, Activity = activity, DateJoined = DateTime.Now, IsHost = true };
        context.UserActivities.Add(attendee);

        var success = await context.SaveChangesAsync() > 0;
        if (!success) throw new Exception("Problem saving changes"); else return Unit.Value;
      }
    }
  }
}
using System;
using MediatR;
using System.Net;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Errors;
using Application.Interfaces;

namespace Application.Profiles
{
  public class Edit
  {
    public class Command : IRequest
    {
      public string DisplayName { get; set; }
      public string Bio { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
      public CommandValidator()
      {
        RuleFor(x => x.DisplayName).NotEmpty();
      }
    }

    public class Handler : IRequestHandler<Command>
    {
      private readonly IUserAccessor userAccessor;
      private readonly DataContext context;
      public Handler(DataContext Context, IUserAccessor UserAccessor) { this.context = Context; this.userAccessor = UserAccessor; }

      public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
      {
        var userName = userAccessor.GetCurrentUsername();
        var user = await context.Users.SingleOrDefaultAsync(u => u.UserName == userName);
        if (user == null) throw new RestException(HttpStatusCode.NotFound);

        user.DisplayName = request.DisplayName ?? user.DisplayName;
        user.Bio = request.Bio ?? user.Bio;

        var success = await context.SaveChangesAsync() > 0;
        if (!success) throw new Exception("Problem saving changes"); else return Unit.Value;
      }
    }
  }
}


// notes:
// logged-in user can only edit his own profile!
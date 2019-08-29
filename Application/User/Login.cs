using MediatR;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Domain;
using Application.Errors;
using System.Net;

namespace Application.User
{
  public class Login
  {
    public class Query : IRequest<User>
    {
      public string Email { get; set; }
      public string Password { get; set; }
    }

    public class QueryValidator : AbstractValidator<Query>
    {
      public QueryValidator()
      {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
      }
    }

    public class Handler : IRequestHandler<Query, User>
    {
      private readonly UserManager<AppUser> userManager;
      private readonly SignInManager<AppUser> signInManager;
      public Handler(UserManager<AppUser> UserManager, SignInManager<AppUser> SignInManager)
      {
        this.signInManager = SignInManager; this.userManager = UserManager;
      }

      public async Task<User> Handle(Query request, CancellationToken cancellationToken)
      {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null) throw new RestException(HttpStatusCode.Unauthorized);
        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded) throw new RestException(HttpStatusCode.Unauthorized);
        else return new User { DisplayName = user.DisplayName, Token = "this will be a token", Username = user.UserName, Image = null };
      }
    }
  }
}
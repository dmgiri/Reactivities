using MediatR;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Domain;
using Application.Errors;
using System.Net;
using Application.Interfaces;
using System.Linq;

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
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
      }
    }

    public class Handler : IRequestHandler<Query, User>
    {
      private readonly UserManager<AppUser> userManager;
      private readonly SignInManager<AppUser> signInManager;
      private readonly IJwtGenerator jwtGenerator;
      public Handler(UserManager<AppUser> UserManager, SignInManager<AppUser> SignInManager, IJwtGenerator JwtGenerator)
      { this.signInManager = SignInManager; this.jwtGenerator = JwtGenerator; this.userManager = UserManager; }


      public async Task<User> Handle(Query request, CancellationToken cancellationToken)
      {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null) throw new RestException(HttpStatusCode.Unauthorized);

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded) throw new RestException(HttpStatusCode.Unauthorized);
        
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
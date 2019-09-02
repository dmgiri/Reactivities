using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Application.Validators;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.User
{
  public class Register
  {
    public class Command : IRequest<User>
    {
      public string DisplayName { get; set; }
      public string Username { get; set; }
      public string Email { get; set; }
      public string Password { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
      public CommandValidator()
      {
        RuleFor(x => x.DisplayName).NotEmpty();
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).Password();
      }
    }

    public class Handler : IRequestHandler<Command, User>
    {
      private readonly UserManager<AppUser> userManager;
      private readonly IJwtGenerator jwtGenerator;
      private readonly DataContext context;
      public Handler(DataContext Context, UserManager<AppUser> UserManager, IJwtGenerator JwtGenerator)
      { this.jwtGenerator = JwtGenerator; this.userManager = UserManager; this.context = Context; }
    

      public async Task<User> Handle(Command request, CancellationToken cancellationToken)
      {
        if (await context.Users.Where(u => u.Email == request.Email).AnyAsync())
          throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already in use" });

        if (await context.Users.Where(u => u.UserName == request.Username).AnyAsync())
          throw new RestException(HttpStatusCode.BadRequest, new { Username = "Username already in use" });

        var user = new AppUser { DisplayName = request.DisplayName, Email = request.Email, UserName = request.Username };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) throw new Exception("Problem creating user");

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
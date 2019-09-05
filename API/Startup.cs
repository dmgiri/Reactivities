using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Persistence;
using MediatR;
using Application.Activities;
using FluentValidation.AspNetCore;
using API.Middleware;
using Domain;
using Microsoft.AspNetCore.Identity;
using Application.Interfaces;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using AutoMapper;
using Infrastructure.Photos;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Threading.Tasks;
using API.SignalR;

namespace API
{
  public class Startup
  {
    public Startup(IConfiguration configuration) => Configuration = configuration;
    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<DataContext>(opt => { opt.UseLazyLoadingProxies(); opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection")); });
      services.AddCors(opt => opt.AddPolicy("CorsPolicy", policy => { 
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000").AllowCredentials(); }));
      services.AddMediatR(typeof(List.Handler).Assembly);
      services.AddAutoMapper(typeof(List.Handler));
      services.AddSignalR();
      services.AddMvc(opt =>
      {
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build(); opt.Filters.Add(new AuthorizeFilter(policy));
      })
        .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Create>())
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

      var builder = services.AddIdentityCore<AppUser>();
      var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
      identityBuilder.AddEntityFrameworkStores<DataContext>();
      identityBuilder.AddSignInManager<SignInManager<AppUser>>();

      services.AddAuthorization(opt => { opt.AddPolicy("IsActivityHost", policy => { policy.Requirements.Add(new IsHostRequirement()); }); });
      services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"]));
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
      {
        opt.TokenValidationParameters = new TokenValidationParameters
        { ValidateIssuerSigningKey = true, IssuerSigningKey = key, ValidateAudience = false, ValidateIssuer = false };
        
        opt.Events = new JwtBearerEvents { OnMessageReceived = context => { 
          var accessToken = context.Request.Query["access_token"]; var path = context.HttpContext.Request.Path;
          if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat")) { context.Token = accessToken; }
          return Task.CompletedTask;
          }};
      });
      services.AddScoped<IJwtGenerator, JwtGenerator>();
      services.AddScoped<IUserAccessor, UserAccessor>();
      services.AddScoped<IPhotoAccessor, PhotoAccessor>();
      services.Configure<CloudinarySettings>(Configuration.GetSection("Cloudinary"));
      services.Configure<FormOptions>(options => { options.BufferBody = true; options.ValueLengthLimit = Int32.MaxValue; 
        options.MultipartBodyLengthLimit = Int32.MaxValue; options.MemoryBufferThreshold = Int32.MaxValue; });
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseMiddleware<ErrorHandlingMiddleware>();
      app.UseAuthentication();
      app.UseCors("CorsPolicy");
      app.UseSignalR(routes => { routes.MapHub<ChatHub>("/chat"); });
      app.UseMvc();
    }
  }
}

// notes:
// if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); } else {}  // app.UseHsts(); 
// app.UseHttpsRedirection();

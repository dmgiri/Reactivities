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

namespace API
{
  public class Startup
  {
    public Startup(IConfiguration configuration) => Configuration = configuration;
    public IConfiguration Configuration { get; }
        
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<DataContext>(opt => { opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection")); });
      services.AddCors(opt => opt.AddPolicy("CorsPolicy", policy => { policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000"); }));
      services.AddMediatR(typeof(List.Handler).Assembly);
      services.AddMvc(opt => { 
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build(); opt.Filters.Add(new AuthorizeFilter(policy)); })
        .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Create>())
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
      
      var builder = services.AddIdentityCore<AppUser>();
      var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
      identityBuilder.AddEntityFrameworkStores<DataContext>();
      identityBuilder.AddSignInManager<SignInManager<AppUser>>();

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"]));
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt => {
        opt.TokenValidationParameters = new TokenValidationParameters
        { ValidateIssuerSigningKey = true, IssuerSigningKey = key, ValidateAudience = false, ValidateIssuer = false }; });
      services.AddScoped<IJwtGenerator, JwtGenerator>();
      services.AddScoped<IUserAccessor, UserAccessor>();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseMiddleware<ErrorHandlingMiddleware>();
      app.UseAuthentication();
      app.UseCors("CorsPolicy");
      app.UseMvc();
    }
  }
}

// notes:
// if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); } else {}  // app.UseHsts(); 
// app.UseHttpsRedirection();

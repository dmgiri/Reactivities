using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Domain;
using Persistence;

namespace API
{
  public class Program
  {
    public static void Main(string[] args) { 

      var host = CreateWebHostBuilder(args).Build();
      using(var scope = host.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        try {
          var context = services.GetRequiredService<DataContext>();
          var userManager = services.GetRequiredService<UserManager<AppUser>>();
          context.Database.Migrate();
          Seed.SeedData(context, userManager).Wait();
        }
        catch (Exception ex) {
          var logger = services.GetRequiredService<Logger<Program>>();
          logger.LogError(ex, "An error occurred during migration.");
        }
      }
      host.Run();
    }
    
    public static IWebHostBuilder CreateWebHostBuilder(string[] args) 
    { 
      return WebHost.CreateDefaultBuilder(args)
        .ConfigureLogging((HostingContext, options) => options.AddConfiguration(HostingContext.Configuration.GetSection("Logging")).AddConsole())
        .UseKestrel(x => x.AddServerHeader = false)
        .UseStartup<Startup>();
    }
  }
}

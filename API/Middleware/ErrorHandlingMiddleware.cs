using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Application.Errors;
using System.Net;

namespace API.Middleware
{
  public class ErrorHandlingMiddleware
  {
    private readonly RequestDelegate next;
    private readonly ILogger<ErrorHandlingMiddleware> logger;
    public ErrorHandlingMiddleware(RequestDelegate Next, ILogger<ErrorHandlingMiddleware> Logger) { this.next = Next; this.logger = Logger; }

    public async Task Invoke(HttpContext context) 
    {
      try { await next(context); } 
      catch (Exception ex) { await HandleExceptionAsync(context, ex, logger); }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger<ErrorHandlingMiddleware> logger)
    {
      object errors = null;

      switch (ex) 
      {
        case RestException re:
          logger.LogError(ex, "REST ERROR");
          errors = re.Errors;
          context.Response.StatusCode = (int)re.Code;
          break;
        case Exception e:
          logger.LogError(ex, "SERVER ERROR");
          errors = string.IsNullOrWhiteSpace(e.Message) ? "Error" : e.Message;
          context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
          break;
        default: break;
      }

      context.Request.ContentType = "application/json";
      if (errors != null) {
        var result = JsonConvert.SerializeObject(new { errors });
        await context.Response.WriteAsync(result);
      }
    }
  }
}
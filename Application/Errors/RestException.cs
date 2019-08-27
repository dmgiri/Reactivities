using System;
using System.Net;

namespace Application.Errors
{
  public class RestException : Exception
  {
    public HttpStatusCode Code;
    public object Errors;
    
    public RestException(HttpStatusCode code, Object errors = null) 
    {  
      this.Code = code;  this.Errors = errors; 
    }
  }
}
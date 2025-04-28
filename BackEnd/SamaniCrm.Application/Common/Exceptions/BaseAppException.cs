using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Exceptions;
public class BaseAppException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public BaseAppException(HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
      : base()
    {
        StatusCode = statusCode;
    }

    public BaseAppException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message)
    {
        StatusCode = statusCode;
    }

  
    public BaseAppException(string message, Exception exp, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message, exp)
    {
        StatusCode = statusCode;
    }
}

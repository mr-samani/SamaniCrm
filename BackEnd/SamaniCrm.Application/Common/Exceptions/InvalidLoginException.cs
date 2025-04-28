using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Exceptions
{
    public class InvalidLoginException : BaseAppException
    {
        public InvalidLoginException() : base("Invalid UserName or Password!") { }
        
        public HttpStatusCode StatusCode => HttpStatusCode.Unauthorized; // 401
    }
}

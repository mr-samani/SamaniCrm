using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Exceptions
{
    public class UnAuthenticateException : BaseAppException
    {
        public UnAuthenticateException() : base("User is not authenticated!", System.Net.HttpStatusCode.Unauthorized) { }
    }
}

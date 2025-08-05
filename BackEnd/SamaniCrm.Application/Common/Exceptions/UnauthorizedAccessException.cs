using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Exceptions
{
    public class UnauthorizedAccessException: BaseAppException
    {
        public UnauthorizedAccessException() : base("Invalid Token!", System.Net.HttpStatusCode.Unauthorized) { }

    }
}

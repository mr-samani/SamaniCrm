using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Exceptions
{
    public class AccessDeniedException : BaseAppException
    {
        public AccessDeniedException() : base("Access denied!", System.Net.HttpStatusCode.Forbidden) { }
        public AccessDeniedException(string param) : base(param, System.Net.HttpStatusCode.Forbidden) { }

    }
}

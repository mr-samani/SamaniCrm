using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Exceptions
{
    public class ForbiddenAccessException:Exception
    {
        public ForbiddenAccessException() : base("Invalid Token!") { }

    }
}

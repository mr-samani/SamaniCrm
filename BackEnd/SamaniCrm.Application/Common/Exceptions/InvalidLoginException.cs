using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Exceptions
{
    public class InvalidLoginException : Exception
    {
        public InvalidLoginException() : base("Invalid Username or Password!") { }
    }
}

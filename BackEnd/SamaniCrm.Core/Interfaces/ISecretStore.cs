using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Core.Shared.Interfaces
{
    public interface ISecretStore
    {
        string GetSecret(string key);
    }

}

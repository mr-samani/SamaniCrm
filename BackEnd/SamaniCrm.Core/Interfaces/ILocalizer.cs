using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Core.Shared.Interfaces;
public interface ILocalizer
{
    string CurrentLanguage { get; }
    string this[string key] { get; }
    string this[string key, params object[] args] { get; }
}


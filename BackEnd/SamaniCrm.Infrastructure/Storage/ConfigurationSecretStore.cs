using Microsoft.Extensions.Configuration;
using SamaniCrm.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Storage
{

    public class ConfigurationSecretStore : ISecretStore
    {
        private readonly IConfiguration _configuration;

        public ConfigurationSecretStore(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // dotnet user-secrets init
        // dotnet user-secrets set "Microsoft:ClientId" "d7e410d4-02d7-4c64-a0f2-4200a0323e96"
        //  Visual Studio, right-click the project in Solution Explorer, and select Manage User Secrets from the context
        public string GetSecret(string key)
        {
            return _configuration[$"{key}"] ?? "";
        }
    }


}

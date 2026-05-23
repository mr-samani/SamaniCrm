using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.FileManager
{
    public class FileManagerSettings
    {
        public string Provider { get; set; } = "locale";

        /// <summary>
        /// Base Path = ./Files
        /// </summary>
        public string PublicFolderPath { get; set; } = "./Files";
        public string? tusFilesPath { get; set; } = "./tusfiles";
    }
}

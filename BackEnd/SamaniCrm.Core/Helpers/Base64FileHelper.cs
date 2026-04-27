using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamaniCrm.Core.Shared.Helpers;

public static class Base64FileHelper
{
    public static Stream ConvertToStream(string base64, out string contentType)
    {
        contentType = "application/octet-stream";

        // حذف data:image/png;base64,
        var match = Regex.Match(base64, @"^data:(.+);base64,(.*)$");
        if (match.Success)
        {
            contentType = match.Groups[1].Value;
            base64 = match.Groups[2].Value;
        }

        var bytes = Convert.FromBase64String(base64);
        return new MemoryStream(bytes);
    }
}


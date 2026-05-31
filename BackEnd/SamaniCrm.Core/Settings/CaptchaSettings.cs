using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Core.Shared.Settings;

public class CaptchaSettings
{
    public bool Enabled { get; set; } = true;
}



public class OIDCSettings
{
    public OIDCCookie Cookie { get; set; } = new();


}

public class OIDCCookie
{
    public string Name { get; set; } = "__SCM.Auth";
    public bool HttpOnly { get; set; } = true;
    public CookieSecurePolicy SecurePolicy { get; set; } = CookieSecurePolicy.Always;
    public SameSiteMode SameSite { get; set; } = SameSiteMode.None;
    public string Domain { get; set; } = "";
    public bool SlidingExpiration { get; set; } = true;


}



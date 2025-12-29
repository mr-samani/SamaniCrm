using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Exceptions;

public class InvalidLoginException : BaseAppException
{
    public InvalidLoginException() : base("Invalid UserName or Password!", HttpStatusCode.Unauthorized) { }

}


public class LoginAttempCountException : BaseAppException
{
    public LoginAttempCountException() : base("Your account is locked! try after 5 minutes", HttpStatusCode.Unauthorized) { }

}

public class InvalidCaptchaException : BaseAppException
{
    public InvalidCaptchaException() : base("Invalid Captcha!") { }

}

public class InvalidTwoFactorCodeException : BaseAppException
{
    public InvalidTwoFactorCodeException() : base("Invalid two factor code!") { }

}
public class ExternalLoginException : BaseAppException
{
    public ExternalLoginException(string v) : base("Login in external providers is faild! \n" + v,HttpStatusCode.Unauthorized) { }

}
namespace SamaniCrm.Infrastructure.ExternalLogin;

public class ExternalLoginResult
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? GivenName { get; set; }

    public string? Image { get; set; }
}
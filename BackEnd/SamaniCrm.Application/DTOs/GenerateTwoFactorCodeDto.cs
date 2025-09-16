
namespace SamaniCrm.Application.DTOs;

public class GenerateTwoFactorCodeDto
{
    public required string Secret { get; set; }
    public required string QrCode { get; set; }
 }
namespace SamaniCrm.Application.Common.DTOs
{
    public class UserDto()
    {
        public required string Id { get; set; }
        [Sortable]
        public string? UserName { get; set; }
        [Sortable]
        public string? FirstName { get; set; }
        [Sortable]
        public string? LastName { get; set; } = string.Empty;
        [Sortable]
        public string? Email { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; } = string.Empty;

    }


}

namespace SamaniCrm.Application.DTOs
{
    public class AutoCompleteDto<T>
    {
        public required T Id { get; set; }
        public required string Title { get; set; }
    }
}
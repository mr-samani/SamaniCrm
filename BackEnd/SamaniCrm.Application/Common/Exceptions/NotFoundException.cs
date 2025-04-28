namespace SamaniCrm.Application.Common.Exceptions
{
    public class NotFoundException : BaseAppException
    {
        public NotFoundException() : base()
        {

        }

        public NotFoundException(string message) : base(message)
        {

        }

        public NotFoundException(string message, Exception exp) : base(message, exp)
        {

        }

        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {

        }
    }
}

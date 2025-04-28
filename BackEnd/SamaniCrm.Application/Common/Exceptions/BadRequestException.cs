namespace SamaniCrm.Application.Common.Exceptions
{
    public class BadRequestException : BaseAppException
    {
        public BadRequestException() : base()
        {

        }

        public BadRequestException(string message) : base(message)
        {

        }

        public BadRequestException(string message, Exception exp) : base(message, exp)
        {

        }
    }
}

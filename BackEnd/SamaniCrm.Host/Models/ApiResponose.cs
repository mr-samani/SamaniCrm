namespace SamaniCrm.Host.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public int StatusCode { get; set; }

        public ApiResponse(bool success, string message,int statusCode, T? data )
        {
            Success = success;
            Message = message;
            Data = data;
            StatusCode = statusCode;
        }
        public ApiResponse(bool success, string message, int statusCode)
        {
            Success = success;
            Message = message;
            StatusCode = statusCode;

            Data = default(T);
        }

    }

}

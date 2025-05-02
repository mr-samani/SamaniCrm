namespace SamaniCrm.Host.Models
{
    /// <summary>
    /// Standardized API response wrapper with support for data, errors, metadata, pagination, and HATEOAS links.
    /// </summary>
    public class ApiResponse<T>
    {
        /// <summary>Indicates if the request was successful.</summary>
        public bool Success { get; set; }

        /// <summary>Holds the response data when Success == true.</summary>
        public T? Data { get; set; }

        /// <summary>List of error details when Success == false.</summary>
        public List<ApiError>? Errors { get; set; }

        /// <summary>Optional metadata (e.g., pagination details).</summary>
        public Meta? Meta { get; set; }

        /// <summary>Optional HATEOAS links.</summary>

        public ApiResponse() { }

        /// <summary>Creates a successful response.</summary>
        public static ApiResponse<T> Ok(T data,
                                       Meta? meta = null)
            => new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Meta = meta
            };

        /// <summary>Creates a failure response with errors.</summary>
        public static ApiResponse<T> Fail(List<ApiError> errors,
                                         Meta? meta = null
                                         )
            => new ApiResponse<T>
            {
                Success = false,
                Errors = errors,
                Meta = meta
            };

        internal static object? Fail(string message)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>Encapsulates error details.</summary>
    public class ApiError
    {
        /// <summary>Field name associated with the error (optional).</summary>
        public string? Field { get; set; }

        /// <summary>Error message.</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>Error code (optional).</summary>
        public string? Code { get; set; }

        /// <summary>Developer-focused exception details (set only in Development).</summary>
        public string? Detail { get; set; }
    }

    /// <summary>General metadata container.</summary>
    public class Meta
    {
        /// <summary>Total number of items (for pagination).</summary>
        public int? TotalCount { get; set; }

        /// <summary>Current page number.</summary>
        public int? PageNumber { get; set; }

        /// <summary>Number of items per page.</summary>
        public int? PageSize { get; set; }

        /// <summary>Total number of pages (computed).</summary>
        public int? TotalPages => (PageSize.HasValue && PageSize > 0 && TotalCount.HasValue)
            ? (int)Math.Ceiling(TotalCount.Value / (double)PageSize.Value)
            : null;
    }






}

namespace StudyHubAPI.Utils
{
    public class ServiceResult
    {
        public bool IsSuccess { get; }
        public int StatusCode { get; }
        public string? ErrorMessage { get; }

        protected ServiceResult(bool isSuccess, int statusCode, string? errorMessage)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }

        public static ServiceResult Success(int statusCode = 200)
            => new(true, statusCode, null);

        public static ServiceResult Failure(int statusCode, string errorMessage)
            => new(false, statusCode, errorMessage);
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; }

        private ServiceResult(bool isSuccess, int statusCode, T? data, string? errorMessage)
            : base(isSuccess, statusCode, errorMessage)
        {
            Data = data;
        }

        public static ServiceResult<T> Success(T data, int statusCode = 200)
            => new(true, statusCode, data, null);

        public static new ServiceResult<T> Failure(int statusCode, string errorMessage)
            => new(false, statusCode, default, errorMessage);
    }
}

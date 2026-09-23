namespace StudyHubAPI.Utils
{
    public enum ResultType
    {
        Ok,        
        Created,   
        NoContent,
        BadRequest, 
        NotFound,   
        Conflict,   
        Failure 
    }

    public class ServiceResult
    {
        public bool IsSuccess => Type is ResultType.Ok or ResultType.Created or ResultType.NoContent;
        public ResultType Type { get; }
        public string? ErrorMessage { get; }

        protected ServiceResult(ResultType type, string? errorMessage)
        {
            Type = type;
            ErrorMessage = errorMessage;
        }

        public static ServiceResult Success(ResultType type = ResultType.Ok)
            => new(type, null);

        public static ServiceResult Failure(ResultType type, string errorMessage)
            => new(type, errorMessage);
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; }

        private ServiceResult(ResultType type, T? data, string? errorMessage)
            : base(type, errorMessage)
        {
            Data = data;
        }

        public static ServiceResult<T> Success(T data, ResultType type = ResultType.Ok)
            => new(type, data, null);

        public static new ServiceResult<T> Failure(ResultType type, string errorMessage)
            => new(type, default, errorMessage);
    }
}
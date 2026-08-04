namespace Core.Common
{
    public class ServiceResult
    {
        public bool Success { get; protected set; } = true;
        public string Message { get; protected set; }
        public Dictionary<string, string> Errors { get; protected set; } = new();

        public static ServiceResult Ok(string message = "Operación exitosa") =>
            new() { Success = true, Message = message };

        public static ServiceResult Fail(string message) =>
            new() { Success = false, Message = message };

        public ServiceResult AddError(string propertyName, string errorMessage)
        {
            Success = false;
            Errors[propertyName] = errorMessage;
            return this;
        }
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T Data { get; private set; }

        public static ServiceResult<T> Ok(T data, string message = "Operación exitosa") =>
            new()
            {
                Success = true,
                Data = data,
                Message = message,
            };

        public static new ServiceResult<T> Fail(string message) =>
            new() { Success = false, Message = message };
    }
}

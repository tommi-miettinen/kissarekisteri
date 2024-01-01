using System.Collections.Generic;

namespace Kissarekisteri.ErrorHandling
{
    public class Error(string message, string code)
    {
        public string Message { get; set; } = message;
        public string Code { get; set; } = code;
    }

    public class Result<T>
    {
        public T Data { get; private set; }
        public bool IsSuccess { get; private set; }
        public List<Error> Errors { get; private set; } = [];

        public Result<T> Success(T data)
        {
            Data = data;
            IsSuccess = true;
            return this;
        }

        public Result<T> AddError(Error error)
        {
            Errors.Add(error);
            return this;
        }
    }
}
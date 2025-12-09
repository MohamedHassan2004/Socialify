using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Domain.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        public List<string> Errors { get; private set; } = new();

        private Result(bool isSuccess, T? data, string errorMessage, List<string> errors)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
            Errors = errors;
        }

        public static Result<T> Success(T data) => new(true, data, string.Empty, new List<string>());
        public static Result<T> Failure(string error) => new(false, default, error, new List<string>());
        public static Result<T> Failure(List<string> errors) => new(false, default, string.Empty, errors);
    }

    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        public List<string> Errors { get; private set; } = new();

        private Result(bool isSuccess, string errorMessage, List<string> errors)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Errors = errors;
        }

        public static Result Success() => new(true, string.Empty, new List<string>());
        public static Result Failure(string error) => new(false, error, new List<string>());
        public static Result Failure(List<string> errors) => new(false, string.Empty, errors);
    }
}
using System;
using TypeGen.Core.TypeAnnotations;

namespace Wishmaster.Mvc
{
    [ExportTsInterface]
    public class ApiResponse<T>
    {
        public T? Response { get; set; }
        public ResultCode Code { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsSuccess => Code == ResultCode.Success;

        public static ApiResponse<T> Success(T data)
        {
            return new ApiResponse<T>
            {
                Response = data,
                Code = ResultCode.Success,
                ErrorMessage = null,
            };
        }

        public static ApiResponse<T> Failure(ResultCode code, string message)
        {
            if (code == ResultCode.Success)
            {
                throw new Exception($"Unable to create '{ResultCode.Success}' result with '{nameof(Failure)}' method");
            }

            return new ApiResponse<T>
            {
                Response = default,
                Code = code,
                ErrorMessage = message,
            };
        }
    }

    [ExportTsEnum]
    public enum ResultCode : short
    {
        Success = 0,
        BadRequest = 1,
        NetworkError = 2,
        ClientError = 3,
    }
}

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Wishmaster.Backend.Mvc
{
    public abstract class BaseController : Controller
    {
        [NonAction]
        protected ActionResult<ApiResponse<T>> Success<T>(T model)
            => ApiResponse<T>.Success(model);

        [NonAction]
        protected ActionResult<ApiResponse<object>> Success(object? model = null)
            => Success<object>(model!);

        [NonAction]
        protected ActionResult<ApiResponse<object>> Failure(ResultCode errorCode, string message)
            => ApiResponse<object>.Failure(errorCode, message);

        [NonAction]
        protected ActionResult<ApiResponse<object>> Failure(ModelStateDictionary modelState)
        {
            var messages = modelState.Values.SelectMany(x => x.Errors, (x, y) => y.ErrorMessage);
            return ApiResponse<object>.Failure(ResultCode.BadRequest, JsonConvert.SerializeObject(messages));
        }
    }
}

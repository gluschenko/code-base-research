using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Wishmaster.Mvc
{
    public abstract class BaseController : Controller
    {
        [NonAction]
        protected IActionResult Success<T>(T model)
            => Json(ApiResponse<T>.Success(model));

        [NonAction]
        protected IActionResult Success(object? model = null)
            => Success<object>(model!);

        [NonAction]
        protected IActionResult Failure(ResultCode errorCode, string message)
            => Json(ApiResponse<object>.Failure(errorCode, message));

        [NonAction]
        protected IActionResult Failure(ModelStateDictionary modelState)
        {
            var messages = modelState.Values.SelectMany(x => x.Errors, (x, y) => y.ErrorMessage);
            return Json(ApiResponse<object>.Failure(ResultCode.BadRequest, JsonConvert.SerializeObject(messages)));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Core.Common;

namespace src.Presentation.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Transfiere los errores capturados en el Service al ModelState de la Vista.
        /// </summary>
        protected void MapServiceErrors(ServiceResult result)
        {
            if (result.Success) return;

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            if (!string.IsNullOrEmpty(result.Message) && result.Errors.Count == 0)
            {
                ModelState.AddModelError(string.Empty, result.Message);
            }

            TempData["ServiceErrorMessage"] = result.Message;
        }

        protected void SetSuccessMessage(string message)
        {
            TempData["ServiceSuccessMessage"] = message;
        }
    }
}
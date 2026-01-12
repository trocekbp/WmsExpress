using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace WmsCore.Controllers
{
    public class BaseController: Controller
    {
        protected void NotifySuccess(string message)
        {
            TempData["UserMessage"] = message;
            TempData["MessageType"] = "success";
        }

        protected void NotifyError(string message)
        {
            TempData["UserMessage"] = message;
            TempData["MessageType"] = "error"; // dla SweetAlert2
        }
    }
}

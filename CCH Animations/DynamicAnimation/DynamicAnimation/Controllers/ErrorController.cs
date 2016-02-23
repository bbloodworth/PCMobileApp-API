using System;
using System.Web.Mvc;

namespace DynamicAnimation.Controllers
{
    public class ErrorController : Controller
    {
        private static readonly string RAISE_ERROR_TOKEN = @"cb95006f-2dc9-4baf-aa42-a737e5351184";

        public ActionResult Index()
        {
            return View("~/Views/Card/Index.cshtml");
        }

        public ActionResult Raise(string id) {
            if (string.IsNullOrEmpty(id) || !id.ToLower().Equals(RAISE_ERROR_TOKEN.ToLower()))
                return View("~/Views/Card/Index.cshtml");

            throw new InvalidOperationException("Error by default");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TotalFireSafety.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        [HandleError]
        public ActionResult NotFound()
        {
            return View();
        }
        [HandleError]
        public ActionResult Forbidden()
        {
            return View();
        }
        [HandleError]
        public ActionResult InternalServerError()
        {
            return View();
        }
        [HandleError]
        public ActionResult MethodNotAllowed()
        {
            return View();
        }
    }
}
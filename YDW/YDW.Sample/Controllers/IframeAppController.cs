using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YDW.Sample.Common;

namespace YDW.Sample.Controllers
{
    public class IframeAppController : Controller
    {
        //
        // GET: /IframeApp/
        [Permission]
        public ActionResult Index()
        {
            var a = Request.Url;
            ViewBag.message = DateTime.Now;
            Url.AddRootUrl("");
            return View();
        }

    }
}

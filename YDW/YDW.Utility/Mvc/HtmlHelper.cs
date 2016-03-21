using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace System.Web.Mvc
{
    public static class HtmlHelper
    {
        public static string AddRootUrl(this UrlHelper urlHelper, string url)
        {
            return urlHelper.Content("~") + url;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YDW.Sample.Common
{
    public class PermissionAttribute : ActionFilterAttribute
    {
        private string _action;
        private string _controller;
        private string _requestType;
        private HttpSessionStateBase _session;

        /// <summary>
        /// 重写OnActionExecuting，判断是用户是否登录超时或拥有该模块权限。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            UrlHelper url = new UrlHelper(filterContext.RequestContext);
            string helpurl = url.Action("sorry", "home", new { area = "" },
                                      url.RequestContext.HttpContext.Request.Url.Scheme);

            _session = filterContext.HttpContext.Session;
            _requestType = filterContext.HttpContext.Request.RequestType;
            _controller = filterContext.RouteData.Values["controller"].ToString().ToLower();
            _action = filterContext.RouteData.Values["action"].ToString().ToLower();

            

        }


    }
}
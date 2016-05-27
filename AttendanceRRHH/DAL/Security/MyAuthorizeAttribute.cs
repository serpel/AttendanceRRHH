﻿using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Configuration;
using System.Web.Routing;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using AttendanceRRHH.Models;

namespace AttendanceRRHH.DAL.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AccessAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            bool isAuthenticated = filterContext.HttpContext.Request.IsAuthenticated;

            if (isAuthenticated) {

                var user = filterContext.HttpContext.User;
                string [] roles = Roles.Split(',');

                bool isInRole = false;
                foreach(var item in roles)
                {
                    if (user.IsInRole(item.Trim()))
                    {
                        isInRole = true;
                        break;
                    }
                }

                if (!isInRole)
                {
                    filterContext.Result = new RedirectToRouteResult(new
                        RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
                }
            }
        }
    }
}
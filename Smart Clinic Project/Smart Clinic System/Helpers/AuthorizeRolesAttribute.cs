using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SmartClinic.Data;
using SmartClinic.Models.Enums;

namespace SmartClinic.Helpers
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        private readonly UserRole[] _roles;

        public AuthorizeRolesAttribute(params UserRole[] roles)
        {
            _roles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }

            var userId = httpContext.User.Identity.GetUserId();
            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return false;
                }

                return _roles.Contains(user.Role);
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // User is authenticated but not authorized
                filterContext.Result = new RedirectResult("~/Account/AccessDenied");
            }
            else
            {
                // User is not authenticated
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}
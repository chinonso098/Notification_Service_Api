using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Security.Claims;

namespace SQNotificationService.Security
{
    /// <summary>
    /// Retruns an HTTP Unauthorized response if the current ClaimsPrincipal does not have a scope claim or the scope is not "user_impersonation."
    /// </summary>
    public class ImpersonationScopeRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var scopeClaim = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/scope");
            if (null == scopeClaim || scopeClaim.Value != "user_impersonation")
            {
                HttpResponseMessage message = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "The Scope claim does not contain 'user_impersonation' or scope claim not found");
            }
        }
    }
}
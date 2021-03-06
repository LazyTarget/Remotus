﻿using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Lux.Extensions;

namespace Remotus.API
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ControllerCategoryActionFilterAttribute : ActionFilterAttribute
    {
        public ControllerCategoryActionFilterAttribute()
        {
        }

        public ControllerCategoryActionFilterAttribute(string categoryName)
            : this()
        {
            CategoryName = categoryName;
        }

        public string CategoryName { get; set; }


        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!string.IsNullOrEmpty(CategoryName))
            {
                var descriptor = actionContext?.ControllerContext?.ControllerDescriptor;
                if (descriptor != null && descriptor.ControllerType != null)
                {
                    var attributes = descriptor.ControllerType.GetCustomAttributes<ControllerCategoryAttribute>();
                    var match = attributes.Any(attr =>
                    {
                        var m = false;
                        if (!string.IsNullOrEmpty(attr.CategoryName))
                            m = string.Equals(CategoryName, attr.CategoryName, StringComparison.OrdinalIgnoreCase);
                        return m;
                    });

                    if (!match)
                    {
                        var error = new HttpError($"Controller is missing category '{CategoryName}'");
                        var actionResult = actionContext.Request.CreateFormattedContentResult(error, HttpStatusCode.NotFound);
                        actionContext.Response = actionResult.ExecuteAsync(CancellationToken.None).WaitForResult();
                        return;
                    }
                }
            }

            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}

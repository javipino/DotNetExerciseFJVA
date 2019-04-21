using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Api.Shared.Controllers
{
    public class BaseController: Controller
    {
        #region Properties
        
        public ILogger Logger { get; set; }

        #endregion

        #region Constructors

        public BaseController()
        {
        }

        #endregion
        #region Event handlers

        [NonAction]
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            SetContext(context.HttpContext);
            Logger.LogInformation(string.Format("[{0}] INIT: {1}", DateTime.Now.ToString(), context.ActionDescriptor.DisplayName));
        }

        [NonAction]
        public override void OnActionExecuted(Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext context)
        {
            SetContext(context.HttpContext);
            Logger.LogInformation(string.Format("[{0} ] END: {1} --> {2}", DateTime.Now.ToString(), context.ActionDescriptor.DisplayName, context.Result == null ? string.Empty : context.Result.ToString()));
            base.OnActionExecuted(context);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Sets the properties value from the context
        /// </summary>
        /// <param name="context"></param>
        private void SetContext(HttpContext context)
        {
            if (Logger == null)
            {
                var factory = context.RequestServices.GetService<ILoggerFactory>();
                Logger = factory.CreateLogger(GetType());
            }
        }

        #endregion
    }
}

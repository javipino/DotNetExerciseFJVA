using Api.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Internal;
using System;

namespace Api.Shared
{
    public class ControllerActivator : DefaultControllerActivator
    {
        private readonly ITypeActivatorCache _typeActivatorCache;

        public ControllerActivator(ITypeActivatorCache typeActivatorCache) : base(typeActivatorCache)
        {
            _typeActivatorCache = typeActivatorCache ?? throw new ArgumentNullException(nameof(typeActivatorCache));
        }

        /// <summary>
        /// Validation to ensure that all controllers inherit from BaseController
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override object Create(ControllerContext context)
        {
            var controllerType = context.ActionDescriptor.ControllerTypeInfo.AsType();

            if (!controllerType.IsSubclassOf(typeof(BaseController)))
            {
                throw new Exception(string.Format("The Controller {0} not inherit from BaseController", controllerType.Name));
            }

            return base.Create(context);
        }
    }
}

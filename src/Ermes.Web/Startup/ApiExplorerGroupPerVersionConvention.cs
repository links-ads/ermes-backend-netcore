using Ermes.Interfaces;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Logging;
using Namotion.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Web.Startup
{
    public class ApiExplorerGroupPerVersionConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            // API partitioning 
            if (controller.Attributes != null && controller.Attributes.Count > 0)
            {
                foreach (var attr in controller.Attributes)
                {
                    if (attr.HasProperty(ErmesConsts.IgnoreApi))
                        return;
                }
            }

            if (typeof(IBackofficeApi).IsAssignableFrom(controller.ControllerType))
                controller.ApiExplorer.GroupName = ErmesConsts.SwaggerBackofficeDocumentName;
            else
                controller.ApiExplorer.GroupName = ErmesConsts.SwaggerAppDocumentName;


        }
    }
}

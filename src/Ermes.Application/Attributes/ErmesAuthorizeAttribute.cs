using Abp.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ErmesAuthorizeAttribute : Attribute, IErmesAuthorizeAttribute
    {
        public ErmesAuthorizeAttribute(params string[] permissions)
        {
            Permissions = permissions;
        }
        public string[] Permissions { get; }
        public bool RequireAllPermissions { get; set; }
    }
}

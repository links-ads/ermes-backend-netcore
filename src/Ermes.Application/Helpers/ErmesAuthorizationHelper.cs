using Abp;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Ermes.Attributes;
using Ermes.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Helpers
{
    public class ErmesAuthorizationHelper : AuthorizationHelper
    {
        private readonly IAuthorizationConfiguration _authConfiguration;
        private readonly ErmesAppSession _session;
        public ErmesPermissionChecker ErmesPermissionChecker { get; set; }

        public ErmesAuthorizationHelper(
                IFeatureChecker featureChecker,
                ErmesAppSession session,
                IAuthorizationConfiguration authConfiguration) : base(featureChecker, authConfiguration)
        {
            _authConfiguration = authConfiguration;
            _session = session;
        }

        public override void Authorize(MethodInfo methodInfo, Type type)
        {
            if (!_authConfiguration.IsEnabled)
                return;

            var listAttributes = GetAttributes(methodInfo, type);
            if (AllowAnonymous(listAttributes))
                return;

            if (IsPropertyGetterSetterMethod(methodInfo, type))
                return;

            //No need to check request deriving from Localization
            if (methodInfo.Name.CompareTo("L") == 0)
                return;

            if (_session.Token == null || _session.Token.CompareTo(string.Empty) == 0)
                throw new AbpAuthorizationException(LocalizationManager.GetString(ErmesConsts.LocalizationSourceName, "InvalidToken"));

            var authorizeAttributes =
                    listAttributes
                    .OfType<IErmesAuthorizeAttribute>()
                    .ToArray();

            if (!authorizeAttributes.Any())
            {
                return;
            }

            Authorize(authorizeAttributes, _session.Roles);
        }

        public virtual void Authorize(IEnumerable<IErmesAuthorizeAttribute> authorizeAttributes, string[] roles)
        {
            foreach (var authorizeAttribute in authorizeAttributes)
            {
                ErmesPermissionCheckerExtensions.Authorize(ErmesPermissionChecker, authorizeAttribute.RequireAllPermissions, roles, authorizeAttribute.Permissions);
            }
        }

        private static List<object> GetAttributes(MemberInfo memberInfo, Type type)
        {
            var attributeList = new List<object>();
            attributeList.AddRange(memberInfo.GetCustomAttributes(true));
            attributeList.AddRange(type.GetTypeInfo().GetCustomAttributes(true));

            return attributeList;
        }

        private static bool AllowAnonymous(List<object> attributeList)
        {           
            return attributeList
                .OfType<IAbpAllowAnonymousAttribute>()
                .Any();
        }

        internal static bool IsPropertyGetterSetterMethod(MethodInfo method, Type type)
        {
            if (!method.IsSpecialName)
            {
                return false;
            }

            if (method.Name.Length < 5)
            {
                return false;
            }

            return type.GetProperty(method.Name.Substring(4), BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic) != null;
        }
    }
}

using Abp.Configuration.Startup;
using Abp.MultiTenancy;
using Abp.Runtime;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.Threading;
using Ermes.Persons;
using Ermes.Persons.Cache;
using System;
using System.Linq;

namespace Ermes
{
    public class ErmesAppSession : ClaimsAbpSession
    {
        private readonly PersonManager _personManager;
        private readonly ITypedCache<Guid, long?> _userIdFromFAGuidCache;
        private readonly PersonCache _personCache;
        public ErmesAppSession(
            IPrincipalAccessor principalAccessor,
            IMultiTenancyConfig multiTenancy,
            ITenantResolver tenantResolver,
            IAmbientScopeProvider<SessionOverride> sessionOverrideScopeProvider,
            PersonManager personManager,
            ICacheManager cacheManager,
            PersonCache personCache) :
            base(principalAccessor, multiTenancy, tenantResolver, sessionOverrideScopeProvider)
        {
            _personManager = personManager;
            _userIdFromFAGuidCache = cacheManager.GetCache<Guid, long?>("FusionAuthId>UserId");
            _personCache = personCache;
        }

        public string Token
        {
            get
            {
                var userTokenClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == ErmesConsts.TokenClaim);
                if (string.IsNullOrEmpty(userTokenClaim?.Value))
                {
                    return null;
                }

                return userTokenClaim.Value;
            }
        }

        public Guid? FusionAuthUserGuid
        {
            get
            {
                var fusionAuthUserId = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == ErmesConsts.FusionAuthUserGuidClaim);
                if (string.IsNullOrEmpty(fusionAuthUserId?.Value))
                {
                    return null;
                }
                return new Guid(fusionAuthUserId.Value);
            }
        }

        public void LoadUserId()
        {
            Guid? token = this.FusionAuthUserGuid;
            if (token == null)
                return;
            var person = AsyncHelper.RunSync(() =>  _personManager.GetPersonByFusionAuthUserGuidAsync(token.Value));
            _userIdFromFAGuidCache.Get(token.Value, token => person.Id);
        }

        public override long? UserId
        {
            get
            {
                Guid? token = this.FusionAuthUserGuid;
                if (token == null)
                    return null;
                return _userIdFromFAGuidCache.GetOrDefault(token.Value);
            }
        }

        public string[] Roles
        {
            get
            {
                var roles = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == ErmesConsts.RolesClaim);
                if (string.IsNullOrEmpty(roles.Value))
                {
                    return null;
                }
                return roles.Value.Split(',');
            }
        }

        public IPersonBase LoggedUserPerson
        {
            get
            {
                long? userid = this.UserId;
                if (userid == null)
                    return null;
                return _personCache.Get(userid.Value);
            }
        }
    }
}
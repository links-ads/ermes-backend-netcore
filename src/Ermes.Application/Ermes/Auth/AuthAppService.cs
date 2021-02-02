using Abp.Authorization;
using Ermes.Attributes;
using Ermes.Auth.Dto;
using io.fusionauth.domain.api;
using System;
using Microsoft.Extensions.Options;
using FusionAuthNetCore;
using Abp.UI;
using System.Threading.Tasks;
using Abp.Firebase;
using System.Collections.Generic;
using Ermes.Notifiers;
using Ermes.Dto;

namespace Ermes.Auth
{
    [ErmesAuthorize]
    [ErmesIgnoreApi(true)]
    public class AuthAppService : ErmesAppServiceBase
    {
        private readonly IOptions<FusionAuthSettings> _fusionAuthSettings;
        private readonly INotifierBase _notifierBase;

        public AuthAppService(
                    IOptions<FusionAuthSettings> fusionAuthSettings,
                    INotifierBase notifierBase
            )
        {
            _fusionAuthSettings = fusionAuthSettings;
            _notifierBase = notifierBase;
        }



        [AbpAllowAnonymous]
        public async Task<LoginOutput> Login(LoginInput input)
        {
            var res = new LoginOutput();
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);
            var response = await client.LoginAsync(new LoginRequest()
                            .with(lr => lr.applicationId = new Guid(_fusionAuthSettings.Value.ApplicationId))
                            .with(lr => lr.loginId = input.Email)
                            .with(lr => lr.password = input.Password));
            if (response.WasSuccessful())
            {
                res.Token = response.successResponse.token;
                res.RefreshToken = response.successResponse.refreshToken;
            }
            else
            {
                var fa_error = FusionAuth.ManageErrorResponse(response);
                throw new UserFriendlyException(fa_error.ErrorCode, fa_error.HasTranslation ? L(fa_error.Message) : fa_error.Message);
            }
            return res;
        }

        [AbpAllowAnonymous]
        public async Task<Dictionary<string, bool>> CheckFirebase(IdInput<string> regToken)
        {
            return await _notifierBase.SendTestPushNotificationAsync(regToken.Id);
        }


        //[AbpAllowAnonymous]
        //public async Task RevokeTokenHook(RevokeTokenHookInput hookEvent)
        //{
        //    if (hookEvent == null || hookEvent.Event == null)
        //        return;

        //    var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);
        //    var response = await client.RevokeRefreshTokenAsync(token: null, userId: hookEvent.Event.UserId,applicationId: hookEvent.Event.ApplicationId);
        //    if (response.WasSuccessful())
        //        return;
        //    else
        //    {
        //        var errResponse = FusionAuth.ManageErrorResponse(response);
        //        Logger.Error(string.Format("Revoke Token Hook error: {0}" + errResponse.Message));
        //    }
        //}


    }
}

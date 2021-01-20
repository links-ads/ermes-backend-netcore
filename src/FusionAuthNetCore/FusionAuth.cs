using com.inversoft.error;
using FusionAuthNetCore.Dto;
using io.fusionauth;
using io.fusionauth.domain;
using io.fusionauth.domain.api;
using io.fusionauth.domain.api.user;
using io.fusionauth.domain.search;
using System;
using System.Collections.Generic;

namespace FusionAuthNetCore
{
    public static class FusionAuth
    {
        public static FusionAuthClient client;
        public static FusionAuthClient GetFusionAuthClient(FusionAuthSettings fusionAuthSettings)
        {
            if (client == null) {
                if (fusionAuthSettings != null)
                {
                    client = new FusionAuthClient(fusionAuthSettings.ApiKey, fusionAuthSettings.Url, fusionAuthSettings.TenantId);
                    return client;
                }
                else
                    throw new Exception("Fusion auth config missing");
            }
            else
                return client;
        }

        public static BaseFusionAuthErrorDto ManageErrorResponse<T>(ClientResponse<T> response)
        {
            var res = new BaseFusionAuthErrorDto();
            res.ErrorCode = response.statusCode;

            if (response.errorResponse != null)
            {
                if (response.errorResponse.fieldErrors != null)
                {
                    foreach (KeyValuePair<string, List<Error>> err in response.errorResponse.fieldErrors)
                    {
                        foreach (var item in err.Value)
                            res.Message += string.Format("Field: {0}, Type:{1}, Message: {2}\n", err.Key, ((Func<String, String>)((s) => { return s.Length > 0 ? s + ']' : "[undef]"; }))(item.code.Split(']')[0]), item.message.Replace("[", "").Replace("]", ""));
                    }
                }
                else if (response.errorResponse.generalErrors != null)
                {
                    foreach (var err in response.errorResponse.generalErrors)
                    {
                        res.Message += string.Format("Code: {0}, Message: {1}\n", err.code, err.message);
                    }
                }
            }   
            if (response.exception != null)
            {
                res.Message += response.exception.Message;
            }

            if (res.Message == null)
            {
                switch(response)
                {
                    case ClientResponse<LoginResponse> t0:
                    {
                            switch(t0.statusCode)
                            {
                                case 404:
                                {
                                    res.Message = "InvalidCredentials";
                                    res.HasTranslation = true;
                                    break;
                                }
                                case 410:
                                case 423:
                                {
                                    res.Message = "User locked or expired";
                                    break;
                                }

                            }
                            break;
                    }
                }
                if (res.Message == null)
                {
                    res.Message = "Unknown Fusion Auth Error within " + response.GetType().GenericTypeArguments[0].FullName.Split('.')[^1];
                }
            }
            return res;
           
        }

        public static Sort GetFusionAuthSortParam(string value)
        {
            return value switch
            {
                "asc" => Sort.asc,
                "desc" => Sort.desc,
                _ => Sort.asc,
            };
        }
    }
}

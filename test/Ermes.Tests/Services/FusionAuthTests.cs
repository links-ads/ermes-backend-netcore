using Abp.UI;
using Castle.Facilities.TypedFactory;
using Ermes.Auth;
using Ermes.Reports.Dto;
using FusionAuthNetCore;
using io.fusionauth.domain;
using io.fusionauth.domain.api;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Ermes.Authorization.AppPermissions;

namespace Ermes.Tests.Services
{
    public class FusionAuthTests: ErmesTestBase
    {

        public FusionAuthTests()
        {
        }

        [Fact]
        public async Task Should_Retrieve_Token()
        {
            var token = await GetToken(USERNAME_OM);
            token.ShouldNotBeNull();
        }
    }
}

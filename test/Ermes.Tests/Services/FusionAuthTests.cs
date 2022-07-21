using Ermes.Auth;
using FusionAuthNetCore;
using io.fusionauth.domain.api;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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
            var token = GetToken("lucabruno1991@gmail.com");
            token.ShouldNotBeNull();
        }
    }
}

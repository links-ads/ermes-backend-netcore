using Abp.UI;
using Ermes.Auth;
using Ermes.Reports.Dto;
using FusionAuthNetCore;
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
            var token = await GetToken("lucabruno1991@gmail.com");
            token.ShouldNotBeNull();

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            Uri uri = new Uri("https://api-demo.shelter-project.cloud/api/services/app/" + "Reports/GetCategories?maxResultCount=100");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(request);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

            var responseValue = string.Empty;
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                Task task = response.Content.ReadAsStreamAsync().ContinueWith(t =>
                {
                    var stream = t.Result;
                    using var reader = new StreamReader(stream);
                    responseValue = reader.ReadToEnd();
                });

                task.Wait();

                var apiOutput = JsonConvert.DeserializeObject<GetCategoriesOutput>(responseValue);
                apiOutput.Categories.Count.ShouldBeGreaterThan(0);
            }
                
        }
    }
}

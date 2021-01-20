using System.Threading.Tasks;
using Ermes.Web.Controllers;
using Shouldly;
using Xunit;

namespace Ermes.Web.Tests.Controllers
{
    public class HomeController_Tests: ErmesWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}


using Microsoft.Extensions.Configuration;
using Moq;

namespace XWebAPI.Tests.Fixtures
{
    public class AuthenticationFixtures
    {

       
        public static Mock<IConfigurationSection> GetMockJWTSettings()
        {
            var mockConfigSection = new Mock<IConfigurationSection>();
            mockConfigSection.SetupGet(x => x["validIssuer"]).Returns("xwebapi");
            mockConfigSection.SetupGet(x => x["validAudience"]).Returns("http://localhost:3000");
            mockConfigSection.SetupGet(x => x["secretKey"]).Returns("secretkey0secretkey1secretkey2secretkey3secretkey4secretkey5secretkey6secretkey7secretkey8secretkey9");
            mockConfigSection.SetupGet(x => x["expires"]).Returns("60");

            return mockConfigSection;
        }


    }
}

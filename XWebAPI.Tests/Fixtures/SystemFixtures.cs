using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using XWebAPI.Utilities.AutoMapper;

namespace XWebAPI.Tests.Fixtures
{
    public class SystemFixtures
    {
        public static Mapper GetAPIsMapper()
        {
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            return new Mapper(configuration);
        }


        public static ControllerContext ControllerContextConfigure()
        {

            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpResponse = new Mock<HttpResponse>();

            var mockHeaderDictionary = new HeaderDictionary();

            mockHttpResponse.SetupGet(r => r.Headers).Returns(mockHeaderDictionary);

            mockHttpContext.SetupGet(m => m.Response).Returns(mockHttpResponse.Object);
           

            return new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };
        }

        public static ControllerContext ControllerContextConfigure(string Username)
        {

            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpResponse = new Mock<HttpResponse>();

            var mockHeaderDictionary = new HeaderDictionary();


            var fakeClaims = new List<Claim>
            {
                new(ClaimTypes.Name, Username),
            };

            var identity = new ClaimsIdentity(fakeClaims, It.IsAny<string>());
            var user = new ClaimsPrincipal(identity);

            mockHttpResponse.SetupGet(r => r.Headers).Returns(mockHeaderDictionary);

            mockHttpContext.SetupGet(m => m.Response).Returns(mockHttpResponse.Object);
            mockHttpContext.SetupGet(m => m.User).Returns(user);

            return new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };
        }

    }
}

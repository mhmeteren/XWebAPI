using Entities.DataTransferObjects.BaseUser;
using Entities.Exceptions.BaseUser;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;

namespace Presentation.Controllers.v1
{
    [ApiController]
    [Route("api/v1/Authentication")]
    public class AuthenticationController(IServiceManager serviceManager) : ControllerBase
    {

        private readonly IServiceManager _serviceManager = serviceManager;



        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> LoginUser([FromBody] BaseUserDtoForLogin baseUserDtoForLogin)
        {
            if (!await _serviceManager.AuthenticationService.ValidateUser(baseUserDtoForLogin))
                throw new BaseUserLoginBadRequestException();

            var tokenDto = await _serviceManager.AuthenticationService.CreateToken(populateExp: true);
            return Ok(tokenDto);

        }



        [HttpPost("refresh")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
        {
            var tokenDtoToReturn = await _serviceManager
                .AuthenticationService
                .RefreshToken(tokenDto);

            return Ok(tokenDtoToReturn);
        }


    }
}

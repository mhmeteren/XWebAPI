using Asp.Versioning;
using Entities.DataTransferObjects.BaseUser;
using Entities.Exceptions.BaseUser;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Presentation.Controllers.v1
{
    [ApiVersion(1)]
    [ApiController]
    [Route("api/v{v:apiVersion}/Authentication")]
    public class AuthenticationController(IServiceManager serviceManager) : ControllerBase
    {

        private readonly IServiceManager _serviceManager = serviceManager;



        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] BaseUserDtoForLogin baseUserDtoForLogin,
            [FromServices] IValidator<BaseUserDtoForLogin> validator)
        {
            var validatorResult = validator.Validate(baseUserDtoForLogin);
            if (!validatorResult.IsValid)
            {
                validatorResult.Errors.ForEach(error => ModelState.AddModelError(error.PropertyName, error.ErrorMessage));
                return BadRequest(ModelState);
            }

            if (!await _serviceManager.AuthenticationService.ValidateUser(baseUserDtoForLogin))
                throw new BaseUserLoginBadRequestException();

            var tokenDto = await _serviceManager.AuthenticationService.CreateToken(populateExp: true);
            return Ok(tokenDto);

        }



        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto,
            [FromServices] IValidator<TokenDto> validator)
        {

            var validatorResult = validator.Validate(tokenDto);
            if (!validatorResult.IsValid)
            {
                validatorResult.Errors.ForEach(error => ModelState.AddModelError(error.PropertyName, error.ErrorMessage));
                return BadRequest(ModelState);
            }

            var tokenDtoToReturn = await _serviceManager
                .AuthenticationService
                .RefreshToken(tokenDto);

            return Ok(tokenDtoToReturn);
        }


    }
}

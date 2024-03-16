using Entities.DataTransferObjects.User;
using Entities.RequestFeatures;
using Entities.UtilityClasses;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;


namespace Presentation.Controllers.v1
{

    [ApiController]
    [Route("api/v1/User")]
    public class UserController(IServiceManager serviceManager) : ControllerBase
    {
        readonly private IServiceManager _serviceManager = serviceManager;


        [HttpGet("{Username}")]
        public async Task<IActionResult> GetUserByUsername([FromRoute] string Username)
        {
            return Ok(await _serviceManager.UserService.GetUserProfileByUsernameAsync(Username));
        }


        [Authorize(Roles = Roles.User)]
        [HttpGet("{Username}/Followers")]
        public async Task<IActionResult> GetAllFollowersByUserName([FromRoute] string Username, [FromQuery] FollowParameters parameters)
        {
            var pagedResponse = await _serviceManager.FollowsService.GetAllFollowersAsync(Username, User.Identity.Name, parameters, false);
            Response.Headers["X-Pagination"] = pagedResponse.MetaData.ToString();
            return Ok(pagedResponse.Items);
        }


        [Authorize(Roles = Roles.User)]
        [HttpGet("{Username}/Following")]
        public async Task<IActionResult> GetAllFollowingByUserName([FromRoute] string Username, [FromQuery] FollowParameters parameters)
        {
            var pagedResponse = await _serviceManager.FollowsService.GetAllFollowingsAsync(Username, User.Identity.Name, parameters, false);
            Response.Headers["X-Pagination"] = pagedResponse.MetaData.ToString();
            return Ok(pagedResponse.Items);
        }






        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] UserDtoForRegister userDto,
            [FromServices] IValidator<UserDtoForRegister> validator)
        {

            var validatorResult = validator.Validate(userDto);
            if (!validatorResult.IsValid)
            {
                validatorResult.Errors.ForEach(error => ModelState.AddModelError(error.PropertyName, error.ErrorMessage));
                return BadRequest(ModelState);
            }

            var result = await _serviceManager.UserService.ReqisterUserAsync(userDto);
            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(error => ModelState.AddModelError(error.Code, error.Description));
                return BadRequest(ModelState);
            }
            return StatusCode(201);
        }



        [Authorize(Roles = Roles.User)]
        [HttpPut("Update/ProfileImage")]
        public async Task<IActionResult> UserUpdateProfileImage(IFormFile profileImage)
        {
            await _serviceManager
                .UserService
                .UpdateProfileImage(profileImage, User.Identity.Name);

            return NoContent();
        }



        [Authorize(Roles = Roles.User)]
        [HttpPut("Update/BackgroundImage")]
        public async Task<IActionResult> UserUpdateBackgroundImage(IFormFile backgroundImage)
        {
            await _serviceManager
                .UserService
                .UpdateBackgroundImage(backgroundImage, User.Identity.Name);

            return NoContent();
        }



        [Authorize(Roles = Roles.User)]
        [HttpPut("Update/Profile")]
        public async Task<IActionResult> UserUpdateProfile([FromBody] UserDtoForAccountUpdate profileDto,
            [FromServices] IValidator<UserDtoForAccountUpdate> validator)
        {

            var validatorResult = validator.Validate(profileDto);
            if (!validatorResult.IsValid)
            {
                validatorResult.Errors.ForEach(error => ModelState.AddModelError(error.PropertyName, error.ErrorMessage));
                return BadRequest(ModelState);
            }


            var result = await _serviceManager
                  .UserService
                  .UpdateProfile(User.Identity.Name, profileDto);

            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(error => ModelState.AddModelError(error.Code, error.Description));
                return BadRequest(ModelState);
            }

            return Ok(new
            {
                StatusCode = 200,
                Message = "Profile has been successfully updated"
            });
        }


    }
}

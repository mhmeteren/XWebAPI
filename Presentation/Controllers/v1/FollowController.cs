
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Entities.UtilityClasses;


namespace Presentation.Controllers.v1
{


    [ApiController]
    [Route("api/v1/Follow")]
    public class FollowController(IServiceManager serviceManager) : ControllerBase
    {
        readonly private IServiceManager _serviceManager = serviceManager;


        [Authorize(Roles = Roles.User)]
        [HttpPost("{userId}")]
        public async Task<IActionResult> FollowUser([FromRoute] string userId)
        {
            await _serviceManager.FollowsService.UserFollow(User.Identity.Name, userId);
            return StatusCode(201);
        }



        [Authorize(Roles = Roles.User)]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> UnFollowUser([FromRoute] string userId)
        {
            await _serviceManager.FollowsService.UserUnFollow(User.Identity.Name, userId);
            return NoContent();
        }



        [Authorize(Roles = Roles.User)]
        [HttpDelete("DeleteFollower/{userId}")]
        public async Task<IActionResult> DeleteFollower([FromRoute] string userId)
        {
            await _serviceManager.FollowsService.DeleteFollower(User.Identity.Name, userId);
            return NoContent();
        }

    }
}

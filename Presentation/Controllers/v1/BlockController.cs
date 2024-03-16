
using Asp.Versioning;
using Entities.RequestFeatures;
using Entities.UtilityClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Presentation.Controllers.v1
{

    [ApiVersion(1)]
    [ApiController]
    [Route("api/v{v:apiVersion}/Block")]
    public class BlockController(IServiceManager serviceManager) : ControllerBase
    {
        private readonly IServiceManager _serviceManager = serviceManager;


        [Authorize(Roles = Roles.User)]
        [HttpGet("me")]
        public async Task<IActionResult> GetAllBlockedUsers([FromRoute] BlockedUserParameters parameters)
        {
            var pagedResponse = await _serviceManager
                .BlockedUsersService
                .GetAllBlockedUsersByBlockerUserIdAsync(User.Identity.Name, parameters, false);

            Response.Headers["X-Pagination"] = pagedResponse.MetaData.ToString();
            return Ok(pagedResponse.Items);
        }



        [Authorize(Roles = Roles.User)]
        [HttpPost("{userId}")]
        public async Task<IActionResult> BlockUser([FromRoute] string userId)
        {
            await _serviceManager.BlockedUsersService.BlockedUser(User.Identity.Name, userId);
            return StatusCode(201);
        }



        [Authorize(Roles = Roles.User)]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> UnBlockUser([FromRoute] string userId)
        {
            await _serviceManager.BlockedUsersService.UnBlockedUser(User.Identity.Name, userId);
            return NoContent();
        }

    }
}

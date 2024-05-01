using Asp.Versioning;
using Entities.Enums;
using Entities.RequestFeatures;
using Entities.UtilityClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;


namespace Presentation.Controllers.v1
{
    [ApiVersion(1)]
    [ApiController]
    [Route("api/v{v:apiVersion}/Feeds")]
    public class FeedsController(IServiceManager serviceManager) : ControllerBase
    {

        readonly private IServiceManager _serviceManager = serviceManager;

        
        [Authorize(Roles = Roles.User)]
        [HttpGet("{Username}/{FeedType}")]
        public async Task<IActionResult> GetAllTweetsByUser(
            [FromRoute] string Username,
            [FromRoute] string FeedType,
            [FromQuery] TweetParameters parameters)
        {

            if (!Enum.TryParse(FeedType, out UserFeedType state))
            {
                ModelState.AddModelError("FeedType", "Invalid FeedType value.");
                return BadRequest(ModelState);
            }

            var pagedResponse = await _serviceManager.TweetsService.GetAllTweetsByUserAsync(Username, User.Identity.Name, state, parameters, false);    
            Response.Headers["X-Pagination"] = pagedResponse.MetaData.ToString();
            return Ok(pagedResponse.Items);
        }



        [Authorize(Roles = Roles.User)]
        [HttpGet("{MainFeedType}")]
        public async Task<IActionResult> GetAllFollowingTweets([FromRoute] string MainFeedType, [FromRoute] TweetParameters parameters)
        {

            if (!Enum.TryParse(MainFeedType, out MainFeedType state))
            {
                ModelState.AddModelError("MainFeedType", "Invalid MainFeedType value.");
                return BadRequest(ModelState);
            }

            var pagedResponse = state switch
            {
                Entities.Enums.MainFeedType.ForMe => throw new NotImplementedException(),
                Entities.Enums.MainFeedType.OnlyFollowings => await _serviceManager.TweetsService.GetAllFollowingTweetsByUserAsync(User.Identity.Name, parameters, false),
            };

            Response.Headers["X-Pagination"] = pagedResponse.MetaData.ToString();
            return Ok(pagedResponse.Items);
        }


    }
}

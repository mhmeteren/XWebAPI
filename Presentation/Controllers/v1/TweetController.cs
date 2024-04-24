using Asp.Versioning;
using Entities.DataTransferObjects.Tweets;
using Entities.UtilityClasses;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Presentation.Controllers.v1
{

    [ApiVersion(1)]
    [ApiController]
    [Route("api/v{v:apiVersion}/Tweet")]
    public class TweetController(IServiceManager serviceManager) : ControllerBase
    {
        private readonly IServiceManager _serviceManager = serviceManager;


        [Authorize(Roles = Roles.User)]
        [HttpGet("{tweetId}")]
        public async Task<IActionResult> GetTweetById([FromRoute] string tweetId)
        {
            return Ok(await _serviceManager.TweetsService.GetTweetById(tweetId, User.Identity.Name, false));
        }




        [Authorize(Roles = Roles.User)]
        [HttpPost]
        public async Task<IActionResult> CreateTweet([FromForm] TweetDtoForCreate tweetDto,
            [FromServices] IValidator<TweetDtoForCreate> validator)
        {
            var validatorResult = validator.Validate(tweetDto);
            if (!validatorResult.IsValid)
            {
                validatorResult.Errors.ForEach(error => ModelState.AddModelError(error.PropertyName, error.ErrorMessage));
                return BadRequest(ModelState);
            }

            await _serviceManager.TweetsService.CreateTweet(User.Identity.Name, tweetDto);

            return StatusCode(201);
        }



        [Authorize(Roles = Roles.User)]
        [HttpPut("{tweetId}/Edit")]
        public async Task<IActionResult> TweetUpdate(
            [FromRoute] string tweetId,
            [FromForm] TweetDtoForEdit tweetDto,
            [FromServices] IValidator<TweetDtoForEdit> validator)
        {
            var validatorResult = validator.Validate(tweetDto);
            if (!validatorResult.IsValid)
            {
                validatorResult.Errors.ForEach(error => ModelState.AddModelError(error.PropertyName, error.ErrorMessage));
                return BadRequest(ModelState);
            }

            await _serviceManager.TweetsService.EditTweet(tweetId, User.Identity.Name, tweetDto);

            return NoContent();
        }




        [Authorize(Roles = Roles.User)]
        [HttpDelete("{tweetId}/Delete")]
        public async Task<IActionResult> TweetDelete([FromRoute] string tweetId)
        {
            await _serviceManager.TweetsService.DeleteTweet(tweetId, User.Identity.Name);
            return NoContent();
        }



        [Authorize(Roles = Roles.User)]
        [HttpPost("{tweetId}/Likes")]
        public async Task<IActionResult> TweetLikes ([FromRoute] string tweetId)
        {
            await _serviceManager.TweetsService.CreateTweetLike(tweetId, User.Identity.Name);
            return StatusCode(201);
        }


        [Authorize(Roles = Roles.User)]
        [HttpDelete("{tweetId}/Likes")]
        public async Task<IActionResult> TweetDeleteLikes ([FromRoute] string tweetId)
        {
            await _serviceManager.TweetsService.DeleteTweetLike(tweetId, User.Identity.Name);
            return NoContent();
        }

    }
}

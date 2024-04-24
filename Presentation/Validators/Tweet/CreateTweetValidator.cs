
using Entities.DataTransferObjects.Tweets;
using Entities.Enums;
using FluentValidation;

namespace Presentation.Validators.Tweet
{
    public sealed class CreateTweetValidator : AbstractValidator<TweetDtoForCreate>
    {
        public CreateTweetValidator()
        {
            RuleFor(i => i.MainTweetID)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(i =>  ValidatorHelpers.ValidateNullableString(i, 36))
                 .WithMessage("MainTweetID must be null or have a maximum length of 36 characters.");

            RuleFor(i => i.Content)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(i => ValidatorHelpers.ValidateNullableString(i, 500))
                .WithMessage("Content must be null or have a maximum length of 500 characters.");



            RuleFor(i => i.Repliers)
                .Must(i => ValidatorHelpers.IsEnumValue<ReplierType>(i))
                .WithMessage("Invalid Repliers value.");


            RuleFor(x => x.IsRetweet)
                 .Must((dto, isRetweet) =>
                 {
                     if (isRetweet)
                     {
                         return dto.MainTweetID != null;
                     }
                     return true;
                 }).WithMessage("If IsRetweet is true, MainTweetID must not be null.");


            RuleFor(x => x.IsRetweet)
              .Must((dto, isRetweet) =>
              {
                  if (!isRetweet)
                  {
                      return !string.IsNullOrWhiteSpace(dto.Content) || (dto.Medias != null && dto.Medias.Any());
                  }
                  return true;
              }).WithMessage("Either Content or Media must be provided when IsRetweet is false.");


            RuleFor(x => x.Medias)
                .Must(i => ValidatorHelpers.ValidateNullableList(i, 10))
                .WithMessage("Medias must be null or have a maximum count of 10 items.");
        }
    }
}

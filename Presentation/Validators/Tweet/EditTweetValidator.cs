
using Entities.DataTransferObjects.Tweets;
using Entities.Enums;
using FluentValidation;

namespace Presentation.Validators.Tweet
{
    public sealed class EditTweetValidator : AbstractValidator<TweetDtoForEdit>
    {
        public EditTweetValidator()
        {
            RuleFor(i => i.Id)
                 .Must(i => ValidatorHelpers.ValidateNullableString(i, 36))
                 .WithMessage("Id must be null or have a maximum length of 36 characters.");

            RuleFor(i => i.Content)
                .Must(i => ValidatorHelpers.ValidateNullableString(i, 500))
                .WithMessage("Content must be null or have a maximum length of 500 characters.");



            RuleFor(i => i.Repliers)
                .Must(i => ValidatorHelpers.IsEnumValue<ReplierType>(i))
                .WithMessage("Invalid Repliers value.");


            RuleFor(x => new { x.Medias, x.DeletedMediaIds })
                 .Must(items =>
                 {
                     var totalItemCount = (items.Medias?.Count ?? 0) + (items.DeletedMediaIds?.Count ?? 0);
                     return totalItemCount <= 10;
                 })
                 .WithMessage("The total count of Medias and Deleted Media must not exceed 10 items.")
                 .WithName("Medias");

        }
    }
}

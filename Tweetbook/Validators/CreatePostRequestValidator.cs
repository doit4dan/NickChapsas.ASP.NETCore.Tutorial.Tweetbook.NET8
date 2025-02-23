using FluentValidation;
using Tweetbook.Contracts.V1.Requests;

namespace Tweetbook.Validators
{
    public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
    {
        public CreatePostRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(1000);

            RuleFor(x => x.Tags)
                .NotEmpty();
            
            RuleForEach(x => x.Tags)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9# ]*$"); // alphanumeric chars & # ( regex expression )
        }
    }
}

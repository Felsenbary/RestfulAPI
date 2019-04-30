using FluentValidation;
using ToDoApi.Models;

namespace ToDoApi.Validation
{
    public class PlayerValidation : AbstractValidator<Player>
    {
        public PlayerValidation()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name of player is required");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name of player is required");
        }
    }
}

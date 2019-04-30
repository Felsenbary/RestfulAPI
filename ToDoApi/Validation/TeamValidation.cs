using FluentValidation;
using System.Threading.Tasks;
using ToDoApi.Models;

namespace ToDoApi.Validation
{
    public class TeamValidation : AbstractValidator<Team>
    {
        public TeamValidation()
        {
            RuleFor(x => x.Location).NotEmpty().WithMessage("The name of team is required");
            RuleFor(x => x.Name).NotEmpty().WithMessage("The location of team is required");
            RuleFor(x => x.Players).Must(x => x.Count > 7).WithMessage("Eight players are required in a team");
        }
    }
}

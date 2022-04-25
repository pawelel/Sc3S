using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;

namespace Sc3S.Validators;

public class QuestionUpdateCommandValidator : AbstractValidator<QuestionUpdateCommand>
{
    
    public QuestionUpdateCommandValidator()
    {
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Name).NotEmpty().NotNull().Length(3, 50);
        RuleFor(x => x.QuestionId).GreaterThanOrEqualTo(0);
        
    }
}

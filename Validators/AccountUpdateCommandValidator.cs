using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;

namespace Sc3S.Validators;

public class AccountUpdateCommandValidator : AbstractValidator<AccountUpdateCommand>
{
    
    public AccountUpdateCommandValidator()
    {
        CascadeMode = CascadeMode.Stop;
      
        RuleFor(x => x.UserName)
          .Length(4, 13).Matches("^[a-zA-Z0-9]*$");

        RuleFor(x => x.Email).EmailAddress().Length(14, 50);
        RuleFor(x => x.Password)
            .Length(8, 23)
            .Matches(@"[A-Z]+")
            .Matches(@"[a-z]+")
            .Matches(@"[0-9]+")
            .Matches(@"[\!\?\*\.]+");
        RuleFor(x => x.RoleId).NotEmpty();
    }
}
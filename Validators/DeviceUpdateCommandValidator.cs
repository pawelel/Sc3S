﻿using FluentValidation;
using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;
using Sc3S.Entities;

namespace Sc3S.Validators;

public class DeviceUpdateCommandValidator : AbstractValidator<DeviceUpdateCommand>
{

    public DeviceUpdateCommandValidator()
    {
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Name).NotEmpty().NotNull().Length(3, 50);
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => x.DeviceId).GreaterThanOrEqualTo(0);
    }
}

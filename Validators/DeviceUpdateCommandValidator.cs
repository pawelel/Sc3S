using FluentValidation;
using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;
using Sc3S.Entities;

namespace Sc3S.Validators;

public class DeviceUpdateCommandValidator : AbstractValidator<DeviceUpdateCommand>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;

    public DeviceUpdateCommandValidator(IDbContextFactory<Sc3SContext> factory)
    {
        _factory = factory;
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Name).NotEmpty().NotNull().Length(3, 50);
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x=>x.DeviceId).GreaterThanOrEqualTo(0);
        RuleFor(x => new { x.Name, x.DeviceId }).MustAsync(async (x, CancellationToken) =>
        {
            return await ValidateDevice(x.Name, x.DeviceId);
        });
    }
    async Task<bool> ValidateDevice(string name, int deviceId)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        return !await ctx.Devices.AsNoTracking().Where(a => a.Name.ToLower().Trim() == name.ToLower().Trim() && a.DeviceId != deviceId).AnyAsync();
    }
    
    async Task<List<Device>> GetDevices(string name, CancellationToken cancellationToken)
    {
        await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);
        return await ctx.Devices.AsNoTracking().Where(a => a.Name.ToLower().Trim() == name.ToLower().Trim()).ToListAsync(cancellationToken);
    }
}

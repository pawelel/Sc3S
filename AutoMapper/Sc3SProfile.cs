using AutoMapper;

using Sc3S.CQRS.Commands;
using Sc3S.CQRS.Queries;
using Sc3S.Entities;

namespace Sc3S.AutoMapper;

public class Sc3SProfile : Profile
{
    public Sc3SProfile()
    {
        CreateMap<Account, AccountQuery>();
        CreateMap<DeviceQuery, DeviceUpdateCommand>();
        CreateMap<PlantQuery, PlantUpdateCommand>();
    }
}

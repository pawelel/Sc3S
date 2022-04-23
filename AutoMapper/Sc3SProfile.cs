using AutoMapper;

using Sc3S.DTO;
using Sc3S.Entities;

namespace Sc3S.AutoMapper;

public class Sc3SProfile : Profile
{
    public Sc3SProfile()
    {
        CreateMap<Account, AccountDto>();
    }
}

using AutoMapper;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.MappingProfile;

public class ExternalProviderProfile : Profile
{
    public ExternalProviderProfile()
    {
        CreateMap<ExternalProvider, CreateOrUpdateExternalProviderDto>().ReverseMap();
    }


}

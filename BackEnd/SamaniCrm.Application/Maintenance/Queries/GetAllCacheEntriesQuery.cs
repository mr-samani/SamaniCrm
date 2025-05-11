using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Core.Shared.DTOs;

namespace SamaniCrm.Application.Maintenance.Queries
{
    public record GetAllCacheEntriesQuery(string? Pattern = null) : IRequest<List<CacheEntryDto>>;

}

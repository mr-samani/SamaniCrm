using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Pages.Queries
{
    public class GetFilteredPagesQuery : PaginationRequest, IRequest<PaginatedResult<PageDto>>
    {
        public required PageTypeEnum Type { get; set; }
        public string? Title { get; set; }
        public string? Introduction { get; set; }
        public string? AuthorName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public PageStatusEnum? Status { get; set; }
    }
}

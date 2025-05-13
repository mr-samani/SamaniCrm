using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Pages.Commands
{
    public class CreatePageCommand : IRequest<Guid>
    {
        public string? Slag { get; set; }
        public string? CoverImage { get; set; }
        public PageStatusEnum Status { get; set; }
        public string Culture { get; set; } = default!;
        public string? Title { get; set; }
        public string? Abstract { get; set; }
        public string? Description { get; set; }
        public string? Html { get; set; }
        public string? Styles { get; set; }
        public string? Scripts { get; set; }
        public string? CreatedBy { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SamaniCrm.Application.Pages.Commands
{
    public class UpdatePageCommand : IRequest<Unit>
    {
        public Guid PageId { get; set; }
        public string Culture { get; set; } = default!;
        public string? Title { get; set; }
        public string? Abstract { get; set; }
        public string? Description { get; set; }
        public string? Html { get; set; }
        public string? Styles { get; set; }
        public string? Scripts { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}

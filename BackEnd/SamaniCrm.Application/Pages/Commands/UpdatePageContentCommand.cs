using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SamaniCrm.Application.Pages.Commands
{
    public class UpdatePageContentCommand : IRequest<Unit>
    {
        public Guid PageId { get; set; }
        public string Culture { get; set; } = default!;
        public string? Data { get; set; }
        public string? Html { get; set; }
        public string? Styles { get; set; }
        public string? Scripts { get; set; }
    }
}

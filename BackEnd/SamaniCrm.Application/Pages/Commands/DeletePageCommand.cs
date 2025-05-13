using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SamaniCrm.Application.Pages.Commands
{
    public class DeletePageCommand : IRequest<Unit>
    {
        public Guid PageId { get; set; }
        public string? DeletedBy { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.Pages.Commands
{
    public class DeletePageCommandHandler : IRequestHandler<DeletePageCommand, Unit>
    {
        private readonly IPageService _pageService;

        public DeletePageCommandHandler(IPageService pageService)
        {
            _pageService = pageService;
        }

        public async Task<Unit> Handle(DeletePageCommand request, CancellationToken cancellationToken)
        {
            return await _pageService.DeletePage(request, cancellationToken);
        }
    }
}

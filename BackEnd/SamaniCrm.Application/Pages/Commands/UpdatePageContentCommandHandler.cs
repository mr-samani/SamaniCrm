using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Pages.Commands
{
    public class UpdatePageContentCommandHandler : IRequestHandler<UpdatePageContentCommand, Unit>
    {
        private readonly IPageService _pageService;

        public UpdatePageContentCommandHandler(IPageService pageService)
        {
            _pageService = pageService;
        }

        public async Task<Unit> Handle(UpdatePageContentCommand request, CancellationToken cancellationToken)
        {
            return await _pageService.UpdatePageContent(request, cancellationToken);
        }
    }
}

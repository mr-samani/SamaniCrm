using MediatR;
using SamaniCrm.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Pages.Commands;

public class DeleteCustomBlockCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}

public class DeleteCustomBlockCommandHandler : IRequestHandler<DeleteCustomBlockCommand, Unit>
{
    private readonly IPageService _pageService;

    public DeleteCustomBlockCommandHandler(IPageService pageService)
    {
        _pageService = pageService;
    }

    public async Task<Unit> Handle(DeleteCustomBlockCommand request, CancellationToken cancellationToken)
    {
        return await _pageService.DeleteCustomBlockPage(request, cancellationToken);
    }

}
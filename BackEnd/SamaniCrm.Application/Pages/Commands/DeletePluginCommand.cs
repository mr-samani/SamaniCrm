using MediatR;
using SamaniCrm.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Pages.Commands;

public record DeletePluginCommand(Guid Id) : IRequest<Unit>;


public class DeletePluginCommandHandler : IRequestHandler<DeletePluginCommand, Unit>
{
    private readonly IPageService _pageService;

    public DeletePluginCommandHandler(IPageService pageService)
    {
        _pageService = pageService;
    }

    public async Task<Unit> Handle(DeletePluginCommand request, CancellationToken cancellationToken)
    {
        return await _pageService.DeletePluginPage(request, cancellationToken);
    }

}
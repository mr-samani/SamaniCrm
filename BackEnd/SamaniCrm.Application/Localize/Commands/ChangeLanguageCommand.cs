using MediatR;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.User.Commands;
using SamaniCrm.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Localize.Commands;

public record ChangeLanguageCommand(string culture) : IRequest<bool>;



public class ChangeLanguageCommandHandler : IRequestHandler<ChangeLanguageCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public ChangeLanguageCommandHandler(ICurrentUserService currentUserService, IMediator mediator)
    {
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<bool> Handle(ChangeLanguageCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId != null)
        {
            return await _mediator.Send(new ChangeUserLanguageCommand(request.culture), cancellationToken);
        }
        return true;
    }
}
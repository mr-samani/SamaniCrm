using MediatR;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs.PageBuilder;
using SamaniCrm.Application.FileManager.Commands;
using SamaniCrm.Core;
using SamaniCrm.Core.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Pages.Commands;

public class CreatePluginCommand : PluginDto, IRequest<Guid>
{

}


public class CreatePluginCommandHandler : IRequestHandler<CreatePluginCommand, Guid>
{
    private readonly IPageService _pageService;
    private readonly IMediator _mediator;

    public CreatePluginCommandHandler(IPageService pageService, IMediator mediator)
    {
        _pageService = pageService;
        _mediator = mediator;
    }

    public async Task<Guid> Handle(CreatePluginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Image) == false)
        {
            var stream = Base64FileHelper.ConvertToStream(
           request.Image,
           out var contentType
       );

            var command = new UploadFileCommand
            {
                FileStreem = stream,
                FolderId = AppConsts.PluginsFolderId.ToString(),
                FileName = request.Name.Replace(" ", "_"),
                filetype = contentType
            };

            var fileId = await _mediator.Send(command, cancellationToken);
            request.Image = fileId.ToString();
        }


        return await _pageService.CreatePlugin(request, cancellationToken);
    }
}
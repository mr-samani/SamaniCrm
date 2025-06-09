using FluentValidation;
using MediatR;
using SamaniCrm.Application.FileManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.FileManager.Commands;

public class UploadFileCommand : IRequest<Guid>
{

    public required Stream FileStreem { get; set; }
    public required string FolderId { get; set; }
    public required string FileName { get; set; }
    public required string filetype { get; set; }


}

public class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
{
    public UploadFileCommandValidator()
    {
        RuleFor(x => x.FileName).NotEmpty();
        //RuleFor(x => x.ContentType).Must(ct => ct == "image/png" || ct == "video/mp4");
        //RuleFor(x => x.Size).LessThanOrEqualTo(5L * 1024 * 1024 * 1024);
    }
}

public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, Guid>
{
    private readonly IFileManagerService _fileManagerService;

    public UploadFileCommandHandler(IFileManagerService fileManagerService)
    {
        _fileManagerService = fileManagerService;
    }

    public Task<Guid> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        Guid.TryParse(request.FolderId, out var folterId);
        return _fileManagerService.UploadFile(folterId, request.FileStreem, request.FileName, cancellationToken);
    }
}
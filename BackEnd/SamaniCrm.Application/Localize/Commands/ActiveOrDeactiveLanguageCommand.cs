using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Localize.Commands
{

    public record ActiveOrDeactiveLanguageCommand(string Culture, bool isActive) : IRequest<bool>;

    public class ActiveOrDeactiveLanguageCommandLandler : IRequestHandler<ActiveOrDeactiveLanguageCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public ActiveOrDeactiveLanguageCommandLandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(ActiveOrDeactiveLanguageCommand request, CancellationToken cancellationToken)
        {
            var language = await _dbContext.Languages.FindAsync(new object[] { request.Culture }, cancellationToken);
            if (language is null) throw new NotFoundException(nameof(Language), request.Culture);
            language.IsActive = request.isActive;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

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
    public record DeleteLocalizeKeyCommand(Guid keyId):IRequest<bool>;

    public class DeleteLocalizeKeyCommandLandler : IRequestHandler<DeleteLocalizeKeyCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public DeleteLocalizeKeyCommandLandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteLocalizeKeyCommand request, CancellationToken cancellationToken)
        {
            var key = await _dbContext.Localizations.FindAsync(new object[] { request.keyId }, cancellationToken);
            if (key is null) throw new NotFoundException(nameof(Localization), request.keyId);

            _dbContext.Localizations.Remove(key);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }


    }
}

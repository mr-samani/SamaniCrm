using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManager.Commands
{
    

    public record DeleteCurrencyCommand(Guid Id) : IRequest<bool>;

    public class DeleteCurrencyCommandHandler : IRequestHandler<DeleteCurrencyCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        public DeleteCurrencyCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Currency.FindAsync(request.Id);
            if (entity == null)
                throw new NotFoundException("Currency not found.");
            if (entity.IsDefault)
                throw new UserFriendlyException("Can not delete default currency");

            entity.IsDeleted = true;
            entity.DeletedTime = DateTime.UtcNow;
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }


}

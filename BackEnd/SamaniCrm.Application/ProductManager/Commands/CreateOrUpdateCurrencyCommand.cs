using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.ProductManager.Dtos;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManager.Commands
{

    public class CreateOrUpdateCurrencyCommand : CurrencyDto, IRequest<bool>;

    public class CreateOrUpdateCurrencyCommandHandler : IRequestHandler<CreateOrUpdateCurrencyCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public CreateOrUpdateCurrencyCommandHandler(IApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }



        public async Task<bool> Handle(CreateOrUpdateCurrencyCommand request, CancellationToken cancellationToken)
        {
            Currency? found = null;

            if (request.Id != null)
            {
                found = await _dbContext.Currency.FindAsync(new object[] { request.Id }, cancellationToken);
            }
            if (found == null)
            {
                var newItem = new Currency
                {
                    CurrencyCode = request.CurrencyCode,
                    Name = request.Name,
                    IsActive = request.IsActive,
                    IsDefault = request.IsDefault,
                    ExchangeRateToBase = request.ExchangeRateToBase,
                    Symbol = request.Symbol,
                };

                var r = await _dbContext.Currency.AddAsync(newItem, cancellationToken);
                found = r.Entity;
            }
            else
            {
                found.Name = request.Name;
                found.IsActive = request.IsActive;
                found.IsDefault = request.IsDefault;
                found.CurrencyCode = request.CurrencyCode;
                found.ExchangeRateToBase = request.ExchangeRateToBase;
                found.Symbol = request.Symbol;

            }
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            if (request.IsDefault && result > 0)
            {
                await SetAsDefault(found.Id, cancellationToken);
            }
            return result > 0;
        }



        private async Task SetAsDefault(Guid id, CancellationToken cancellationToken)
        {
            var found = await _dbContext.Currency.FindAsync(new object[] { id }, cancellationToken);
            if (found == null)
                throw new NotFoundException("Currency not found.");

            if (!found.IsActive)
                throw new UserFriendlyException("Currency is not active.");

            // Find all previous defaults (including current one, in case it was already default)
            var previousDefaults = await _dbContext.Currency
                .Where(w => w.IsDefault && w.Id != id)
                .ToListAsync(cancellationToken);

            // Remove default flag from previous ones
            foreach (var currency in previousDefaults)
            {
                currency.IsDefault = false;
            }

            found.IsDefault = true;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }




    }
}

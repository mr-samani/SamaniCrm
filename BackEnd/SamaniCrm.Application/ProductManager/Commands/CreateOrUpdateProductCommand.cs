using MediatR;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using System;

namespace SamaniCrm.Application.ProductManagerManager.Commands
{
    public class CreateOrUpdateProductCommand : ProductDto, IRequest<Guid>
    {
    }
}

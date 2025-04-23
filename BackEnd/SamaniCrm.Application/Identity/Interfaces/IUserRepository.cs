using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Application.Identity.DTOs;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Identity.Interfaces;
public interface IUserRepository
{
    Task<IUser?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
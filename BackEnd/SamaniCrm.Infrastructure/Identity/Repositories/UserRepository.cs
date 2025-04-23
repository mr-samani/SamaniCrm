using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Identity.DTOs;
using SamaniCrm.Application.Identity.Interfaces;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.Identity.Repositories;
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IUser?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new ApplicationUserDto
            {
                Id = u.Id,
                UserName = u.UserName!,
                Email = u.Email!
            })
            .FirstOrDefaultAsync(cancellationToken);

        return user;
    }
}
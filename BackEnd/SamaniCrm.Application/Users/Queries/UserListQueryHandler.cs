using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Infrastructure.Identity;
using System.Linq.Dynamic.Core;



namespace SamaniCrm.Application.Users.Queries
{
    public class UserListQueryHandler : IRequestHandler<UserListQuery, PaginatedResult<UserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserListQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<PaginatedResult<UserDto>> Handle(UserListQuery request, CancellationToken cancellationToken)
        {
            IQueryable<ApplicationUser> query = _userManager.Users.AsQueryable();
            if (!string.IsNullOrEmpty(request.Filter))
            {
                query = query.Where(x =>
                x.UserName.Contains(request.Filter) ||
                x.FirstName.Contains(request.Filter) ||
                x.LastName.Contains(request.Filter) ||
                x.Email.Contains(request.Filter)
                );
            }

            // Sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                var sortString = $"{request.SortBy} {request.SortDirection}";
                query = query.OrderBy(sortString);
            }
            // old version
            //query = request.SortBy?.ToLower() switch
            //{
            //    "email" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            //    "firstname" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
            //    "lastname" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName),
            //    _ => request.SortDirection == "desc" ? query.OrderByDescending(u => u.UserName) : query.OrderBy(u => u.UserName),
            //};

            int total = await query.CountAsync(cancellationToken);

            var users =await query
                .Skip(request.PageSize * (request.PageNumber - 1))
                .Take(request.PageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    ProfilePicture = u.ProfilePicture
                })
                .ToListAsync(cancellationToken);


            return new PaginatedResult<UserDto>
            {
                Items = users,
                TotalCount = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }



}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Auth.Queries
{
    //public class ValidateUserQueryHandler : IRequestHandler<ValidateUserQuery, IUser?>
    //{
    //    private readonly UserManager<IUser> _userManager;
    //    private readonly SignInManager<IUser> _signInManager;

    //    public ValidateUserQueryHandler(UserManager<IUser> userManager, SignInManager<IUser> signInManager)
    //    {
    //        _userManager = userManager;
    //        _signInManager = signInManager;
    //    }

    //    public async Task<IUser?> Handle(ValidateUserQuery request, CancellationToken cancellationToken)
    //    {
    //        var user = await _userManager.FindByNameAsync(request.Username);
    //        if (user == null)
    //            return null;

    //        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
    //        return result.Succeeded ? user : null;
    //    }
    //}

}

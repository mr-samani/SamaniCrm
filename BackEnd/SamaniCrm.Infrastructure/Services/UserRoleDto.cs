using AutoMapper.Internal;
using Azure.Core;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.JsonWebTokens;
using SamaniCrm.Application.Role.Commands;
using SamaniCrm.Core;

namespace SamaniCrm.Infrastructure.Services;
public class UserRoleDto
{
    public Guid UserId { get; set; }
    public required string RoleName { get; set; }

}
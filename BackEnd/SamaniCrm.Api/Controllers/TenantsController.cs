using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Features.Tenants;
using SamaniCrm.Application.Features.Tenants.Commands;
using SamaniCrm.Application.Features.Tenants.Dtos;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Application.Features.Tenants.Queries;
using SamaniCrm.Application.User.Commands;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Domain.Interfaces;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers;

[ApiController]
// [Authorize(Roles = "SuperAdmin")]
public partial class TenantsController : ApiBaseController
{
    private readonly IMediator _mediator;
    private readonly ICurrentTenant _currentTenant;

    public TenantsController(IMediator mediator, ICurrentTenant currentTenant)
    {
        _mediator = mediator;
        _currentTenant = currentTenant;
    }

    /// <summary>
    /// Create a new tenant
    /// </summary>
    [HttpPost("CreateTenant")]
    [Permission(AppPermissions.TenantManagement_Create)]
    [ProducesResponseType(typeof(ApiResponse<CreateTenantResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantCommand command)
    {
        var result = await _mediator.Send(command);
        return ApiOk<CreateTenantResponse>(result);
    }

    /// <summary>
    /// Get tenant by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Permission(AppPermissions.TenantManagement)]
    [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTenantById(Guid id)
    {
        var result = await _mediator.Send(new GetTenantByIdQuery(id));
        return ApiOk(result);
    }

    /// <summary>
    /// Get tenant by slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTenantBySlug(string slug)
    {
        var result = await _mediator.Send(new GetTenantBySlugQuery(slug));
        return ApiOk(result);
    }

    /// <summary>
    /// Get all tenants with pagination
    /// </summary>
    [HttpPost("GetAllTenants")]
    [Permission(AppPermissions.TenantManagement_List)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<TenantListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTenants([FromBody] TenantListQuery query)
    {
        var result = await _mediator.Send(query);
        return ApiOk(result);
    }

    /// <summary>
    /// Update tenant
    /// </summary>
    [HttpPut("{id:guid}")]
    [Permission(AppPermissions.TenantManagement_Edit)]
    [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTenant(Guid id, [FromBody] UpdateTenantCommand command)
    {
        if (id != command.Id) return BadRequest();

        var result = await _mediator.Send(command);
        return ApiOk(result);
    }

    /// <summary>
    /// Suspend tenant
    /// </summary>
    [HttpPost("{id:guid}/suspend")]
    [Permission(AppPermissions.TenantManagement_ActiveDeActive)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SuspendTenant(Guid id)
    {
        await _mediator.Send(new SuspendTenantCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Activate suspended tenant
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [Permission(AppPermissions.TenantManagement_ActiveDeActive)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ActivateTenant(ActivateTenantCommand input)
    {
        await _mediator.Send(input);
        return NoContent();
    }

    /// <summary>
    /// Delete tenant (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Permission(AppPermissions.TenantManagement_Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteTenant(Guid id)
    {
        await _mediator.Send(new DeleteTenantCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Get tenant settings
    /// </summary>
    [HttpGet("{id:guid}/settings")]
    [ProducesResponseType(typeof(ApiResponse<TenantSettingsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTenantSettings(Guid id)
    {
        var result = await _mediator.Send(new GetTenantSettingsQuery(id));
        return ApiOk(result);
    }

    /// <summary>
    /// Update tenant settings
    /// </summary>
    [HttpPut("{id:guid}/settings")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateTenantSettings(Guid id, [FromBody] UpdateTenantSettingsCommand command)
    {
        if (id != command.TenantId) return BadRequest();
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Get tenant provisioning status
    /// </summary>
    [HttpGet("{id:guid}/provisioning-status")]
    [ProducesResponseType(typeof(ApiResponse<List<ProvisioningStatusDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProvisioningTenantStatus(Guid id)
    {
        var result = await _mediator.Send(new GetProvisioningStatusQuery(id));
        return ApiOk(result);
    }

    /// <summary>
    /// Retry failed provisioning
    /// </summary>
    [HttpPost("{id:guid}/retry-provisioning")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> RetryProvisioningTenant(Guid id)
    {
        await _mediator.Send(new RetryProvisioningCommand(id));
        return Accepted();
    }

    /// <summary>
    /// Get tenant database info
    /// </summary>
    [HttpGet("{id:guid}/database")]
    [ProducesResponseType(typeof(ApiResponse<TenantDatabaseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTenantDatabaseInfo(Guid id)
    {
        var result = await _mediator.Send(new GetTenantDatabaseQuery(id));
        return ApiOk(result);
    }

    /// <summary>
    /// Test tenant database connection
    /// </summary>
    [HttpPost("{id:guid}/test-connection")]
    [ProducesResponseType(typeof(ApiResponse<ConnectionTestResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> TestConnection(Guid id)
    {
        var result = await _mediator.Send(new TestTenantConnectionQuery(id));
        return ApiOk(result);
    }

    /// <summary>
    /// Get tenant usage statistics
    /// </summary>
    [HttpGet("{id:guid}/usage")]
    [ProducesResponseType(typeof(ApiResponse<TenantUsageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTenantUsage(Guid id)
    {
        var result = await _mediator.Send(new GetTenantUsageQuery(id));
        return ApiOk(result);
    }
}
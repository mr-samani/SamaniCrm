using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Features.Tenants;
using SamaniCrm.Application.Features.Tenants.Commands;
using SamaniCrm.Application.Features.Tenants.Dtos;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Application.Features.Tenants.Queries;
using SamaniCrm.Application.User.Commands;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin")]
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
    [HttpPost]
    [ProducesResponseType(typeof(CreateTenantResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTenantCommand command)
    {
        var result = await _mediator.Send(command);
        return ApiOk<CreateTenantResponse>(result);
    }

    /// <summary>
    /// Get tenant by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetTenantByIdQuery(id));
        return ApiOk(result);
    }

    /// <summary>
    /// Get tenant by slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await _mediator.Send(new GetTenantBySlugQuery(slug));
        return ApiOk(result);
    }

    /// <summary>
    /// Get all tenants with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<TenantListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] TenantListQuery query)
    {
        var result = await _mediator.Send(query);
        return ApiOk(result);
    }

    /// <summary>
    /// Update tenant
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTenantCommand command)
    {
        if (id != command.Id) return BadRequest();

        var result = await _mediator.Send(command);
        return ApiOk(result);
    }

    /// <summary>
    /// Suspend tenant
    /// </summary>
    [HttpPost("{id:guid}/suspend")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Suspend(Guid id)
    {
        await _mediator.Send(new SuspendTenantCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Activate suspended tenant
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Activate(ActivateTenantCommand input)
    {
        await _mediator.Send(input);
        return NoContent();
    }

    /// <summary>
    /// Delete tenant (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteTenantCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Get tenant settings
    /// </summary>
    [HttpGet("{id:guid}/settings")]
    [ProducesResponseType(typeof(TenantSettingsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSettings(Guid id)
    {
        var result = await _mediator.Send(new GetTenantSettingsQuery(id));
        return ApiOk(result);
    }

    /// <summary>
    /// Update tenant settings
    /// </summary>
    [HttpPut("{id:guid}/settings")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateSettings(Guid id, [FromBody] UpdateTenantSettingsCommand command)
    {
        if (id != command.TenantId) return BadRequest();
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Get tenant provisioning status
    /// </summary>
    [HttpGet("{id:guid}/provisioning-status")]
    [ProducesResponseType(typeof(ProvisioningStatusDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProvisioningStatus(Guid id)
    {
        var result = await _mediator.Send(new GetProvisioningStatusQuery(id));
        return ApiOk(result);
    }

    /// <summary>
    /// Retry failed provisioning
    /// </summary>
    [HttpPost("{id:guid}/retry-provisioning")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> RetryProvisioning(Guid id)
    {
        await _mediator.Send(new RetryProvisioningCommand(id));
        return Accepted();
    }

    /// <summary>
    /// Get tenant database info
    /// </summary>
    [HttpGet("{id:guid}/database")]
    [ProducesResponseType(typeof(TenantDatabaseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDatabaseInfo(Guid id)
    {
        var result = await _mediator.Send(new GetTenantDatabaseQuery(id));
        return ApiOk(result);
    }

    /// <summary>
    /// Test tenant database connection
    /// </summary>
    [HttpPost("{id:guid}/test-connection")]
    [ProducesResponseType(typeof(ConnectionTestResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> TestConnection(Guid id)
    {
        var result = await _mediator.Send(new TestTenantConnectionQuery(id));
        return ApiOk(result);
    }

    /// <summary>
    /// Get tenant usage statistics
    /// </summary>
    [HttpGet("{id:guid}/usage")]
    [ProducesResponseType(typeof(TenantUsageDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsage(Guid id)
    {
        var result = await _mediator.Send(new GetTenantUsageQuery(id));
        return ApiOk(result);
    }
}
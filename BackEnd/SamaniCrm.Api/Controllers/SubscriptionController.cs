using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.SubscriptionManager.Commands;
using SamaniCrm.Application.SubscriptionManager.Queries;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Core.Shared.Subscriptions;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers;

public class SubscriptionController : ApiBaseController
{
    private readonly IMediator _mediator;

    public SubscriptionController(IMediator mediator)
    {
        _mediator = mediator;
    }



    [HttpGet("GetAllPlans")]
    [Permission(AppPermissions.SubscriptionManagement.Plans.List)]
    [ProducesResponseType(typeof(ApiResponse<List<PlanDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPlans()
    {
        List<PlanDto> result = await _mediator.Send(new GetAllPlansQuery());
        return ApiOk(result);
    }


    [HttpPost("CreateOrEditPlan")]
    [Permission(AppPermissions.SubscriptionManagement.Plans.Create)]
    [Permission(AppPermissions.SubscriptionManagement.Plans.Edit)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOrEditPlan(CreateOrEditPlanCommand input)
    {
        bool result = await _mediator.Send(input);
        return ApiOk(result);
    }


    [HttpGet("GetPlanForEdit")]
    [Permission(AppPermissions.SubscriptionManagement.Plans.Edit)]
    [ProducesResponseType(typeof(ApiResponse<PlanDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlanForEdit(Guid id)
    {
        PlanDto result = await _mediator.Send(new GetForEditPlanQuery(id));
        return ApiOk(result);
    }


    [HttpPost("DeletePlan")]
    [Permission(AppPermissions.SubscriptionManagement.Plans.Create)]
    [Permission(AppPermissions.SubscriptionManagement.Plans.Edit)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeletePlan(DeletePlanCommand input)
    {
        bool result = await _mediator.Send(input);
        return ApiOk(result);
    }

}

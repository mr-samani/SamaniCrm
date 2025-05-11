using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Localize.Commands;
using SamaniCrm.Application.Localize.Queries;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers
{

    [Authorize]
    public class LanguageController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public LanguageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Language

        [HttpGet("GetAllLanguages")]
        [Permission(AppPermissions.LanguageManagement_List)]
        [ProducesResponseType(typeof(ApiResponse<List<LanguageDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllLanguages()
        {
            var result = await _mediator.Send(new GetAllLanguageQuery());
            return ApiOk<List<LanguageDTO>>(result);
        }


      


        [HttpPost("CreateOrUpdate")]
        [Permission(AppPermissions.LanguageManagement_Create)]
        [Permission(AppPermissions.LanguageManagement_Edit)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrUpdate(CreateOrEditLanguageCommand input)
        {
            var result = await _mediator.Send(input);
            return ApiOk(result);
        }


        [HttpPost("DeleteLangauuge")]
        [Permission(AppPermissions.LanguageManagement_Delete)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteLangauuge(DeleteLanguageCommand input)
        {
            var result = await _mediator.Send(input);
            return ApiOk(result);
        }

        [HttpPost("ActiveOrDeactive")]
        [Permission(AppPermissions.LanguageManagement_Edit)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ActiveOrDeactive(ActiveOrDeactiveLanguageCommand input)
        {
            var result = await _mediator.Send(input);
            return ApiOk(result);
        }

        #endregion

        #region Localization keys
        [HttpGet("GetAllLanguageKeys")]
        [Permission(AppPermissions.LanguageManagement_List)]
        [ProducesResponseType(typeof(ApiResponse<List<LocalizationKeyDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllLanguageKeys(string culture)
        {
           var result = await _mediator.Send(new GetAllLanguageKeys(culture));
            return ApiOk<List<LocalizationKeyDTO>>(result);
        }




        /// <summary>
        /// افزودن یک کلید به کلیه زبان ها
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        [HttpPost("CreateOrEditLocalizeKey")]
        [Permission(AppPermissions.LanguageManagement_Create)]
        [Permission(AppPermissions.LanguageManagement_Edit)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrEditLocalizeKey(CreateOrEditLocalizeKeyCommand input)
        {
            var result = await _mediator.Send(input);
            return ApiOk(result);
        }

        [HttpPost("DeleteKey")]
        [Permission(AppPermissions.LanguageManagement_Delete)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteKey(DeleteLocalizeKeyCommand input)
        {
            var result = await _mediator.Send(input);
            return ApiOk(result);
        }


        [HttpPost("UpdateBatchLocalizeKey")]
        [Permission(AppPermissions.LanguageManagement_Create)]
        [Permission(AppPermissions.LanguageManagement_Edit)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateBatchLocalizeKey(UpdateBatchLocalizeKeyCommand input)
        {
            var result = await _mediator.Send(input);
            return ApiOk(result);
        }

        #endregion

    }
}

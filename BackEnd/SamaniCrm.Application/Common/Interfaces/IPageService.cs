﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.DTOs.PageBuilder;
using SamaniCrm.Application.Pages.Commands;
using SamaniCrm.Application.Pages.Queries;

namespace SamaniCrm.Application.Common.Interfaces
{
    public interface IPageService
    {
        Task<PaginatedResult<PageDto>> GetPagedList(GetFilteredPagesQuery request, CancellationToken cancellationToken);
        Task<PageForEditDto> GetPageForEdit(GetPageForEditMetaDataQuery request, CancellationToken cancellationToken);
        Task<PageDto> GetPageInfo(GetPageInfoQuery request, CancellationToken cancellationToken);
        Task<Guid> CreateOrEditMetaDataPage(CreateOrEditPageMetaDataCommand request, CancellationToken cancellationToken);

        Task<Unit> DeletePage(DeletePageCommand request, CancellationToken cancellationToken);
        Task<Unit> UpdatePageContent(UpdatePageContentCommand request, CancellationToken cancellationToken);




        // Blocks
        Task<Guid> CreateCustomBlock(CreateCustomBlockCommand request, CancellationToken cancellationToken);
        Task<Unit> DeleteCustomBlockPage(DeleteCustomBlockCommand request, CancellationToken cancellationToken);
        Task<List<CustomBlockDto>> GetCustomBlocks(GetCustomBlockQuery request, CancellationToken cancellationToken);

    }
}

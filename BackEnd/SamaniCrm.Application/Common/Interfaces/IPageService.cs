using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Pages.Commands;
using SamaniCrm.Application.Pages.Queries;

namespace SamaniCrm.Application.Common.Interfaces
{
    public interface IPageService
    {
        Task<PaginatedResult<PageDto>> GetPagedList(GetFilteredPagesQuery request, CancellationToken cancellationToken);
        Task<PageForEditDto> GetPageForEdit(GetPageForEditMetaDataQuery request, CancellationToken cancellationToken);

        Task<Guid> CreatePage(CreateOrEditPageMetaDataCommand request, CancellationToken cancellationToken);

        Task<Unit> DeletePage(DeletePageCommand request, CancellationToken cancellationToken);
        Task<Unit> UpdatePageContent(UpdatePageContentCommand request, CancellationToken cancellationToken);
    }
}

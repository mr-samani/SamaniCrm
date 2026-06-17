using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.DTOs.PageBuilder;
using SamaniCrm.Application.DynamicData.Queries;


namespace SamaniCrm.Application.Common.Interfaces;

public interface IDynamicDataService
{
    List<DynamicDataListDto> GetDynamicDataList(CancellationToken cancellation);
    Task<PaginatedResult<T>> GetDynamicDataAsync<T>(GetDynamicDataQuery request, CancellationToken cancellation) where T : class;


}



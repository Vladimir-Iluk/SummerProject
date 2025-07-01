using BLL.DTO.ResponseDto;
using BLL.DTO.CommonDto;
using BLL.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IResponseService
    {
        Task<IEnumerable<ResponseResponseDto>> GetAllAsync(string? searchTerm = null, CancellationToken cancellationToken = default);
        Task<PagedList<ResponseResponseDto>> GetPagedAsync(SearchParametersDto searchParams, CancellationToken cancellationToken = default);
        Task<ResponseResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ResponseResponseDto> CreateAsync(ResponseCreateDto dto, CancellationToken cancellationToken = default);
        Task<ResponseResponseDto?> UpdateAsync(Guid id, ResponseUpdateDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

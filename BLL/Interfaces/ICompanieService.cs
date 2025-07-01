using BLL.DTO.CompanieDto;
using BLL.DTO.CommonDto;
using BLL.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ICompanieService
    {
        Task<IEnumerable<CompanieResponseDto>> GetAllAsync(string? searchTerm = null, string? sortBy = null, string? sortDirection = null, CancellationToken cancellationToken = default);
        Task<PagedList<CompanieResponseDto>> GetPagedAsync(SearchParametersDto searchParams, CancellationToken cancellationToken = default);
        Task<CompanieResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<CompanieResponseDto> CreateAsync(CompanieCreateDto dto, CancellationToken cancellationToken = default);
        Task<CompanieResponseDto?> UpdateAsync(Guid id, CompanieUpdateDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

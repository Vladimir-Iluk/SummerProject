using BLL.DTO.AgreementDto;
using BLL.DTO.CommonDto;
using BLL.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAgreementService
    {
        Task<IEnumerable<AgreementResponseDto>> GetAllAsync(string? searchTerm = null, string? sortBy = null, string? sortDirection = null, CancellationToken cancellationToken = default);
        Task<PagedList<AgreementResponseDto>> GetPagedAsync(SearchParametersDto searchParams, CancellationToken cancellationToken = default);
        Task<AgreementResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<AgreementResponseDto> CreateAsync(AgreementCreateDto dto, CancellationToken cancellationToken = default);
        Task<AgreementResponseDto?> UpdateAsync(Guid id, AgreementUpdateDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

using BLL.DTO.AgreementDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAgreementService
    {
        Task<IEnumerable<AgreementResponseDto>> GetAllAsync(string? sortBy = null, string? sortDirection = null, CancellationToken cancellationToken = default);
        Task<AgreementResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<AgreementResponseDto> CreateAsync(AgreementCreateDto dto, CancellationToken cancellationToken = default);
        Task<AgreementResponseDto?> UpdateAsync(Guid id, AgreementUpdateDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

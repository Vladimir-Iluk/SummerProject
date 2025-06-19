using BLL.DTO.WorkerDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IWorkerService
    {
        Task<IEnumerable<WorkerResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<WorkerResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<WorkerResponseDto> CreateAsync(WorkerCreateDto dto, CancellationToken cancellationToken = default);
        Task<WorkerResponseDto?> UpdateAsync(Guid id, WorkerUpdateDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

using BLL.DTO.ResponseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IResponseService
    {
        Task<IEnumerable<ResponseResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ResponseResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ResponseResponseDto> CreateAsync(ResponseCreateDto dto, CancellationToken cancellationToken = default);
        Task<ResponseResponseDto?> UpdateAsync(Guid id, ResponseUpdateDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

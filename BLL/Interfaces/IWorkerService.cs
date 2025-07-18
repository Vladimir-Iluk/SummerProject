﻿using BLL.DTO.WorkerDto;
using BLL.DTO.CommonDto;
using BLL.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IWorkerService
    {
        Task<IEnumerable<WorkerResponseDto>> GetAllAsync(string? searchTerm = null, string? sortBy = null, string? sortDirection = null, CancellationToken cancellationToken = default);
        Task<PagedList<WorkerResponseDto>> GetPagedAsync(SearchParametersDto searchParams, CancellationToken cancellationToken = default);
        Task<WorkerResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<WorkerResponseDto> CreateAsync(WorkerCreateDto dto, CancellationToken cancellationToken = default);
        Task<WorkerResponseDto?> UpdateAsync(Guid id, WorkerUpdateDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

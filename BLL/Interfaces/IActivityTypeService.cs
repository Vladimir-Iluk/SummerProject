﻿using BLL.DTO.ActivityTypeDto;
using BLL.DTO.CommonDto;
using BLL.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IActivityTypeService
    {
        Task<IEnumerable<ActivityTypeResponseDto>> GetAllAsync(string? searchTerm = null, string? sortBy = null, string? sortDirection = null, CancellationToken cancellationToken = default);
        Task<PagedList<ActivityTypeResponseDto>> GetPagedAsync(SearchParametersDto searchParams, CancellationToken cancellationToken = default);
        Task<ActivityTypeResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ActivityTypeResponseDto> CreateAsync(ActivityTypeCreateDto dto, CancellationToken cancellationToken = default);
        Task<ActivityTypeResponseDto?> UpdateAsync(Guid id, ActivityTypeUpdateDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

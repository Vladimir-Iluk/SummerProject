﻿using BLL.DTO.VacancyDto;
using BLL.DTO.CommonDto;
using BLL.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IVacancyService
    {
        Task<IEnumerable<VacancyResponseDto>> GetAllAsync(string? searchTerm = null, CancellationToken cancellationToken = default);
        Task<PagedList<VacancyResponseDto>> GetPagedAsync(SearchParametersDto searchParams, CancellationToken cancellationToken = default);
        Task<VacancyResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<VacancyResponseDto> CreateAsync(VacancyCreateDto dto, CancellationToken cancellationToken = default);
        Task<VacancyResponseDto?> UpdateAsync(Guid id, VacancyUpdateDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

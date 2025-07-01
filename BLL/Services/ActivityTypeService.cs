using AutoMapper;
using BLL.DTO.ActivityTypeDto;
using BLL.DTO.CommonDto;
using BLL.Interfaces;
using BLL.Pagination;
using DAL.EF.Entities;
using DAL.EF.UoW;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ActivityTypeService : IActivityTypeService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public ActivityTypeService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _uow = unitOfWork;
        }

        public async Task<IEnumerable<ActivityTypeResponseDto>> GetAllAsync(string? searchTerm = null, string? sortBy = null, string? sortDirection = null, CancellationToken cancellationToken = default)
        {
            var query = _uow.ActivityTypes.GetAll();
            query = query.Include(at => at.Workers).Include(at => at.Companies);

            // Застосовуємо пошук
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchTermLower = searchTerm.ToLower();
                query = query.Where(at => 
                    at.ActivityName.ToLower().Contains(searchTermLower));
            }

            var list = await query.ToListAsync(cancellationToken);
            var dtos = _mapper.Map<IEnumerable<ActivityTypeResponseDto>>(list);

            if (!string.IsNullOrEmpty(sortBy))
            {
                bool desc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
                dtos = sortBy.ToLower() switch
                {
                    "activityname" => desc ? dtos.OrderByDescending(x => x.ActivityName) : dtos.OrderBy(x => x.ActivityName),
                    _ => dtos
                };
            }
            return dtos;
        }

        public async Task<PagedList<ActivityTypeResponseDto>> GetPagedAsync(SearchParametersDto searchParams, CancellationToken cancellationToken = default)
        {
            var query = _uow.ActivityTypes.GetAll();
            query = query.Include(at => at.Workers).Include(at => at.Companies);

            // Застосовуємо пошук
            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.ToLower();
                query = query.Where(at => 
                    at.ActivityName.ToLower().Contains(searchTerm));
            }

            // Застосовуємо сортування
            IQueryable<ActivityType> orderedQuery;
            if (!string.IsNullOrEmpty(searchParams.SortBy))
            {
                bool desc = string.Equals(searchParams.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
                orderedQuery = searchParams.SortBy.ToLower() switch
                {
                    "activityname" => desc ? query.OrderByDescending(x => x.ActivityName) : query.OrderBy(x => x.ActivityName),
                    _ => query.OrderBy(x => x.ActivityName)
                };
            }
            else
            {
                orderedQuery = query.OrderBy(x => x.ActivityName);
            }

            // Виконуємо пагінацію на рівні Entity
            var pagedEntities = await PagedList<ActivityType>.ToPagedListAsync(
                orderedQuery, 
                searchParams.PageNumber, 
                searchParams.PageSize, 
                cancellationToken);

            // Мапимо результат
            var dtos = _mapper.Map<List<ActivityTypeResponseDto>>(pagedEntities.Items);
            
            return new PagedList<ActivityTypeResponseDto>(
                dtos, 
                pagedEntities.TotalCount, 
                pagedEntities.CurrentPage, 
                pagedEntities.PageSize);
        }

        public async Task<ActivityTypeResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.ActivityTypes.GetByIdAsync(
                id,
                include: query => query.Include(at => at.Workers).Include(at => at.Companies),
                cancellationToken: cancellationToken);
            return entity == null ? null : _mapper.Map<ActivityTypeResponseDto>(entity);
        }

        public async Task<ActivityTypeResponseDto> CreateAsync(ActivityTypeCreateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<ActivityType>(dto);
            await _uow.ActivityTypes.AddAsync(entity, cancellationToken);
            await _uow.SaveAsync(cancellationToken);
            return _mapper.Map<ActivityTypeResponseDto>(entity);
        }

        public async Task<ActivityTypeResponseDto?> UpdateAsync(Guid id, ActivityTypeUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.ActivityTypes.GetByIdAsync(
                id,
                include: query => query.Include(at => at.Workers).Include(at => at.Companies),
                cancellationToken: cancellationToken);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            _uow.ActivityTypes.Update(entity);
            await _uow.SaveAsync(cancellationToken);
            return _mapper.Map<ActivityTypeResponseDto>(entity);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.ActivityTypes.GetByIdAsync(
                id,
                cancellationToken: cancellationToken);
            if (entity == null) return false;

            _uow.ActivityTypes.Delete(entity); 
            await _uow.SaveAsync(cancellationToken);
            return true;
        }
    }
}
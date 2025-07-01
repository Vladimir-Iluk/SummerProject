using AutoMapper;
using BLL.DTO.VacancyDto;
using BLL.DTO.CommonDto;
using BLL.Interfaces;
using BLL.Pagination;
using DAL.EF.Entities;
using DAL.EF.UoW;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class VacancyService : IVacancyService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public VacancyService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _uow = unitOfWork;
        }

        public async Task<IEnumerable<VacancyResponseDto>> GetAllAsync(string? searchTerm = null, CancellationToken cancellationToken = default)
        {
            var query = _uow.Vacancy.GetAll();
            query = query.Include(v => v.Companie);

            // Застосовуємо пошук
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchTermLower = searchTerm.ToLower();
                query = query.Where(v => 
                    v.Position.ToLower().Contains(searchTermLower) ||
                    v.Description.ToLower().Contains(searchTermLower) ||
                    v.Salary.ToString().Contains(searchTerm) ||
                    v.Companie.CompanyName.ToLower().Contains(searchTermLower));
            }

            var vacancies = await query.ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<VacancyResponseDto>>(vacancies);
        }

        public async Task<PagedList<VacancyResponseDto>> GetPagedAsync(SearchParametersDto searchParams, CancellationToken cancellationToken = default)
        {
            var query = _uow.Vacancy.GetAll();
            query = query.Include(v => v.Companie);

            // Застосовуємо пошук
            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.ToLower();
                query = query.Where(v => 
                    v.Position.ToLower().Contains(searchTerm) ||
                    v.Description.ToLower().Contains(searchTerm) ||
                    v.Salary.ToString().Contains(searchTerm) ||
                    v.Companie.CompanyName.ToLower().Contains(searchTerm));
            }

            // Застосовуємо сортування
            IQueryable<Vacancy> orderedQuery;
            if (!string.IsNullOrEmpty(searchParams.SortBy))
            {
                bool desc = string.Equals(searchParams.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
                orderedQuery = searchParams.SortBy.ToLower() switch
                {
                    "position" => desc ? query.OrderByDescending(x => x.Position) : query.OrderBy(x => x.Position),
                    "salary" => desc ? query.OrderByDescending(x => x.Salary) : query.OrderBy(x => x.Salary),
                    "createdat" => desc ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
                    "isopen" => desc ? query.OrderByDescending(x => x.IsOpen) : query.OrderBy(x => x.IsOpen),
                    "companyname" => desc ? query.OrderByDescending(x => x.Companie.CompanyName) : query.OrderBy(x => x.Companie.CompanyName),
                    _ => query.OrderBy(x => x.CreatedAt)
                };
            }
            else
            {
                orderedQuery = query.OrderBy(x => x.CreatedAt);
            }

            // Виконуємо пагінацію на рівні Entity
            var pagedEntities = await PagedList<Vacancy>.ToPagedListAsync(
                orderedQuery, 
                searchParams.PageNumber, 
                searchParams.PageSize, 
                cancellationToken);

            // Мапимо результат
            var dtos = _mapper.Map<List<VacancyResponseDto>>(pagedEntities.Items);
            
            return new PagedList<VacancyResponseDto>(
                dtos, 
                pagedEntities.TotalCount, 
                pagedEntities.CurrentPage, 
                pagedEntities.PageSize);
        }

        public async Task<VacancyResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var vacancy = await _uow.Vacancy.GetByIdAsync(
                id,
                include: query => query.Include(v => v.Companie),
                cancellationToken: cancellationToken);
            if (vacancy == null) return null;

            var dto = _mapper.Map<VacancyResponseDto>(vacancy);
            dto.CompanyName = vacancy.Companie.CompanyName;
            return dto;
        }

        public async Task<VacancyResponseDto> CreateAsync(VacancyCreateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<Vacancy>(dto);
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsOpen = true;

            await _uow.Vacancy.AddAsync(entity, cancellationToken);
            await _uow.SaveAsync(cancellationToken);

            return await GetByIdAsync(entity.Id, cancellationToken);
        }

        public async Task<VacancyResponseDto?> UpdateAsync(Guid id, VacancyUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.Vacancy.GetByIdAsync(
                id,
                include: query => query.Include(v => v.Companie),
                cancellationToken: cancellationToken);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            _uow.Vacancy.Update(entity);
            await _uow.SaveAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.Vacancy.GetByIdAsync(
                id,
                cancellationToken: cancellationToken);
            if (entity == null) return false;

            _uow.Vacancy.Delete(entity);
            await _uow.SaveAsync(cancellationToken);
            return true;
        }
    }
}

using AutoMapper;
using BLL.DTO.CompanieDto;
using BLL.DTO.CommonDto;
using BLL.Interfaces;
using BLL.Pagination;
using DAL.EF.Entities;
using DAL.EF.UoW;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CompanieService : ICompanieService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public CompanieService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _uow = unitOfWork;
        }

        public async Task<IEnumerable<CompanieResponseDto>> GetAllAsync(string? searchTerm = null, string? sortBy = null, string? sortDirection = null, CancellationToken cancellationToken = default)
        {
            var query = _uow.Companies.GetAll();
            query = query.Include(c => c.ActivityType);

            // Застосовуємо пошук
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchTermLower = searchTerm.ToLower();
                query = query.Where(c => 
                    c.CompanyName.ToLower().Contains(searchTermLower) ||
                    c.EmailCompany.ToLower().Contains(searchTermLower) ||
                    c.Address.ToLower().Contains(searchTermLower) ||
                    c.Phone.ToLower().Contains(searchTermLower) ||
                    c.ActivityType.ActivityName.ToLower().Contains(searchTermLower));
            }

            var companies = await query.ToListAsync(cancellationToken);
            var dtos = _mapper.Map<IEnumerable<CompanieResponseDto>>(companies);

            if (!string.IsNullOrEmpty(sortBy))
            {
                bool desc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
                dtos = sortBy.ToLower() switch
                {
                    "companyname" => desc ? dtos.OrderByDescending(x => x.CompanyName) : dtos.OrderBy(x => x.CompanyName),
                    "emailcompany" => desc ? dtos.OrderByDescending(x => x.EmailCompany) : dtos.OrderBy(x => x.EmailCompany),
                    "address" => desc ? dtos.OrderByDescending(x => x.Address) : dtos.OrderBy(x => x.Address),
                    "phone" => desc ? dtos.OrderByDescending(x => x.Phone) : dtos.OrderBy(x => x.Phone),
                    "activitytypename" => desc ? dtos.OrderByDescending(x => x.ActivityTypeName) : dtos.OrderBy(x => x.ActivityTypeName),
                    _ => dtos
                };
            }
            return dtos;
        }

        public async Task<PagedList<CompanieResponseDto>> GetPagedAsync(SearchParametersDto searchParams, CancellationToken cancellationToken = default)
        {
            var query = _uow.Companies.GetAll();
            query = query.Include(c => c.ActivityType);

            // Застосовуємо пошук
            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.ToLower();
                query = query.Where(c => 
                    c.CompanyName.ToLower().Contains(searchTerm) ||
                    c.EmailCompany.ToLower().Contains(searchTerm) ||
                    c.Address.ToLower().Contains(searchTerm) ||
                    c.Phone.ToLower().Contains(searchTerm) ||
                    c.ActivityType.ActivityName.ToLower().Contains(searchTerm));
            }

            // Застосовуємо сортування
            IQueryable<Companie> orderedQuery;
            if (!string.IsNullOrEmpty(searchParams.SortBy))
            {
                bool desc = string.Equals(searchParams.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
                orderedQuery = searchParams.SortBy.ToLower() switch
                {
                    "companyname" => desc ? query.OrderByDescending(x => x.CompanyName) : query.OrderBy(x => x.CompanyName),
                    "emailcompany" => desc ? query.OrderByDescending(x => x.EmailCompany) : query.OrderBy(x => x.EmailCompany),
                    "address" => desc ? query.OrderByDescending(x => x.Address) : query.OrderBy(x => x.Address),
                    "phone" => desc ? query.OrderByDescending(x => x.Phone) : query.OrderBy(x => x.Phone),
                    "activitytypename" => desc ? query.OrderByDescending(x => x.ActivityType.ActivityName) : query.OrderBy(x => x.ActivityType.ActivityName),
                    _ => query.OrderBy(x => x.CompanyName)
                };
            }
            else
            {
                orderedQuery = query.OrderBy(x => x.CompanyName);
            }

            // Виконуємо пагінацію на рівні Entity
            var pagedEntities = await PagedList<Companie>.ToPagedListAsync(
                orderedQuery, 
                searchParams.PageNumber, 
                searchParams.PageSize, 
                cancellationToken);

            // Мапимо результат
            var dtos = _mapper.Map<List<CompanieResponseDto>>(pagedEntities.Items);
            
            return new PagedList<CompanieResponseDto>(
                dtos, 
                pagedEntities.TotalCount, 
                pagedEntities.CurrentPage, 
                pagedEntities.PageSize);
        }

        public async Task<CompanieResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var company = await _uow.Companies.GetByIdAsync(
                id,
                include: query => query.Include(c => c.ActivityType),
                cancellationToken: cancellationToken);
            if (company == null) return null;

            var dto = _mapper.Map<CompanieResponseDto>(company);
            dto.ActivityTypeName = company.ActivityType.ActivityName;
            return dto;
        }

        public async Task<CompanieResponseDto> CreateAsync(CompanieCreateDto dto, CancellationToken cancellationToken = default)
        {
            // Перевіряємо, чи існує вказаний тип активності
            var activityType = await _uow.ActivityTypes.GetByIdAsync(
                dto.ActivityTypeId,
                cancellationToken: cancellationToken);
            if (activityType == null)
            {
                throw new ValidationException($"ActivityType з ID {dto.ActivityTypeId} не знайдено");
            }

            var entity = _mapper.Map<Companie>(dto);
            await _uow.Companies.AddAsync(entity, cancellationToken);
            await _uow.SaveAsync(cancellationToken);

            var createdCompany = await _uow.Companies.GetByIdAsync(
                entity.Id,
                include: query => query.Include(c => c.ActivityType),
                cancellationToken: cancellationToken);
            return _mapper.Map<CompanieResponseDto>(createdCompany);
        }

        public async Task<CompanieResponseDto?> UpdateAsync(Guid id, CompanieUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.Companies.GetByIdAsync(
                id,
                include: query => query.Include(c => c.ActivityType),
                cancellationToken: cancellationToken);
            if (entity == null) return null;

            // Перевіряємо, чи існує вказаний тип активності
            if (dto.ActivityTypeId != entity.ActivityTypeId)
            {
                var activityType = await _uow.ActivityTypes.GetByIdAsync(
                    dto.ActivityTypeId,
                    cancellationToken: cancellationToken);
                if (activityType == null)
                {
                    throw new ValidationException($"ActivityType з ID {dto.ActivityTypeId} не знайдено");
                }
            }

            _mapper.Map(dto, entity);
            _uow.Companies.Update(entity);
            await _uow.SaveAsync(cancellationToken);

            var updatedCompany = await _uow.Companies.GetByIdAsync(
                id,
                include: query => query.Include(c => c.ActivityType),
                cancellationToken: cancellationToken);
            return _mapper.Map<CompanieResponseDto>(updatedCompany);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.Companies.GetByIdAsync(
                id,
                cancellationToken: cancellationToken);
            if (entity == null) return false;

            _uow.Companies.Delete(entity);
            await _uow.SaveAsync(cancellationToken);
            return true;
        }
    }
}

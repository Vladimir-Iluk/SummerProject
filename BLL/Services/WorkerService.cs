using AutoMapper;
using BLL.DTO.WorkerDto;
using BLL.DTO.CommonDto;
using BLL.Interfaces;
using BLL.Pagination;
using DAL.EF.Entities;
using DAL.EF.UoW;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class WorkerService : IWorkerService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public WorkerService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _uow = unitOfWork;
        }

        public async Task<IEnumerable<WorkerResponseDto>> GetAllAsync(string? searchTerm = null, string? sortBy = null, string? sortDirection = null, CancellationToken cancellationToken = default)
        {
            var query = _uow.Workers.GetAll();
            query = query.Include(w => w.ActivityType);

            // Застосовуємо пошук
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchTermLower = searchTerm.ToLower();
                query = query.Where(w => 
                    w.FirstName.ToLower().Contains(searchTermLower) ||
                    w.LastName.ToLower().Contains(searchTermLower) ||
                    w.Email.ToLower().Contains(searchTermLower) ||
                    w.Qualification.ToLower().Contains(searchTermLower) ||
                    w.ActivityType.ActivityName.ToLower().Contains(searchTermLower));
            }

            var workers = await query.ToListAsync(cancellationToken);
            var dtos = _mapper.Map<IEnumerable<WorkerResponseDto>>(workers);

            if (!string.IsNullOrEmpty(sortBy))
            {
                bool desc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
                dtos = sortBy.ToLower() switch
                {
                    "firstname" => desc ? dtos.OrderByDescending(x => x.FirstName) : dtos.OrderBy(x => x.FirstName),
                    "lastname" => desc ? dtos.OrderByDescending(x => x.LastName) : dtos.OrderBy(x => x.LastName),
                    "email" => desc ? dtos.OrderByDescending(x => x.Email) : dtos.OrderBy(x => x.Email),
                    "qualification" => desc ? dtos.OrderByDescending(x => x.Qualification) : dtos.OrderBy(x => x.Qualification),
                    "expectedsalary" => desc ? dtos.OrderByDescending(x => x.ExpectedSalary) : dtos.OrderBy(x => x.ExpectedSalary),
                    "activitytypename" => desc ? dtos.OrderByDescending(x => x.ActivityTypeName) : dtos.OrderBy(x => x.ActivityTypeName),
                    _ => dtos
                };
            }
            return dtos;
        }

        public async Task<PagedList<WorkerResponseDto>> GetPagedAsync(SearchParametersDto searchParams, CancellationToken cancellationToken = default)
        {
            var query = _uow.Workers.GetAll();
            query = query.Include(w => w.ActivityType);

            // Застосовуємо пошук
            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.ToLower();
                query = query.Where(w => 
                    w.LastName.ToLower().Contains(searchTerm) ||
                    w.FirstName.ToLower().Contains(searchTerm) ||
                    w.Qualification.ToLower().Contains(searchTerm) ||
                    w.Email.ToLower().Contains(searchTerm) ||
                    w.ExpectedSalary.ToString().Contains(searchTerm) ||
                    w.ActivityType.ActivityName.ToLower().Contains(searchTerm));
            }

            // Застосовуємо сортування
            IQueryable<Worker> orderedQuery;
            if (!string.IsNullOrEmpty(searchParams.SortBy))
            {
                bool desc = string.Equals(searchParams.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
                orderedQuery = searchParams.SortBy.ToLower() switch
                {
                    "lastname" => desc ? query.OrderByDescending(x => x.LastName) : query.OrderBy(x => x.LastName),
                    "firstname" => desc ? query.OrderByDescending(x => x.FirstName) : query.OrderBy(x => x.FirstName),
                    "qualification" => desc ? query.OrderByDescending(x => x.Qualification) : query.OrderBy(x => x.Qualification),
                    "email" => desc ? query.OrderByDescending(x => x.Email) : query.OrderBy(x => x.Email),
                    "expectedsalary" => desc ? query.OrderByDescending(x => x.ExpectedSalary) : query.OrderBy(x => x.ExpectedSalary),
                    "activitytypename" => desc ? query.OrderByDescending(x => x.ActivityType.ActivityName) : query.OrderBy(x => x.ActivityType.ActivityName),
                    _ => query.OrderBy(x => x.LastName)
                };
            }
            else
            {
                orderedQuery = query.OrderBy(x => x.LastName);
            }

            // Виконуємо пагінацію на рівні Entity
            var pagedEntities = await PagedList<Worker>.ToPagedListAsync(
                orderedQuery, 
                searchParams.PageNumber, 
                searchParams.PageSize, 
                cancellationToken);

            // Мапимо результат
            var dtos = _mapper.Map<List<WorkerResponseDto>>(pagedEntities.Items);
            
            return new PagedList<WorkerResponseDto>(
                dtos, 
                pagedEntities.TotalCount, 
                pagedEntities.CurrentPage, 
                pagedEntities.PageSize);
        }

        public async Task<WorkerResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var worker = await _uow.Workers.GetByIdAsync(
                id,
                include: query => query.Include(w => w.ActivityType),
                cancellationToken: cancellationToken);
            if (worker == null)
                return null;

            return _mapper.Map<WorkerResponseDto>(worker);
        }

        public async Task<WorkerResponseDto> CreateAsync(WorkerCreateDto dto, CancellationToken cancellationToken = default)
        {
            // Перевіряємо, чи існує вказаний тип активності
            var activityType = await _uow.ActivityTypes.GetByIdAsync(
                dto.ActivityTypeId,
                cancellationToken: cancellationToken);
            if (activityType == null)
                throw new KeyNotFoundException($"ActivityType with ID {dto.ActivityTypeId} not found");

            var entity = _mapper.Map<Worker>(dto);
            
            await _uow.Workers.AddAsync(entity, cancellationToken);
            await _uow.SaveAsync(cancellationToken);

            // Отримуємо створеного працівника з бази даних
            var createdWorker = await _uow.Workers.GetByIdAsync(
                entity.Id,
                include: query => query.Include(w => w.ActivityType),
                cancellationToken: cancellationToken);
            if (createdWorker == null)
                throw new Exception("Worker was not created properly");

            return _mapper.Map<WorkerResponseDto>(createdWorker);
        }

        public async Task<WorkerResponseDto?> UpdateAsync(Guid id, WorkerUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.Workers.GetByIdAsync(
                id,
                include: query => query.Include(w => w.ActivityType),
                cancellationToken: cancellationToken);
            if (entity == null)
                return null;

            // Перевіряємо, чи існує вказаний тип активності
            if (dto.ActivityTypeId != Guid.Empty)
            {
                var activityType = await _uow.ActivityTypes.GetByIdAsync(
                    dto.ActivityTypeId,
                    cancellationToken: cancellationToken);
                if (activityType == null)
                    throw new KeyNotFoundException($"ActivityType with ID {dto.ActivityTypeId} not found");
            }

            _mapper.Map(dto, entity);
            _uow.Workers.Update(entity);
            await _uow.SaveAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.Workers.GetByIdAsync(
                id,
                cancellationToken: cancellationToken);
            if (entity == null)
                return false;

            _uow.Workers.Delete(entity);
            await _uow.SaveAsync(cancellationToken);
            return true;
        }
    }
}

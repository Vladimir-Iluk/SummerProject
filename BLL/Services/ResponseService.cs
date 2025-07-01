using AutoMapper;
using BLL.DTO.ResponseDto;
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
    public class ResponseService : IResponseService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public ResponseService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _uow = unitOfWork;
        }

        public async Task<IEnumerable<ResponseResponseDto>> GetAllAsync(string? searchTerm = null, CancellationToken cancellationToken = default)
        {
            var query = _uow.Responses.GetAll();
            query = query.Include(r => r.Worker).Include(r => r.Vacancy);

            // Застосовуємо пошук
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchTermLower = searchTerm.ToLower();
                query = query.Where(r => 
                    r.Worker.FirstName.ToLower().Contains(searchTermLower) ||
                    r.Worker.LastName.ToLower().Contains(searchTermLower) ||
                    r.Vacancy.Position.ToLower().Contains(searchTermLower) ||
                    r.Status.ToString().ToLower().Contains(searchTermLower));
            }

            var responses = await query.ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<ResponseResponseDto>>(responses);
        }

        public async Task<ResponseResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _uow.Responses.GetByIdAsync(
                id,
                include: query => query.Include(r => r.Worker).Include(r => r.Vacancy),
                cancellationToken: cancellationToken);
            if (response == null) return null;

            var dto = _mapper.Map<ResponseResponseDto>(response);
            dto.WorkerFullName = $"{response.Worker.LastName} {response.Worker.FirstName}";
            dto.Position = response.Vacancy.Position;
            return dto;
        }

        public async Task<ResponseResponseDto> CreateAsync(ResponseCreateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<Response>(dto);
            entity.SentAt = DateTime.UtcNow;
            entity.Status = ResponseStatus.Pending;

            await _uow.Responses.AddAsync(entity, cancellationToken);
            await _uow.SaveAsync(cancellationToken);

            return await GetByIdAsync(entity.Id, cancellationToken);
        }

        public async Task<ResponseResponseDto?> UpdateAsync(Guid id, ResponseUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.Responses.GetByIdAsync(
                id,
                include: query => query.Include(r => r.Worker).Include(r => r.Vacancy),
                cancellationToken: cancellationToken);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            _uow.Responses.Update(entity);
            await _uow.SaveAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.Responses.GetByIdAsync(
                id,
                cancellationToken: cancellationToken);
            if (entity == null) return false;

            _uow.Responses.Delete(entity);
            await _uow.SaveAsync(cancellationToken);
            return true;
        }

        public async Task<PagedList<ResponseResponseDto>> GetPagedAsync(SearchParametersDto searchParams, CancellationToken cancellationToken = default)
        {
            var query = _uow.Responses.GetAll();
            query = query.Include(r => r.Worker).Include(r => r.Vacancy);

            // Застосовуємо пошук
            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.ToLower();
                query = query.Where(r => 
                    r.Worker.FirstName.ToLower().Contains(searchTerm) ||
                    r.Worker.LastName.ToLower().Contains(searchTerm) ||
                    r.Vacancy.Position.ToLower().Contains(searchTerm) ||
                    r.Status.ToString().ToLower().Contains(searchTerm));
            }

            // Застосовуємо сортування
            IQueryable<Response> orderedQuery;
            if (!string.IsNullOrEmpty(searchParams.SortBy))
            {
                bool desc = string.Equals(searchParams.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
                orderedQuery = searchParams.SortBy.ToLower() switch
                {
                    "workername" => desc ? query.OrderByDescending(x => x.Worker.LastName) : query.OrderBy(x => x.Worker.LastName),
                    "position" => desc ? query.OrderByDescending(x => x.Vacancy.Position) : query.OrderBy(x => x.Vacancy.Position),
                    "status" => desc ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
                    "sentat" => desc ? query.OrderByDescending(x => x.SentAt) : query.OrderBy(x => x.SentAt),
                    _ => query.OrderBy(x => x.SentAt)
                };
            }
            else
            {
                orderedQuery = query.OrderBy(x => x.SentAt);
            }

            // Виконуємо пагінацію на рівні Entity
            var pagedEntities = await PagedList<Response>.ToPagedListAsync(
                orderedQuery, 
                searchParams.PageNumber, 
                searchParams.PageSize, 
                cancellationToken);

            // Мапимо результат
            var dtos = _mapper.Map<List<ResponseResponseDto>>(pagedEntities.Items);
            
            return new PagedList<ResponseResponseDto>(
                dtos, 
                pagedEntities.TotalCount, 
                pagedEntities.CurrentPage, 
                pagedEntities.PageSize);
        }
    }
}

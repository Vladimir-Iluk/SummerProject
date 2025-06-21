using AutoMapper;
using BLL.DTO.ResponseDto;
using BLL.Interfaces;
using DAL.EF.Entities;
using DAL.EF.UoW;
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

        public async Task<IEnumerable<ResponseResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var responses = await _uow.Responses.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<ResponseResponseDto>>(responses);
        }

        public async Task<ResponseResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _uow.Responses.GetByIdAsync(id, cancellationToken);
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
            var entity = await _uow.Responses.GetByIdAsync(id, cancellationToken);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            _uow.Responses.Update(entity);
            await _uow.SaveAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.Responses.GetByIdAsync(id, cancellationToken);
            if (entity == null) return false;

            _uow.Responses.Delete(entity);
            await _uow.SaveAsync(cancellationToken);
            return true;
        }
    }

}

using AutoMapper;
using BLL.DTO.WorkerDto;
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
    public class WorkerService : IWorkerService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public WorkerService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _uow = unitOfWork;
        }

        public async Task<IEnumerable<WorkerResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var workers = await _uow.Workers.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<WorkerResponseDto>>(workers);
        }

        public async Task<WorkerResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var worker = await _uow.Workers.GetByIdAsync(id, cancellationToken);
            if (worker == null) return null;

            var dto = _mapper.Map<WorkerResponseDto>(worker);
            dto.ActivityTypeName = worker.ActivityType.ActivityName;
            return dto;
        }

        public async Task<WorkerResponseDto> CreateAsync(WorkerCreateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<Worker>(dto);
            await _uow.Workers.AddAsync(entity, cancellationToken);
            await _uow.SaveAsync(cancellationToken);

            return await GetByIdAsync(entity.Id, cancellationToken);
        }

        public async Task<WorkerResponseDto?> UpdateAsync(Guid id, WorkerUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.Workers.GetByIdAsync(id, cancellationToken);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            _uow.Workers.Update(entity);
            await _uow.SaveAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.Workers.GetByIdAsync(id, cancellationToken);
            if (entity == null) return false;

            _uow.Workers.Delete(entity);
            await _uow.SaveAsync(cancellationToken);
            return true;
        }
    }
}

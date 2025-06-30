using AutoMapper;
using BLL.DTO.WorkerDto;
using BLL.Interfaces;
using DAL.EF.Entities;
using DAL.EF.UoW;
using System;
using System.Collections.Generic;
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

        public async Task<WorkerResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var worker = await _uow.Workers.GetByIdAsync(id, cancellationToken);
            if (worker == null)
                throw new KeyNotFoundException($"Worker with ID {id} not found");

            return _mapper.Map<WorkerResponseDto>(worker);
        }

        public async Task<WorkerResponseDto> CreateAsync(WorkerCreateDto dto, CancellationToken cancellationToken = default)
        {
            // Перевіряємо, чи існує вказаний тип активності
            var activityType = await _uow.ActivityTypes.GetByIdAsync(dto.ActivityTypeId, cancellationToken);
            if (activityType == null)
                throw new KeyNotFoundException($"ActivityType with ID {dto.ActivityTypeId} not found");

            var entity = _mapper.Map<Worker>(dto);
            
            await _uow.Workers.AddAsync(entity, cancellationToken);
            await _uow.SaveAsync(cancellationToken);

            // Отримуємо створеного працівника з бази даних
            var createdWorker = await _uow.Workers.GetByIdAsync(entity.Id, cancellationToken);
            if (createdWorker == null)
                throw new Exception("Worker was not created properly");

            return _mapper.Map<WorkerResponseDto>(createdWorker);
        }

        public async Task<WorkerResponseDto> UpdateAsync(Guid id, WorkerUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.Workers.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                throw new KeyNotFoundException($"Worker with ID {id} not found");

            // Перевіряємо, чи існує вказаний тип активності
            if (dto.ActivityTypeId != Guid.Empty)
            {
                var activityType = await _uow.ActivityTypes.GetByIdAsync(dto.ActivityTypeId, cancellationToken);
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
            var entity = await _uow.Workers.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                return false;

            _uow.Workers.Delete(entity);
            await _uow.SaveAsync(cancellationToken);
            return true;
        }
    }
}

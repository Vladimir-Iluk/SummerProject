using AutoMapper;
using BLL.DTO.ActivityTypeDto;
using BLL.Interfaces;
using DAL.EF.Entities;
using DAL.EF.UoW;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<ActivityTypeResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var list = await _uow.ActivityTypes.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<ActivityTypeResponseDto>>(list);
        }

        public async Task<ActivityTypeResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.ActivityTypes.GetByIdAsync(id, cancellationToken);
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
            var entity = await _uow.ActivityTypes.GetByIdAsync(id, cancellationToken);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            _uow.ActivityTypes.Update(entity);
            await _uow.SaveAsync(cancellationToken);
            return _mapper.Map<ActivityTypeResponseDto>(entity);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.ActivityTypes.GetByIdAsync(id, cancellationToken);
            if (entity == null) return false;

            _uow.ActivityTypes.Delete(entity); 
            await _uow.SaveAsync(cancellationToken);
            return true;
        }
    }
}
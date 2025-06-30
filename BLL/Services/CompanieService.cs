using AutoMapper;
using BLL.DTO.CompanieDto;
using BLL.Interfaces;
using DAL.EF.Entities;
using DAL.EF.UoW;
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

        public async Task<IEnumerable<CompanieResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var companies = await _uow.Companies.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<CompanieResponseDto>>(companies);
        }

        public async Task<CompanieResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var company = await _uow.Companies.GetByIdAsync(id, cancellationToken);
            if (company == null) return null;

            var dto = _mapper.Map<CompanieResponseDto>(company);
            dto.ActivityTypeName = company.ActivityType.ActivityName;
            return dto;
        }

        public async Task<CompanieResponseDto> CreateAsync(CompanieCreateDto dto, CancellationToken cancellationToken = default)
        {
            // Перевіряємо, чи існує вказаний тип активності
            var activityType = await _uow.ActivityTypes.GetByIdAsync(dto.ActivityTypeId, cancellationToken);
            if (activityType == null)
            {
                throw new ValidationException($"ActivityType з ID {dto.ActivityTypeId} не знайдено");
            }

            var entity = _mapper.Map<Companie>(dto);
            await _uow.Companies.AddAsync(entity, cancellationToken);
            await _uow.SaveAsync(cancellationToken);

            var createdCompany = await _uow.Companies.GetByIdAsync(entity.Id, cancellationToken);
            return _mapper.Map<CompanieResponseDto>(createdCompany);
        }

        public async Task<CompanieResponseDto?> UpdateAsync(Guid id, CompanieUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.Companies.GetByIdAsync(id, cancellationToken);
            if (entity == null) return null;

            // Перевіряємо, чи існує вказаний тип активності
            if (dto.ActivityTypeId != entity.ActivityTypeId)
            {
                var activityType = await _uow.ActivityTypes.GetByIdAsync(dto.ActivityTypeId, cancellationToken);
                if (activityType == null)
                {
                    throw new ValidationException($"ActivityType з ID {dto.ActivityTypeId} не знайдено");
                }
            }

            _mapper.Map(dto, entity);
            _uow.Companies.Update(entity);
            await _uow.SaveAsync(cancellationToken);

            var updatedCompany = await _uow.Companies.GetByIdAsync(id, cancellationToken);
            return _mapper.Map<CompanieResponseDto>(updatedCompany);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.Companies.GetByIdAsync(id, cancellationToken);
            if (entity == null) return false;

            _uow.Companies.Delete(entity);
            await _uow.SaveAsync(cancellationToken);
            return true;
        }
    }
}

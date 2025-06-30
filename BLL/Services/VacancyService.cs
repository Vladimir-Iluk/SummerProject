using AutoMapper;
using BLL.DTO.VacancyDto;
using BLL.Interfaces;
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

        public async Task<IEnumerable<VacancyResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var vacancies = await _uow.Vacancy.GetAllAsync(
                include: query => query.Include(v => v.Companie),
                cancellationToken: cancellationToken);
            return _mapper.Map<IEnumerable<VacancyResponseDto>>(vacancies);
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

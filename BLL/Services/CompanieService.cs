using AutoMapper;
using BLL.DTO.CompanieDto;
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
            var entity = _mapper.Map<Companie>(dto);
            await _uow.Companies.AddAsync(entity, cancellationToken);
            await _uow.SaveAsync(cancellationToken);

            return await GetByIdAsync(entity.Id, cancellationToken);
        }

        public async Task<CompanieResponseDto?> UpdateAsync(Guid id, CompanieUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.Companies.GetByIdAsync(id, cancellationToken);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            _uow.Companies.Update(entity);
            await _uow.SaveAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
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

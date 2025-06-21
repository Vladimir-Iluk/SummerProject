using AutoMapper;
using BLL.DTO.AgreementDto;
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
    public class AgreementService : IAgreementService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public AgreementService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _uow = unitOfWork;
        }

        public async Task<IEnumerable<AgreementResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var agreements = await _uow.Agreements.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<AgreementResponseDto>>(agreements);
        }

        public async Task<AgreementResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var agreement = await _uow.Agreements.GetByIdAsync(id, cancellationToken);
            if (agreement == null) return null;

            var dto = _mapper.Map<AgreementResponseDto>(agreement);
            dto.WorkerFullName = $"{agreement.Worker.LastName} {agreement.Worker.FirstName}";
            dto.CompanyName = agreement.Companie.CompanyName;
            return dto;
        }

        public async Task<AgreementResponseDto> CreateAsync(AgreementCreateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<Agreement>(dto);
            entity.AgreementDate = DateTime.UtcNow;

            await _uow.Agreements.AddAsync(entity, cancellationToken);
            await _uow.SaveAsync(cancellationToken);

            return await GetByIdAsync(entity.Id, cancellationToken);
        }

        public async Task<AgreementResponseDto?> UpdateAsync(Guid id, AgreementUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.Agreements.GetByIdAsync(id, cancellationToken);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            _uow.Agreements.Update(entity);
            await _uow.SaveAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.Agreements.GetByIdAsync(id, cancellationToken);
            if (entity == null) return false;

            _uow.Agreements.Delete(entity);
            await _uow.SaveAsync(cancellationToken);
            return true;
        }
    }

}

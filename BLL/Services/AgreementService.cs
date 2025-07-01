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
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class AgreementService : IAgreementService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AgreementService> _logger;

        public AgreementService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<AgreementService> logger)
        {
            _mapper = mapper;
            _uow = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<AgreementResponseDto>> GetAllAsync(string? sortBy = null, string? sortDirection = null, CancellationToken cancellationToken = default)
        {
            var agreements = await _uow.Agreements.GetAllAsync(
                include: query => query.Include(a => a.Worker).Include(a => a.Companie),
                cancellationToken: cancellationToken);
            var dtos = _mapper.Map<IEnumerable<AgreementResponseDto>>(agreements);

            if (!string.IsNullOrEmpty(sortBy))
            {
                bool desc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
                dtos = sortBy.ToLower() switch
                {
                    "workerfullname" => desc ? dtos.OrderByDescending(x => x.WorkerFullName) : dtos.OrderBy(x => x.WorkerFullName),
                    "companyname" => desc ? dtos.OrderByDescending(x => x.CompanyName) : dtos.OrderBy(x => x.CompanyName),
                    "position" => desc ? dtos.OrderByDescending(x => x.Position) : dtos.OrderBy(x => x.Position),
                    "commission" => desc ? dtos.OrderByDescending(x => x.Commission) : dtos.OrderBy(x => x.Commission),
                    _ => dtos
                };
            }
            return dtos;
        }

        public async Task<AgreementResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var agreement = await _uow.Agreements.GetByIdAsync(
                id,
                include: query => query.Include(a => a.Worker).Include(a => a.Companie),
                cancellationToken: cancellationToken);
            return agreement == null ? null : _mapper.Map<AgreementResponseDto>(agreement);
        }

        public async Task<AgreementResponseDto> CreateAsync(AgreementCreateDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Початок створення угоди. WorkerId: {WorkerId}, CompanieId: {CompanieId}", 
                    dto.WorkerId, dto.CompanieId);

                // Перевіряємо існування працівника
                var worker = await _uow.Workers.GetByIdAsync(
                    dto.WorkerId,
                    cancellationToken: cancellationToken);
                if (worker == null)
                {
                    _logger.LogWarning("Працівник з ID {WorkerId} не знайдений", dto.WorkerId);
                    throw new ValidationException($"Працівник з ID {dto.WorkerId} не знайдений");
                }

                // Перевіряємо існування компанії
                var company = await _uow.Companies.GetByIdAsync(
                    dto.CompanieId,
                    cancellationToken: cancellationToken);
                if (company == null)
                {
                    _logger.LogWarning("Компанія з ID {CompanieId} не знайдена", dto.CompanieId);
                    throw new ValidationException($"Компанія з ID {dto.CompanieId} не знайдена");
                }

                // Перевіряємо, чи не існує вже угода для цього працівника з цією компанією
                var existingAgreement = await _uow.Agreements.FindAsync(
                    a => a.WorkerId == dto.WorkerId && a.CompanieId == dto.CompanieId,
                    cancellationToken: cancellationToken);

                if (existingAgreement.Any())
                {
                    _logger.LogWarning("Угода між працівником {WorkerId} та компанією {CompanieId} вже існує",
                        dto.WorkerId, dto.CompanieId);
                    throw new ValidationException("Угода між цим працівником та компанією вже існує");
                }

                // Валідація комісії
                if (dto.Commission < 0 || dto.Commission > 100)
                {
                    _logger.LogWarning("Недійсне значення комісії: {Commission}", dto.Commission);
                    throw new ValidationException("Комісія повинна бути в межах від 0 до 100");
                }

                // Валідація позиції
                if (string.IsNullOrWhiteSpace(dto.Position))
                {
                    _logger.LogWarning("Позиція не вказана");
                    throw new ValidationException("Позиція є обов'язковою");
                }

                var entity = _mapper.Map<Agreement>(dto);
                entity.AgreementDate = DateTime.UtcNow;

                await _uow.Agreements.AddAsync(entity, cancellationToken);
                await _uow.SaveAsync(cancellationToken);

                _logger.LogInformation("Угоду успішно створено з ID: {AgreementId}", entity.Id);

                return await GetByIdAsync(entity.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при створенні угоди");
                throw;
            }
        }

        public async Task<AgreementResponseDto?> UpdateAsync(Guid id, AgreementUpdateDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Початок оновлення угоди {AgreementId}", id);

                var entity = await _uow.Agreements.GetByIdAsync(
                    id,
                    include: query => query.Include(a => a.Worker).Include(a => a.Companie),
                    cancellationToken: cancellationToken);
                if (entity == null)
                {
                    _logger.LogWarning("Угода з ID {AgreementId} не знайдена", id);
                    return null;
                }

                // Перевіряємо існування працівника, якщо ID змінився
                if (dto.WorkerId != entity.WorkerId)
                {
                    var worker = await _uow.Workers.GetByIdAsync(
                        dto.WorkerId,
                        cancellationToken: cancellationToken);
                    if (worker == null)
                    {
                        _logger.LogWarning("Працівник з ID {WorkerId} не знайдений", dto.WorkerId);
                        throw new ValidationException($"Працівник з ID {dto.WorkerId} не знайдений");
                    }
                }

                // Перевіряємо існування компанії, якщо ID змінився
                if (dto.CompanieId != entity.CompanieId)
                {
                    var company = await _uow.Companies.GetByIdAsync(
                        dto.CompanieId,
                        cancellationToken: cancellationToken);
                    if (company == null)
                    {
                        _logger.LogWarning("Компанія з ID {CompanieId} не знайдена", dto.CompanieId);
                        throw new ValidationException($"Компанія з ID {dto.CompanieId} не знайдена");
                    }

                    // Перевіряємо, чи не існує вже угода для цього працівника з новою компанією
                    var existingAgreement = await _uow.Agreements.FindAsync(
                        a => a.Id != id && a.WorkerId == dto.WorkerId && a.CompanieId == dto.CompanieId,
                        cancellationToken: cancellationToken);

                    if (existingAgreement.Any())
                    {
                        _logger.LogWarning("Угода між працівником {WorkerId} та компанією {CompanieId} вже існує",
                            dto.WorkerId, dto.CompanieId);
                        throw new ValidationException("Угода між цим працівником та компанією вже існує");
                    }
                }

                // Валідація комісії
                if (dto.Commission < 0 || dto.Commission > 100)
                {
                    _logger.LogWarning("Недійсне значення комісії: {Commission}", dto.Commission);
                    throw new ValidationException("Комісія повинна бути в межах від 0 до 100");
                }

                // Валідація позиції
                if (string.IsNullOrWhiteSpace(dto.Position))
                {
                    _logger.LogWarning("Позиція не вказана");
                    throw new ValidationException("Позиція є обов'язковою");
                }

                _mapper.Map(dto, entity);
                _uow.Agreements.Update(entity);
                await _uow.SaveAsync(cancellationToken);

                _logger.LogInformation("Угоду {AgreementId} успішно оновлено", id);

                return await GetByIdAsync(id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при оновленні угоди {AgreementId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Початок видалення угоди {AgreementId}", id);

                var entity = await _uow.Agreements.GetByIdAsync(
                    id,
                    cancellationToken: cancellationToken);
                if (entity == null)
                {
                    _logger.LogWarning("Угода з ID {AgreementId} не знайдена", id);
                    return false;
                }

                _uow.Agreements.Delete(entity);
                await _uow.SaveAsync(cancellationToken);

                _logger.LogInformation("Угоду {AgreementId} успішно видалено", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при видаленні угоди {AgreementId}", id);
                throw;
            }
        }
    }
}

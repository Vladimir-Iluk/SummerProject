using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using BLL.DTO.AgreementDto;
using BLL.Pagination;
using BLL.Exceptions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace SummerProj.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Базова авторизація для всіх методів
    public class AgreementController : ControllerBase
    {
        private readonly IAgreementService _agreementService;

        public AgreementController(IAgreementService agreementService)
        {
            _agreementService = agreementService;
        }

        /// <summary>
        /// Отримати всі угоди
        /// </summary>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Список всіх угод</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")] // Тільки адміністратори можуть бачити всі угоди
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AgreementResponseDto>>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var agreements = await _agreementService.GetAllAsync(cancellationToken);
                return Ok(agreements);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при отриманні угод", error = ex.Message });
            }
        }

        /// <summary>
        /// Отримати угоду за ID
        /// </summary>
        /// <param name="id">ID угоди</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Угода</returns>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")] // Тільки адміністратори можуть отримувати угоди за ID
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AgreementResponseDto>> GetByIdAsync(
            [Required] Guid id, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var agreement = await _agreementService.GetByIdAsync(id, cancellationToken);
                
                if (agreement == null)
                {
                    return NotFound(new { message = $"Угода з ID {id} не знайдена" });
                }

                return Ok(agreement);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при отриманні угоди", error = ex.Message });
            }
        }

        /// <summary>
        /// Створити нову угоду
        /// </summary>
        /// <param name="createDto">Дані для створення угоди</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Створена угода</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")] // Тільки адміністратори можуть створювати угоди
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AgreementResponseDto>> CreateAsync(
            [FromBody] AgreementCreateDto createDto, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdAgreement = await _agreementService.CreateAsync(createDto, cancellationToken);
                
                return CreatedAtAction(
                    nameof(GetByIdAsync), 
                    new { id = createdAgreement.Id }, 
                    createdAgreement);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = "Помилка валідації", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при створенні угоди", error = ex.Message });
            }
        }

        /// <summary>
        /// Оновити угоду
        /// </summary>
        /// <param name="id">ID угоди</param>
        /// <param name="updateDto">Дані для оновлення</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Оновлена угода</returns>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")] // Тільки адміністратори можуть оновлювати угоди
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AgreementResponseDto>> UpdateAsync(
            [Required] Guid id, 
            [FromBody] AgreementUpdateDto updateDto, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedAgreement = await _agreementService.UpdateAsync(id, updateDto, cancellationToken);
                
                if (updatedAgreement == null)
                {
                    return NotFound(new { message = $"Угода з ID {id} не знайдена" });
                }

                return Ok(updatedAgreement);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = "Помилка валідації", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при оновленні угоди", error = ex.Message });
            }
        }

        /// <summary>
        /// Видалити угоду
        /// </summary>
        /// <param name="id">ID угоди</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Результат видалення</returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")] // Тільки адміністратори можуть видаляти угоди
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAsync(
            [Required] Guid id, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var isDeleted = await _agreementService.DeleteAsync(id, cancellationToken);
                
                if (!isDeleted)
                {
                    return NotFound(new { message = $"Угода з ID {id} не знайдена" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при видаленні угоди", error = ex.Message });
            }
        }
    }
} 
using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using BLL.DTO.VacancyDto;
using BLL.Pagination;
using BLL.Exceptions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace SummerProj.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Базова авторизація для всіх методів
    public class VacancyController : ControllerBase
    {
        private readonly IVacancyService _vacancyService;

        public VacancyController(IVacancyService vacancyService)
        {
            _vacancyService = vacancyService;
        }

        /// <summary>
        /// Отримати всі вакансії
        /// </summary>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Список всіх вакансій</returns>
        [HttpGet]
        [AllowAnonymous] // Доступно всім
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<VacancyResponseDto>>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var vacancies = await _vacancyService.GetAllAsync(cancellationToken);
                return Ok(vacancies);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при отриманні вакансій", error = ex.Message });
            }
        }

        /// <summary>
        /// Отримати вакансію за ID
        /// </summary>
        /// <param name="id">ID вакансії</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Вакансія</returns>
        [HttpGet("{id:guid}")]
        [AllowAnonymous] // Доступно всім
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VacancyResponseDto>> GetByIdAsync(
            [Required] Guid id, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var vacancy = await _vacancyService.GetByIdAsync(id, cancellationToken);
                
                if (vacancy == null)
                {
                    return NotFound(new { message = $"Вакансія з ID {id} не знайдена" });
                }

                return Ok(vacancy);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при отриманні вакансії", error = ex.Message });
            }
        }

        /// <summary>
        /// Створити нову вакансію
        /// </summary>
        /// <param name="createDto">Дані для створення вакансії</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Створена вакансія</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")] // Тільки для адміністраторів
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VacancyResponseDto>> CreateAsync(
            [FromBody] VacancyCreateDto createDto, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdVacancy = await _vacancyService.CreateAsync(createDto, cancellationToken);
                
                return CreatedAtAction(
                    nameof(GetByIdAsync), 
                    new { id = createdVacancy.Id }, 
                    createdVacancy);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = "Помилка валідації", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при створенні вакансії", error = ex.Message });
            }
        }

        /// <summary>
        /// Оновити вакансію
        /// </summary>
        /// <param name="id">ID вакансії</param>
        /// <param name="updateDto">Дані для оновлення</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Оновлена вакансія</returns>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")] // Тільки для адміністраторів
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VacancyResponseDto>> UpdateAsync(
            [Required] Guid id, 
            [FromBody] VacancyUpdateDto updateDto, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedVacancy = await _vacancyService.UpdateAsync(id, updateDto, cancellationToken);
                
                if (updatedVacancy == null)
                {
                    return NotFound(new { message = $"Вакансія з ID {id} не знайдена" });
                }

                return Ok(updatedVacancy);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = "Помилка валідації", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при оновленні вакансії", error = ex.Message });
            }
        }

        /// <summary>
        /// Видалити вакансію
        /// </summary>
        /// <param name="id">ID вакансії</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Результат видалення</returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")] // Тільки для адміністраторів
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAsync(
            [Required] Guid id, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var isDeleted = await _vacancyService.DeleteAsync(id, cancellationToken);
                
                if (!isDeleted)
                {
                    return NotFound(new { message = $"Вакансія з ID {id} не знайдена" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при видаленні вакансії", error = ex.Message });
            }
        }
    }
} 
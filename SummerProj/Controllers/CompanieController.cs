using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using BLL.DTO.CompanieDto;
using BLL.Pagination;
using BLL.Exceptions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace SummerProj.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Базова авторизація для всіх методів
    public class CompanieController : ControllerBase
    {
        private readonly ICompanieService _companieService;
        private readonly ILogger<CompanieController> _logger;

        public CompanieController(ICompanieService companieService, ILogger<CompanieController> logger)
        {
            _companieService = companieService;
            _logger = logger;
        }

        /// <summary>
        /// Отримати всі компанії
        /// </summary>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Список всіх компаній</returns>
        [HttpGet]
        [AllowAnonymous] // Доступно всім
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CompanieResponseDto>>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var companies = await _companieService.GetAllAsync(cancellationToken);
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при отриманні компаній", error = ex.Message });
            }
        }

        /// <summary>
        /// Отримати компанію за ID
        /// </summary>
        /// <param name="id">ID компанії</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Компанія</returns>
        [HttpGet("{id:guid}", Name = "GetCompanyById")]
        [AllowAnonymous] // Доступно всім
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CompanieResponseDto>> GetByIdAsync(
            [Required] Guid id, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var company = await _companieService.GetByIdAsync(id, cancellationToken);
                
                if (company == null)
                {
                    return NotFound(new { message = $"Компанія з ID {id} не знайдена" });
                }

                return Ok(company);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при отриманні компанії", error = ex.Message });
            }
        }

        /// <summary>
        /// Створити нову компанію
        /// </summary>
        /// <param name="createDto">Дані для створення компанії</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Створена компанія</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")] // Тільки для адміністраторів
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CompanieResponseDto>> CreateAsync(
            [FromBody] CompanieCreateDto createDto, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Логуємо отримані дані
                _logger.LogInformation("Отримано запит на створення компанії: {RequestBody}", 
                    JsonSerializer.Serialize(createDto));

                // Перевіряємо тіло запиту
                if (!Request.Body.CanRead)
                {
                    return BadRequest(new { message = "Тіло запиту не може бути прочитане" });
                }

                if (createDto == null)
                {
                    return BadRequest(new { message = "Дані для створення компанії не надані" });
                }

                // Перевіряємо валідність моделі
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return BadRequest(new { message = "Неправильний формат даних", errors });
                }

                // Валідуємо обов'язкові поля
                if (string.IsNullOrWhiteSpace(createDto.CompanyName))
                {
                    return BadRequest(new { message = "Назва компанії є обов'язковою" });
                }

                if (string.IsNullOrWhiteSpace(createDto.EmailCompany))
                {
                    return BadRequest(new { message = "Email компанії є обов'язковим" });
                }

                if (string.IsNullOrWhiteSpace(createDto.Address))
                {
                    return BadRequest(new { message = "Адреса компанії є обов'язковою" });
                }

                if (string.IsNullOrWhiteSpace(createDto.Phone))
                {
                    return BadRequest(new { message = "Телефон компанії є обов'язковим" });
                }

                if (createDto.ActivityTypeId == Guid.Empty)
                {
                    return BadRequest(new { message = "ID типу активності є обов'язковим" });
                }

                var createdCompany = await _companieService.CreateAsync(createDto, cancellationToken);
                
                _logger.LogInformation("Компанію успішно створено з ID: {CompanyId}", createdCompany.Id);

                return CreatedAtRoute(
                    "GetCompanyById",
                    new { id = createdCompany.Id },
                    createdCompany);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Помилка валідації при створенні компанії: {Error}", ex.Message);
                return BadRequest(new { message = "Помилка валідації", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при створенні компанії");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при створенні компанії", error = ex.Message });
            }
        }

        /// <summary>
        /// Оновити компанію
        /// </summary>
        /// <param name="id">ID компанії</param>
        /// <param name="updateDto">Дані для оновлення</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Оновлена компанія</returns>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")] // Тільки для адміністраторів
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CompanieResponseDto>> UpdateAsync(
            [Required] Guid id, 
            [FromBody] CompanieUpdateDto updateDto, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedCompany = await _companieService.UpdateAsync(id, updateDto, cancellationToken);
                
                if (updatedCompany == null)
                {
                    return NotFound(new { message = $"Компанія з ID {id} не знайдена" });
                }

                return Ok(updatedCompany);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = "Помилка валідації", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при оновленні компанії", error = ex.Message });
            }
        }

        /// <summary>
        /// Видалити компанію
        /// </summary>
        /// <param name="id">ID компанії</param>
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
                var isDeleted = await _companieService.DeleteAsync(id, cancellationToken);
                
                if (!isDeleted)
                {
                    return NotFound(new { message = $"Компанія з ID {id} не знайдена" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при видаленні компанії", error = ex.Message });
            }
        }
    }
}

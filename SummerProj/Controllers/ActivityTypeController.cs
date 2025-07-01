using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using BLL.DTO.ActivityTypeDto;
using BLL.DTO.CommonDto;
using BLL.Pagination;
using BLL.Exceptions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace SummerProj.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Базова авторизація для всіх методів
    public class ActivityTypeController : ControllerBase
    {
        private readonly IActivityTypeService _activityTypeService;
        private readonly ILogger<ActivityTypeController> _logger;

        public ActivityTypeController(IActivityTypeService activityTypeService, ILogger<ActivityTypeController> logger)
        {
            _activityTypeService = activityTypeService;
            _logger = logger;
        }

        /// <summary>
        /// Отримати типи активності з пагінацією та пошуком
        /// </summary>
        /// <param name="searchParams">Параметри пошуку та пагінації</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Сторінкований список типів активності</returns>
        [HttpGet]
        [AllowAnonymous] // Доступно всім
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedList<ActivityTypeResponseDto>>> GetAllAsync(
            [FromQuery] SearchParametersDto searchParams,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return BadRequest(new { message = "Неправильні параметри пошуку", errors });
                }

                var activityTypes = await _activityTypeService.GetPagedAsync(searchParams, cancellationToken);
                return Ok(activityTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при отриманні типів активності", error = ex.Message });
            }
        }

        /// <summary>
        /// Отримати тип активності за ID
        /// </summary>
        /// <param name="id">ID типу активності</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Тип активності</returns>
        [HttpGet("{id:guid}", Name = "GetActivityTypeById")]
        [AllowAnonymous] // Доступно всім
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ActivityTypeResponseDto>> GetByIdAsync(
            [Required] Guid id, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var activityType = await _activityTypeService.GetByIdAsync(id, cancellationToken);
                
                if (activityType == null)
                {
                    return NotFound(new { message = $"Тип активності з ID {id} не знайдений" });
                }

                return Ok(activityType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні типу активності");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при отриманні типу активності", error = ex.Message });
            }
        }

        /// <summary>
        /// Створити новий тип активності
        /// </summary>
        /// <param name="createDto">Дані для створення типу активності</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Створений тип активності</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")] // Тільки для адміністраторів
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ActivityTypeResponseDto>> CreateAsync(
            [FromBody] ActivityTypeCreateDto createDto, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Отримано запит на створення типу активності: {CreateDto}", createDto);

                if (createDto == null)
                {
                    return BadRequest(new { message = "Дані для створення типу активності не надані" });
                }

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

                var createdActivityType = await _activityTypeService.CreateAsync(createDto, cancellationToken);
                
                _logger.LogInformation("Тип активності успішно створено з ID: {ActivityTypeId}", createdActivityType.Id);

                return CreatedAtRoute(
                    "GetActivityTypeById",
                    new { id = createdActivityType.Id },
                    createdActivityType);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Помилка валідації при створенні типу активності: {Error}", ex.Message);
                return BadRequest(new { message = "Помилка валідації", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при створенні типу активності");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при створенні типу активності", error = ex.Message });
            }
        }

        /// <summary>
        /// Оновити тип активності
        /// </summary>
        /// <param name="id">ID типу активності</param>
        /// <param name="updateDto">Дані для оновлення</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Оновлений тип активності</returns>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")] // Тільки для адміністраторів
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ActivityTypeResponseDto>> UpdateAsync(
            [Required] Guid id, 
            [FromBody] ActivityTypeUpdateDto updateDto, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedActivityType = await _activityTypeService.UpdateAsync(id, updateDto, cancellationToken);
                
                if (updatedActivityType == null)
                {
                    return NotFound(new { message = $"Тип активності з ID {id} не знайдений" });
                }

                return Ok(updatedActivityType);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Помилка валідації при оновленні типу активності: {Error}", ex.Message);
                return BadRequest(new { message = "Помилка валідації", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при оновленні типу активності");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при оновленні типу активності", error = ex.Message });
            }
        }

        /// <summary>
        /// Видалити тип активності
        /// </summary>
        /// <param name="id">ID типу активності</param>
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
                var isDeleted = await _activityTypeService.DeleteAsync(id, cancellationToken);
                
                if (!isDeleted)
                {
                    return NotFound(new { message = $"Тип активності з ID {id} не знайдений" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при видаленні типу активності");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при видаленні типу активності", error = ex.Message });
            }
        }
    }
} 
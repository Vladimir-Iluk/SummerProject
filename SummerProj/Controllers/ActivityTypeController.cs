using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using BLL.DTO.ActivityTypeDto;
using BLL.Pagination;
using BLL.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace SummerProj.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityTypeController : ControllerBase
    {
        private readonly IActivityTypeService _activityTypeService;

        public ActivityTypeController(IActivityTypeService activityTypeService)
        {
            _activityTypeService = activityTypeService;
        }

        /// <summary>
        /// Отримати всі типи активності
        /// </summary>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Список всіх типів активності</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ActivityTypeResponseDto>>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var activityTypes = await _activityTypeService.GetAllAsync(cancellationToken);
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
        [HttpGet("{id:guid}")]
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ActivityTypeResponseDto>> CreateAsync(
            [FromBody] ActivityTypeCreateDto createDto, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdActivityType = await _activityTypeService.CreateAsync(createDto, cancellationToken);
                
                return CreatedAtAction(
                    nameof(GetByIdAsync), 
                    new { id = createdActivityType.Id }, 
                    createdActivityType);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = "Помилка валідації", error = ex.Message });
            }
            catch (Exception ex)
            {
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
                return BadRequest(new { message = "Помилка валідації", error = ex.Message });
            }
            catch (Exception ex)
            {
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
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при видаленні типу активності", error = ex.Message });
            }
        }
    }
} 
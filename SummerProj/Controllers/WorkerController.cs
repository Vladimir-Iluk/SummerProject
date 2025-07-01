using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using BLL.DTO.WorkerDto;
using BLL.Pagination;
using BLL.Exceptions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace SummerProj.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkerController : ControllerBase
    {
        private readonly IWorkerService _workerService;

        public WorkerController(IWorkerService workerService)
        {
            _workerService = workerService;
        }

        /// <summary>
        /// Отримати всіх працівників
        /// </summary>
        /// <param name="sortBy">Поле для сортування</param>
        /// <param name="sortDirection">Напрямок сортування</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Список всіх працівників</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<WorkerResponseDto>>> GetAllAsync(
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortDirection = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var workers = await _workerService.GetAllAsync(sortBy, sortDirection, cancellationToken);
                return Ok(workers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при отриманні працівників", error = ex.Message });
            }
        }

        /// <summary>
        /// Отримати працівника за ID
        /// </summary>
        /// <param name="id">ID працівника</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Працівник</returns>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<WorkerResponseDto>> GetByIdAsync(
            [Required] Guid id, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var worker = await _workerService.GetByIdAsync(id, cancellationToken);
                
                if (worker == null)
                {
                    return NotFound(new { message = $"Працівник з ID {id} не знайдений" });
                }

                return Ok(worker);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при отриманні працівника", error = ex.Message });
            }
        }

        /// <summary>
        /// Створити нового працівника
        /// </summary>
        /// <param name="createDto">Дані для створення працівника</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Створений працівник</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<WorkerResponseDto>> CreateAsync(
            [FromBody] WorkerCreateDto createDto, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdWorker = await _workerService.CreateAsync(createDto, cancellationToken);
                
                return CreatedAtAction(
                    nameof(GetByIdAsync), 
                    new { id = createdWorker.Id }, 
                    createdWorker);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = "Помилка валідації", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при створенні працівника", error = ex.Message });
            }
        }

        /// <summary>
        /// Оновити працівника
        /// </summary>
        /// <param name="id">ID працівника</param>
        /// <param name="updateDto">Дані для оновлення</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Оновлений працівник</returns>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<WorkerResponseDto>> UpdateAsync(
            [Required] Guid id, 
            [FromBody] WorkerUpdateDto updateDto, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedWorker = await _workerService.UpdateAsync(id, updateDto, cancellationToken);
                
                if (updatedWorker == null)
                {
                    return NotFound(new { message = $"Працівник з ID {id} не знайдений" });
                }

                return Ok(updatedWorker);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = "Помилка валідації", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при оновленні працівника", error = ex.Message });
            }
        }

        /// <summary>
        /// Видалити працівника
        /// </summary>
        /// <param name="id">ID працівника</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Результат видалення</returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAsync(
            [Required] Guid id, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var isDeleted = await _workerService.DeleteAsync(id, cancellationToken);
                
                if (!isDeleted)
                {
                    return NotFound(new { message = $"Працівник з ID {id} не знайдений" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при видаленні працівника", error = ex.Message });
            }
        }
    }
} 
using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using BLL.DTO.ResponseDto;
using BLL.Pagination;
using BLL.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace SummerProj.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResponseController : ControllerBase
    {
        private readonly IResponseService _responseService;

        public ResponseController(IResponseService responseService)
        {
            _responseService = responseService;
        }

        /// <summary>
        /// Отримати всі відгуки
        /// </summary>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Список всіх відгуків</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ResponseResponseDto>>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var responses = await _responseService.GetAllAsync(cancellationToken);
                return Ok(responses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при отриманні відгуків", error = ex.Message });
            }
        }

        /// <summary>
        /// Отримати відгук за ID
        /// </summary>
        /// <param name="id">ID відгуку</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Відгук</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseResponseDto>> GetByIdAsync(
            [Required] Guid id, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _responseService.GetByIdAsync(id, cancellationToken);
                
                if (response == null)
                {
                    return NotFound(new { message = $"Відгук з ID {id} не знайдений" });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при отриманні відгуку", error = ex.Message });
            }
        }

        /// <summary>
        /// Створити новий відгук
        /// </summary>
        /// <param name="createDto">Дані для створення відгуку</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Створений відгук</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseResponseDto>> CreateAsync(
            [FromBody] ResponseCreateDto createDto, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdResponse = await _responseService.CreateAsync(createDto, cancellationToken);
                
                return CreatedAtAction(
                    nameof(GetByIdAsync), 
                    new { id = createdResponse.Id }, 
                    createdResponse);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = "Помилка валідації", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при створенні відгуку", error = ex.Message });
            }
        }

        /// <summary>
        /// Оновити відгук
        /// </summary>
        /// <param name="id">ID відгуку</param>
        /// <param name="updateDto">Дані для оновлення</param>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Оновлений відгук</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseResponseDto>> UpdateAsync(
            [Required] Guid id, 
            [FromBody] ResponseUpdateDto updateDto, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedResponse = await _responseService.UpdateAsync(id, updateDto, cancellationToken);
                
                if (updatedResponse == null)
                {
                    return NotFound(new { message = $"Відгук з ID {id} не знайдений" });
                }

                return Ok(updatedResponse);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = "Помилка валідації", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при оновленні відгуку", error = ex.Message });
            }
        }

        /// <summary>
        /// Видалити відгук
        /// </summary>
        /// <param name="id">ID відгуку</param>
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
                var isDeleted = await _responseService.DeleteAsync(id, cancellationToken);
                
                if (!isDeleted)
                {
                    return NotFound(new { message = $"Відгук з ID {id} не знайдений" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при видаленні відгуку", error = ex.Message });
            }
        }
    }
} 
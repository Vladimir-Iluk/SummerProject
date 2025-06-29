using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using BLL.DTO.CompanieDto;
using BLL.Pagination;
using BLL.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace SummerProj.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanieController : ControllerBase
    {
        private readonly ICompanieService _companieService;

        public CompanieController(ICompanieService companieService)
        {
            _companieService = companieService;
        }

        /// <summary>
        /// Отримати всі компанії
        /// </summary>
        /// <param name="cancellationToken">Токен скасування</param>
        /// <returns>Список всіх компаній</returns>
        [HttpGet]
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
        [HttpGet("{id:guid}")]
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CompanieResponseDto>> CreateAsync(
            [FromBody] CompanieCreateDto createDto, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdCompany = await _companieService.CreateAsync(createDto, cancellationToken);
                
                return CreatedAtAction(
                    nameof(GetByIdAsync), 
                    new { id = createdCompany.Id }, 
                    createdCompany);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = "Помилка валідації", error = ex.Message });
            }
            catch (Exception ex)
            {
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

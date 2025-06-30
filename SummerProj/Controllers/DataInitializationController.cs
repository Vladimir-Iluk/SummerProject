using Microsoft.AspNetCore.Mvc;
using DAL.EF.DbCreating;
using Microsoft.EntityFrameworkCore;

namespace SummerProj.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataInitializationController : ControllerBase
    {
        private readonly SummerDbContext _context;

        public DataInitializationController(SummerDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Ініціалізувати базу даних тестовими даними
        /// </summary>
        /// <returns>Результат ініціалізації</returns>
        [HttpPost("seed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SeedDataAsync()
        {
            try
            {
                var hasData = await _context.ActivityTypes.AnyAsync() ||
                             await _context.Companies.AnyAsync() ||
                             await _context.Workers.AnyAsync() ||
                             await _context.Vacancies.AnyAsync() ||
                             await _context.Responses.AnyAsync() ||
                             await _context.Agreements.AnyAsync();

                if (hasData)
                {
                    return BadRequest(new { message = "База даних вже містить дані. Очистіть базу даних перед повторною ініціалізацією." });
                }

                _context.SeedData();

                return Ok(new { 
                    message = "База даних успішно ініціалізована тестовими даними",
                    details = new
                    {
                        activityTypes = await _context.ActivityTypes.CountAsync(),
                        companies = await _context.Companies.CountAsync(),
                        workers = await _context.Workers.CountAsync(),
                        vacancies = await _context.Vacancies.CountAsync(),
                        responses = await _context.Responses.CountAsync(),
                        agreements = await _context.Agreements.CountAsync()
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при ініціалізації бази даних", error = ex.Message });
            }
        }

        /// <summary>
        /// Очистити всі дані з бази даних
        /// </summary>
        /// <returns>Результат очищення</returns>
        [HttpDelete("clear")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ClearDataAsync()
        {
            try
            {
                _context.Agreements.RemoveRange(_context.Agreements);
                _context.Responses.RemoveRange(_context.Responses);
                _context.Vacancies.RemoveRange(_context.Vacancies);
                _context.Workers.RemoveRange(_context.Workers);
                _context.Companies.RemoveRange(_context.Companies);
                _context.ActivityTypes.RemoveRange(_context.ActivityTypes);

                await _context.SaveChangesAsync();

                return Ok(new { message = "Всі дані успішно видалені з бази даних" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при очищенні бази даних", error = ex.Message });
            }
        }

        /// <summary>
        /// Отримати статистику бази даних
        /// </summary>
        /// <returns>Статистика кількості записів</returns>
        [HttpGet("stats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetDatabaseStatsAsync()
        {
            try
            {
                var stats = new
                {
                    activityTypes = await _context.ActivityTypes.CountAsync(),
                    companies = await _context.Companies.CountAsync(),
                    workers = await _context.Workers.CountAsync(),
                    vacancies = await _context.Vacancies.CountAsync(),
                    responses = await _context.Responses.CountAsync(),
                    agreements = await _context.Agreements.CountAsync()
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Помилка при отриманні статистики", error = ex.Message });
            }
        }
    }
} 
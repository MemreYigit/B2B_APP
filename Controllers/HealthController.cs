using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public HealthController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetHealth()
        {
            try
            {
                await _dbContext.Database.CanConnectAsync();
                return Ok(new { status = "healthy", database = "connected" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "unhealthy", error = ex.Message });
            }
        }
    }
}

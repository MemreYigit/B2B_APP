using Microsoft.AspNetCore.Mvc;
using EDG_B2B.Data;

namespace EDG_B2B.Controllers
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

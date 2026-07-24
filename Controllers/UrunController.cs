using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EDG_B2B.Services;

namespace EDG_B2B.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Bayi")]
    public class UrunController : ControllerBase
    {
        private readonly IUrunService _urunService;

        public UrunController(IUrunService urunService)
        {
            _urunService = urunService;
        }

        private Guid? GetKullaniciId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var id) ? id : null;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == null)
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });

            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var (success, message, items, totalCount) = await _urunService.GetAllForBayiAsync(kullaniciId.Value, search, page, pageSize);
            if (!success)
                return BadRequest(new { success, message });

            return Ok(new { TotalCount = totalCount, Data = items });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == null)
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });

            var (success, message, urun) = await _urunService.GetByIdAsync(kullaniciId.Value, id);
            if (!success)
                return NotFound(new { success, message });

            return Ok(urun);
        }
    }
}

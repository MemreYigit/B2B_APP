using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EDG_B2B.Models;
using EDG_B2B.Services;

namespace EDG_B2B.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Bayi")]
    public class SepetController : ControllerBase
    {
        private readonly ISepetService _sepetService;

        public SepetController(ISepetService sepetService)
        {
            _sepetService = sepetService;
        }

        private Guid? GetKullaniciId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var id) ? id : null;
        }

        [HttpGet]
        public async Task<IActionResult> GetSepet()
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == null)
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });

            var (success, message, sepet) = await _sepetService.GetActiveSepetAsync(kullaniciId.Value);
            if (!success)
                return BadRequest(new { success, message });

            return Ok(sepet);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddSepetItemRequest request)
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == null)
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });

            var (success, message, sepet) = await _sepetService.AddItemAsync(kullaniciId.Value, request);
            if (!success)
                return BadRequest(new { success, message });

            return Ok(sepet);
        }

        [HttpPut("items/{sepetUrunId}")]
        public async Task<IActionResult> UpdateItem(Guid sepetUrunId, [FromBody] UpdateSepetItemRequest request)
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == null)
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });

            var (success, message, sepet) = await _sepetService.UpdateItemAsync(kullaniciId.Value, sepetUrunId, request);
            if (!success)
                return BadRequest(new { success, message });

            return Ok(sepet);
        }

        [HttpDelete("items/{sepetUrunId}")]
        public async Task<IActionResult> RemoveItem(Guid sepetUrunId)
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == null)
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });

            var (success, message) = await _sepetService.RemoveItemAsync(kullaniciId.Value, sepetUrunId);
            if (!success)
                return BadRequest(new { success, message });

            return Ok(new { success, message });
        }

        [HttpDelete]
        public async Task<IActionResult> ClearSepet()
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == null)
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });

            var (success, message) = await _sepetService.ClearAsync(kullaniciId.Value);
            return Ok(new { success, message });
        }

        [HttpPost("siparis")]
        public async Task<IActionResult> PlaceOrder()
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == null)
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });

            var (success, message, siparis) = await _sepetService.PlaceOrderAsync(kullaniciId.Value);
            if (!success)
                return BadRequest(new { success, message });

            return StatusCode(201, new { success, message, siparis });
        }
    }
}

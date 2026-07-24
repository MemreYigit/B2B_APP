using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using EDG_B2B.Data;
using EDG_B2B.Models;
using EDG_B2B.Services;

namespace EDG_B2B.Controllers
{
    // Satış temsilcilerinin kendilerine bağlı bayiler adına sepet/sipariş işlemleri
    [ApiController]
    [Route("api/satisbayii")]
    [Authorize(Roles = "SatisBayii")]
    public class SatisBayiiController : ControllerBase
    {
        private readonly ISepetService _sepetService;
        private readonly AppDbContext _dbContext;

        public SatisBayiiController(ISepetService sepetService, AppDbContext dbContext)
        {
            _sepetService = sepetService;
            _dbContext = dbContext;
        }

        private Guid? GetKullaniciId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var id) ? id : null;
        }

        [HttpGet("bayiler")]
        public async Task<IActionResult> GetBayilerim()
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == null)
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });

            var satisBayii = await _dbContext.SatisBayiler.AsNoTracking().FirstOrDefaultAsync(sb => sb.KullaniciId == kullaniciId.Value);
            if (satisBayii == null)
                return BadRequest(new { message = "Satış temsilcisi kaydı bulunamadı" });

            var bayiler = await _dbContext.Bayiler
                .AsNoTracking()
                .Include(b => b.Kullanici)
                .Where(b => b.SatisBayiiId == satisBayii.Id)
                .OrderBy(b => b.Unvan)
                .Select(b => new BayiListItemDto
                {
                    Id = b.Id,
                    Unvan = b.Unvan,
                    VergiNo = b.VergiNo,
                    Email = b.Kullanici.Email,
                    Telefon = b.Kullanici.Telefon
                })
                .ToListAsync();

            return Ok(bayiler);
        }

        [HttpGet("bayiler/{bayiId}/sepet")]
        public async Task<IActionResult> GetBayiSepeti(Guid bayiId)
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == null)
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });

            var (success, message, sepet) = await _sepetService.GetBayiSepetiAsync(kullaniciId.Value, bayiId);
            if (!success)
                return BadRequest(new { success, message });

            return Ok(sepet);
        }

        [HttpPost("bayiler/{bayiId}/sepet/items")]
        public async Task<IActionResult> AddItem(Guid bayiId, [FromBody] AddSepetItemRequest request)
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == null)
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });

            var (success, message, sepet) = await _sepetService.AddItemForBayiAsync(kullaniciId.Value, bayiId, request);
            if (!success)
                return BadRequest(new { success, message });

            return Ok(sepet);
        }

        [HttpPut("bayiler/{bayiId}/sepet/items/{sepetUrunId}")]
        public async Task<IActionResult> UpdateItem(Guid bayiId, Guid sepetUrunId, [FromBody] UpdateSepetItemRequest request)
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == null)
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });

            var (success, message, sepet) = await _sepetService.UpdateBayiItemAsync(kullaniciId.Value, bayiId, sepetUrunId, request);
            if (!success)
                return BadRequest(new { success, message });

            return Ok(sepet);
        }

        [HttpDelete("bayiler/{bayiId}/sepet/items/{sepetUrunId}")]
        public async Task<IActionResult> RemoveItem(Guid bayiId, Guid sepetUrunId)
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == null)
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });

            var (success, message) = await _sepetService.RemoveBayiItemAsync(kullaniciId.Value, bayiId, sepetUrunId);
            if (!success)
                return BadRequest(new { success, message });

            return Ok(new { success, message });
        }

        [HttpDelete("bayiler/{bayiId}/sepet")]
        public async Task<IActionResult> ClearSepet(Guid bayiId)
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == null)
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });

            var (success, message) = await _sepetService.ClearBayiSepetiAsync(kullaniciId.Value, bayiId);
            if (!success)
                return BadRequest(new { success, message });

            return Ok(new { success, message });
        }

        [HttpPost("bayiler/{bayiId}/sepet/siparis")]
        public async Task<IActionResult> PlaceOrder(Guid bayiId)
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == null)
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });

            var (success, message, siparis) = await _sepetService.PlaceOrderForBayiAsync(kullaniciId.Value, bayiId);
            if (!success)
                return BadRequest(new { success, message });

            return StatusCode(201, new { success, message, siparis });
        }
    }
}

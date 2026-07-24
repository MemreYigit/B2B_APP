using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using EDG_B2B.Data;
using EDG_B2B.Entity.Enums;
using EDG_B2B.Models;
using EDG_B2B.Services;

namespace EDG_B2B.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IJwtService _jwtService;

        public AdminController(AppDbContext dbContext, IJwtService jwtService)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
        }

        // 2. PERFORMANS: Sayfalama (Pagination) eklendi
        [HttpGet("pending-users")]
        public async Task<IActionResult> GetPendingUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _dbContext.Kullanicilar
                .Include(k => k.Bayi)
                .Where(k => k.Durum == KullaniciDurumu.Beklemede);

            var totalCount = await query.CountAsync();
            var pendingUsers = await query
                .OrderByDescending(k => k.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(k => new
                {
                    k.Id,
                    k.Email,
                    k.Ad,
                    k.Soyad,
                    Unvan = k.Bayi != null ? k.Bayi.Unvan : null,
                    k.Telefon,
                    k.CreatedAt,
                    Durum = k.Durum.ToString()
                })
                .ToListAsync();

            return Ok(new { TotalCount = totalCount, Data = pendingUsers });
        }

        [HttpPost("approve-user/{userId}")]
        public async Task<IActionResult> ApproveUser(Guid userId, [FromBody] ApprovalRequest request)
        {
            var user = await _dbContext.Kullanicilar.FindAsync(userId);
            if (user == null)
                return NotFound(new { message = "Kullanıcı bulunamadı" });

            // 3. İŞ MANTIĞI: Durum kontrolü (Sadece bekleyenler onaylanabilir)
            if (user.Durum != KullaniciDurumu.Beklemede)
                return BadRequest(new { message = "Sadece bekleme durumundaki kullanıcılar onaylanabilir." });

            // 4. DENETİM: İşlemi yapan Admin ID bilgisi JWT Token'dan dinamik alınıyor
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim) || !Guid.TryParse(adminIdClaim, out Guid adminId))
            {
                return Unauthorized(new { message = "Admin kimliği doğrulanamadı." });
            }

            user.Durum = KullaniciDurumu.Onaylandi;
            user.Aktif = true;
            user.OnaylanmaTarihi = DateTime.UtcNow;
            user.OnaylayanAdminId = adminId;
            user.OnayNotu = request.Notes;

            _dbContext.Kullanicilar.Update(user);
            await _dbContext.SaveChangesAsync();

            // NOT: Burada onaylanan kullanıcıya e-posta gönderme (E-mail trigger) servisi çağrılabilir.

            return Ok(new { success = true, message = "Kullanıcı başarıyla onaylandı." });
        }

        [HttpPost("reject-user/{userId}")]
        public async Task<IActionResult> RejectUser(Guid userId, [FromBody] ApprovalRequest request)
        {
            var user = await _dbContext.Kullanicilar.FindAsync(userId);
            if (user == null)
                return NotFound(new { message = "Kullanıcı bulunamadı" });

            if (user.Durum != KullaniciDurumu.Beklemede)
                return BadRequest(new { message = "Sadece bekleme durumundaki kullanıcılar reddedilebilir." });

            user.Durum = KullaniciDurumu.Reddedildi;
            user.Aktif = false;
            user.OnayNotu = request.Notes;

            _dbContext.Kullanicilar.Update(user);
            await _dbContext.SaveChangesAsync();

            // Revoke all active sessions for this user
            await _jwtService.RevokeAllForUserAsync(userId);

            return Ok(new { success = true, message = "Kullanıcı reddedildi." });
        }

        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _dbContext.Kullanicilar.Include(k => k.Bayi);
            var totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(k => k.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(k => new
                {
                    k.Id,
                    k.Email,
                    k.Ad,
                    k.Soyad,
                    Unvan = k.Bayi != null ? k.Bayi.Unvan : null,
                    Durum = k.Durum.ToString(),
                    Rol = k.Rol.ToString(),
                    k.Aktif,
                    k.CreatedAt,
                    k.OnaylanmaTarihi
                })
                .ToListAsync();

            return Ok(new { TotalCount = totalCount, Data = users });
        }
    }
}

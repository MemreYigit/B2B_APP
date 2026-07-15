using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Entity;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // 1. GÜVENLİK: Sadece Admin rolündekiler erişebilir
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public AdminController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 2. PERFORMANS: Sayfalama (Pagination) eklendi
        [HttpGet("pending-users")]
        public async Task<IActionResult> GetPendingUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _dbContext.Users.Where(u => u.Status == UserStatus.Pending);

            var totalCount = await query.CountAsync();
            var pendingUsers = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.FullName,
                    u.CompanyName,
                    u.PhoneNumber,
                    u.CreatedAt,
                    Status = u.Status.ToString()
                })
                .ToListAsync(); // Asenkron veritabanı okuması

            return Ok(new { TotalCount = totalCount, Data = pendingUsers });
        }

        [HttpPost("approve-user/{userId}")]
        public async Task<IActionResult> ApproveUser(int userId, [FromBody] ApprovalRequest request)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { message = "Kullanıcı bulunamadı" });

            // 3. İŞ MANTIĞI: Durum kontrolü (Sadece bekleyenler onaylanabilir)
            if (user.Status != UserStatus.Pending)
                return BadRequest(new { message = "Sadece bekleme durumundaki kullanıcılar onaylanabilir." });

            // 4. DENETİM: İşlemi yapan Admin ID bilgisi JWT Token'dan dinamik alınıyor
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out int adminId))
            {
                return Unauthorized(new { message = "Admin kimliği doğrulanamadı." });
            }

            user.Status = UserStatus.Approved;
            user.ApprovedAt = DateTime.UtcNow;
            user.ApprovedByAdminId = adminId; 
            user.ApprovalNotes = request.Notes;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            // NOT: Burada onaylanan kullanıcıya e-posta gönderme (E-mail trigger) servisi çağrılabilir.

            return Ok(new { success = true, message = "Kullanıcı başarıyla onaylandı." });
        }

        [HttpPost("reject-user/{userId}")]
        public async Task<IActionResult> RejectUser(int userId, [FromBody] ApprovalRequest request)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { message = "Kullanıcı bulunamadı" });

            // 3. İŞ MANTIĞI: Durum kontrolü
            if (user.Status != UserStatus.Pending)
                return BadRequest(new { message = "Sadece bekleme durumundaki kullanıcılar reddedilebilir." });

            user.Status = UserStatus.Rejected;
            user.ApprovalNotes = request.Notes;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Kullanıcı reddedildi." });
        }

        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _dbContext.Users;
            var totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.FullName,
                    u.CompanyName,
                    Status = u.Status.ToString(),
                    Role = u.Role.ToString(),
                    u.CreatedAt,
                    u.ApprovedAt
                })
                .ToListAsync();

            return Ok(new { TotalCount = totalCount, Data = users });
        }
    }
}
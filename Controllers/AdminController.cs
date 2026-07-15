using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Entity;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public AdminController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("pending-users")]
        public IActionResult GetPendingUsers()
        {
            var pendingUsers = _dbContext.Users
                .Where(u => u.Status == UserStatus.Pending)
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
                .ToList();

            return Ok(pendingUsers);
        }

        [HttpPost("approve-user/{userId}")]
        public async Task<IActionResult> ApproveUser(int userId, [FromBody] ApprovalRequest request)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { message = "Kullanıcı bulunamadı" });

            user.Status = UserStatus.Approved;
            user.ApprovedAt = DateTime.UtcNow;
            user.ApprovedByAdminId = 1; // Admin ID buraya gelir (JWT'den alınabilir)
            user.ApprovalNotes = request.Notes;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Kullanıcı onaylandı" });
        }

        [HttpPost("reject-user/{userId}")]
        public async Task<IActionResult> RejectUser(int userId, [FromBody] ApprovalRequest request)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { message = "Kullanıcı bulunamadı" });

            user.Status = UserStatus.Rejected;
            user.ApprovalNotes = request.Notes;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Kullanıcı reddedildi" });
        }

        [HttpGet("all-users")]
        public IActionResult GetAllUsers()
        {
            var users = _dbContext.Users
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
                .ToList();

            return Ok(users);
        }
    }
}

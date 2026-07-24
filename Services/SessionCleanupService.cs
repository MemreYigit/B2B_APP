using Microsoft.EntityFrameworkCore;
using EDG_B2B.Data;

namespace EDG_B2B.Services
{
    public class SessionCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SessionCleanupService> _logger;
        private static readonly TimeSpan Interval = TimeSpan.FromHours(24);
        private static readonly TimeSpan RetentionPeriod = TimeSpan.FromDays(30);

        public SessionCleanupService(IServiceScopeFactory scopeFactory, ILogger<SessionCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cutoff = DateTime.UtcNow - RetentionPeriod;

                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var deleted = await dbContext.KullaniciOturumlari
                        .Where(s => s.ExpiresAt < cutoff || (s.IsRevoked && s.RevokedAt < cutoff))
                        .ExecuteDeleteAsync(stoppingToken);

                    if (deleted > 0)
                        _logger.LogInformation("Session cleanup: {Count} eski oturum kaydı silindi", deleted);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Session cleanup sırasında hata oluştu");
                }

                try
                {
                    await Task.Delay(Interval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                }
            }
        }
    }
}

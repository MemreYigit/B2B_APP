using Microsoft.EntityFrameworkCore;
using EDG_B2B.Data;
using EDG_B2B.Models;

namespace EDG_B2B.Services
{
    public interface IUrunService
    {
        Task<(bool Success, string Message, List<UrunListItemDto> Items, int TotalCount)> GetAllForBayiAsync(Guid kullaniciId, string? search, int page, int pageSize);
        Task<(bool Success, string Message, UrunListItemDto? Urun)> GetByIdAsync(Guid kullaniciId, Guid urunId);
    }

    public class UrunService : IUrunService
    {
        private readonly AppDbContext _dbContext;

        public UrunService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(bool Success, string Message, List<UrunListItemDto> Items, int TotalCount)> GetAllForBayiAsync(Guid kullaniciId, string? search, int page, int pageSize)
        {
            var bayi = await _dbContext.Bayiler.AsNoTracking().FirstOrDefaultAsync(b => b.KullaniciId == kullaniciId);
            if (bayi == null)
                return (false, "Bayi kaydı bulunamadı", new List<UrunListItemDto>(), 0);

            var query = _dbContext.Urunler.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(u => EF.Functions.ILike(u.Ad, $"%{term}%") || EF.Functions.ILike(u.StokKodu, $"%{term}%"));
            }

            var totalCount = await query.CountAsync();

            var now = DateTime.UtcNow;
            var bayiId = bayi.Id;

            var items = await query
                .OrderBy(u => u.Ad)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UrunListItemDto
                {
                    Id = u.Id,
                    Ad = u.Ad,
                    StokKodu = u.StokKodu,
                    StokMiktari = u.StokMiktari,
                    KdvOrani = u.KdvOrani,
                    StandartFiyat = u.StandartFiyat,
                    OzelFiyat = u.BayiUrunFiyatlari
                        .Where(f => f.BayiId == bayiId
                            && (f.GecerlilikBaslangic == null || f.GecerlilikBaslangic <= now)
                            && (f.GecerlilikBitis == null || f.GecerlilikBitis >= now))
                        .Select(f => (decimal?)f.OzelFiyat)
                        .FirstOrDefault()
                })
                .ToListAsync();

            foreach (var item in items)
                item.Fiyat = item.OzelFiyat ?? item.StandartFiyat;

            return (true, "OK", items, totalCount);
        }

        public async Task<(bool Success, string Message, UrunListItemDto? Urun)> GetByIdAsync(Guid kullaniciId, Guid urunId)
        {
            var bayi = await _dbContext.Bayiler.AsNoTracking().FirstOrDefaultAsync(b => b.KullaniciId == kullaniciId);
            if (bayi == null)
                return (false, "Bayi kaydı bulunamadı", null);

            var now = DateTime.UtcNow;
            var bayiId = bayi.Id;

            var item = await _dbContext.Urunler
                .AsNoTracking()
                .Where(u => u.Id == urunId)
                .Select(u => new UrunListItemDto
                {
                    Id = u.Id,
                    Ad = u.Ad,
                    StokKodu = u.StokKodu,
                    StokMiktari = u.StokMiktari,
                    KdvOrani = u.KdvOrani,
                    StandartFiyat = u.StandartFiyat,
                    OzelFiyat = u.BayiUrunFiyatlari
                        .Where(f => f.BayiId == bayiId
                            && (f.GecerlilikBaslangic == null || f.GecerlilikBaslangic <= now)
                            && (f.GecerlilikBitis == null || f.GecerlilikBitis >= now))
                        .Select(f => (decimal?)f.OzelFiyat)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (item == null)
                return (false, "Ürün bulunamadı", null);

            item.Fiyat = item.OzelFiyat ?? item.StandartFiyat;

            return (true, "OK", item);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entity;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IPriceResolver
    {
        Task<FiyatDto> ResolveAsync(Urun urun, int cariId);
    }

    // Bir ürünün, belirli bir cari için geçerli fiyatını hesaplar:
    //  - CariUrunFiyat tablosunda cari+ürün için (geçerlilik tarihleri uyan) bir kayıt varsa
    //    ve ListeFiyati'ndan farklıysa, hem listeFiyati hem cariyeOzelFiyat JSON'da döner.
    //  - Kayıt yoksa veya ListeFiyati ile aynıysa, tek bir "fiyat" alanı döner.
    public class PriceResolver : IPriceResolver
    {
        private readonly AppDbContext _dbContext;

        public PriceResolver(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<FiyatDto> ResolveAsync(Urun urun, int cariId)
        {
            var now = DateTime.UtcNow;

            var ozelFiyat = await _dbContext.CariUrunFiyatlari
                .Where(f => f.CariId == cariId && f.UrunId == urun.Id)
                .Where(f => (f.GecerlilikBaslangic == null || f.GecerlilikBaslangic <= now)
                         && (f.GecerlilikBitis == null || f.GecerlilikBitis >= now))
                .OrderByDescending(f => f.GecerlilikBaslangic)
                .Select(f => (decimal?)f.OzelFiyat)
                .FirstOrDefaultAsync();

            if (ozelFiyat is null || ozelFiyat == urun.ListeFiyati)
            {
                return new FiyatDto { Fiyat = urun.ListeFiyati };
            }

            return new FiyatDto
            {
                ListeFiyati = urun.ListeFiyati,
                CariyeOzelFiyat = ozelFiyat
            };
        }
    }
}

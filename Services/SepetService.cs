using Microsoft.EntityFrameworkCore;
using EDG_B2B.Data;
using EDG_B2B.Entity;
using EDG_B2B.Entity.Enums;
using EDG_B2B.Models;

namespace EDG_B2B.Services
{
    public interface ISepetService
    {
        // Bayinin kendi sepeti üzerindeki işlemleri
        Task<(bool Success, string Message, SepetDto? Sepet)> GetActiveSepetAsync(Guid kullaniciId);
        Task<(bool Success, string Message, SepetDto? Sepet)> AddItemAsync(Guid kullaniciId, AddSepetItemRequest request);
        Task<(bool Success, string Message, SepetDto? Sepet)> UpdateItemAsync(Guid kullaniciId, Guid sepetUrunId, UpdateSepetItemRequest request);
        Task<(bool Success, string Message)> RemoveItemAsync(Guid kullaniciId, Guid sepetUrunId);
        Task<(bool Success, string Message)> ClearAsync(Guid kullaniciId);
        Task<(bool Success, string Message, SiparisDto? Siparis)> PlaceOrderAsync(Guid kullaniciId);

        // Satış temsilcisinin kendisine bağlı bir bayi adına yaptığı işlemler
        Task<(bool Success, string Message, SepetDto? Sepet)> GetBayiSepetiAsync(Guid satisBayiiKullaniciId, Guid bayiId);
        Task<(bool Success, string Message, SepetDto? Sepet)> AddItemForBayiAsync(Guid satisBayiiKullaniciId, Guid bayiId, AddSepetItemRequest request);
        Task<(bool Success, string Message, SepetDto? Sepet)> UpdateBayiItemAsync(Guid satisBayiiKullaniciId, Guid bayiId, Guid sepetUrunId, UpdateSepetItemRequest request);
        Task<(bool Success, string Message)> RemoveBayiItemAsync(Guid satisBayiiKullaniciId, Guid bayiId, Guid sepetUrunId);
        Task<(bool Success, string Message)> ClearBayiSepetiAsync(Guid satisBayiiKullaniciId, Guid bayiId);
        Task<(bool Success, string Message, SiparisDto? Siparis)> PlaceOrderForBayiAsync(Guid satisBayiiKullaniciId, Guid bayiId);
    }

    public class SepetService : ISepetService
    {
        private readonly AppDbContext _dbContext;

        public SepetService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(bool Success, string Message, SepetDto? Sepet)> GetActiveSepetAsync(Guid kullaniciId)
        {
            var bayiId = await ResolveOwnBayiIdAsync(kullaniciId);
            if (bayiId == null)
                return (false, "Bayi kaydı bulunamadı", null);

            return await GetActiveSepetCoreAsync(bayiId.Value);
        }

        public async Task<(bool Success, string Message, SepetDto? Sepet)> AddItemAsync(Guid kullaniciId, AddSepetItemRequest request)
        {
            var bayiId = await ResolveOwnBayiIdAsync(kullaniciId);
            if (bayiId == null)
                return (false, "Bayi kaydı bulunamadı", null);

            return await AddItemCoreAsync(bayiId.Value, request);
        }

        public async Task<(bool Success, string Message, SepetDto? Sepet)> UpdateItemAsync(Guid kullaniciId, Guid sepetUrunId, UpdateSepetItemRequest request)
        {
            var bayiId = await ResolveOwnBayiIdAsync(kullaniciId);
            if (bayiId == null)
                return (false, "Bayi kaydı bulunamadı", null);

            return await UpdateItemCoreAsync(bayiId.Value, sepetUrunId, request);
        }

        public async Task<(bool Success, string Message)> RemoveItemAsync(Guid kullaniciId, Guid sepetUrunId)
        {
            var bayiId = await ResolveOwnBayiIdAsync(kullaniciId);
            if (bayiId == null)
                return (false, "Bayi kaydı bulunamadı");

            return await RemoveItemCoreAsync(bayiId.Value, sepetUrunId);
        }

        public async Task<(bool Success, string Message)> ClearAsync(Guid kullaniciId)
        {
            var bayiId = await ResolveOwnBayiIdAsync(kullaniciId);
            if (bayiId == null)
                return (false, "Bayi kaydı bulunamadı");

            return await ClearCoreAsync(bayiId.Value);
        }

        public async Task<(bool Success, string Message, SiparisDto? Siparis)> PlaceOrderAsync(Guid kullaniciId)
        {
            var bayiId = await ResolveOwnBayiIdAsync(kullaniciId);
            if (bayiId == null)
                return (false, "Bayi kaydı bulunamadı", null);

            return await PlaceOrderCoreAsync(bayiId.Value);
        }

        public async Task<(bool Success, string Message, SepetDto? Sepet)> GetBayiSepetiAsync(Guid satisBayiiKullaniciId, Guid bayiId)
        {
            var (authorized, message) = await AuthorizeSatisBayiiForBayiAsync(satisBayiiKullaniciId, bayiId);
            if (!authorized)
                return (false, message, null);

            return await GetActiveSepetCoreAsync(bayiId);
        }

        public async Task<(bool Success, string Message, SepetDto? Sepet)> AddItemForBayiAsync(Guid satisBayiiKullaniciId, Guid bayiId, AddSepetItemRequest request)
        {
            var (authorized, message) = await AuthorizeSatisBayiiForBayiAsync(satisBayiiKullaniciId, bayiId);
            if (!authorized)
                return (false, message, null);

            return await AddItemCoreAsync(bayiId, request);
        }

        public async Task<(bool Success, string Message, SepetDto? Sepet)> UpdateBayiItemAsync(Guid satisBayiiKullaniciId, Guid bayiId, Guid sepetUrunId, UpdateSepetItemRequest request)
        {
            var (authorized, message) = await AuthorizeSatisBayiiForBayiAsync(satisBayiiKullaniciId, bayiId);
            if (!authorized)
                return (false, message, null);

            return await UpdateItemCoreAsync(bayiId, sepetUrunId, request);
        }

        public async Task<(bool Success, string Message)> RemoveBayiItemAsync(Guid satisBayiiKullaniciId, Guid bayiId, Guid sepetUrunId)
        {
            var (authorized, message) = await AuthorizeSatisBayiiForBayiAsync(satisBayiiKullaniciId, bayiId);
            if (!authorized)
                return (false, message);

            return await RemoveItemCoreAsync(bayiId, sepetUrunId);
        }

        public async Task<(bool Success, string Message)> ClearBayiSepetiAsync(Guid satisBayiiKullaniciId, Guid bayiId)
        {
            var (authorized, message) = await AuthorizeSatisBayiiForBayiAsync(satisBayiiKullaniciId, bayiId);
            if (!authorized)
                return (false, message);

            return await ClearCoreAsync(bayiId);
        }

        public async Task<(bool Success, string Message, SiparisDto? Siparis)> PlaceOrderForBayiAsync(Guid satisBayiiKullaniciId, Guid bayiId)
        {
            var (authorized, message) = await AuthorizeSatisBayiiForBayiAsync(satisBayiiKullaniciId, bayiId);
            if (!authorized)
                return (false, message, null);

            return await PlaceOrderCoreAsync(bayiId);
        }

        private async Task<Guid?> ResolveOwnBayiIdAsync(Guid kullaniciId)
        {
            var bayi = await _dbContext.Bayiler.AsNoTracking().FirstOrDefaultAsync(b => b.KullaniciId == kullaniciId);
            return bayi?.Id;
        }

        // Talep eden kullanıcının, ilgili bayiye atanmış satış temsilcisi olduğunu doğrular
        private async Task<(bool Authorized, string Message)> AuthorizeSatisBayiiForBayiAsync(Guid satisBayiiKullaniciId, Guid bayiId)
        {
            var satisBayii = await _dbContext.SatisBayiler.AsNoTracking().FirstOrDefaultAsync(sb => sb.KullaniciId == satisBayiiKullaniciId);
            if (satisBayii == null)
                return (false, "Satış temsilcisi kaydı bulunamadı");

            var bayiBagli = await _dbContext.Bayiler.AsNoTracking().AnyAsync(b => b.Id == bayiId && b.SatisBayiiId == satisBayii.Id);
            if (!bayiBagli)
                return (false, "Bu bayi size bağlı değil veya bulunamadı");

            return (true, "OK");
        }

        private async Task<(bool Success, string Message, SepetDto? Sepet)> GetActiveSepetCoreAsync(Guid bayiId)
        {
            var sepet = await _dbContext.Sepetler
                .AsNoTracking()
                .Include(s => s.SepetUrunleri)
                    .ThenInclude(su => su.Urun)
                .FirstOrDefaultAsync(s => s.BayiId == bayiId && s.Durum == SepetDurumu.Aktif);

            return (true, "OK", MapToDto(sepet));
        }

        private async Task<(bool Success, string Message, SepetDto? Sepet)> AddItemCoreAsync(Guid bayiId, AddSepetItemRequest request)
        {
            if (request.Miktar <= 0)
                return (false, "Miktar sıfırdan büyük olmalıdır", null);

            var urun = await _dbContext.Urunler.FirstOrDefaultAsync(u => u.Id == request.UrunId);
            if (urun == null)
                return (false, "Ürün bulunamadı", null);

            var sepet = await _dbContext.Sepetler
                .Include(s => s.SepetUrunleri)
                .FirstOrDefaultAsync(s => s.BayiId == bayiId && s.Durum == SepetDurumu.Aktif);

            if (sepet == null)
            {
                sepet = new Sepet { Id = Guid.NewGuid(), BayiId = bayiId };
                _dbContext.Sepetler.Add(sepet);
            }

            var mevcutSatir = sepet.SepetUrunleri.FirstOrDefault(su => su.UrunId == request.UrunId);
            if (mevcutSatir != null)
            {
                mevcutSatir.Miktar += request.Miktar;
            }
            else
            {
                var now = DateTime.UtcNow;
                var ozelFiyat = await _dbContext.BayiUrunFiyatlari
                    .Where(f => f.BayiId == bayiId && f.UrunId == urun.Id
                        && (f.GecerlilikBaslangic == null || f.GecerlilikBaslangic <= now)
                        && (f.GecerlilikBitis == null || f.GecerlilikBitis >= now))
                    .Select(f => (decimal?)f.OzelFiyat)
                    .FirstOrDefaultAsync();

                sepet.SepetUrunleri.Add(new SepetUrun
                {
                    Id = Guid.NewGuid(),
                    SepetId = sepet.Id,
                    UrunId = urun.Id,
                    Miktar = request.Miktar,
                    BirimFiyat = ozelFiyat ?? urun.StandartFiyat
                });
            }

            await _dbContext.SaveChangesAsync();

            return await GetActiveSepetCoreAsync(bayiId);
        }

        private async Task<(bool Success, string Message, SepetDto? Sepet)> UpdateItemCoreAsync(Guid bayiId, Guid sepetUrunId, UpdateSepetItemRequest request)
        {
            if (request.Miktar <= 0)
                return (false, "Miktar sıfırdan büyük olmalıdır", null);

            var satir = await _dbContext.SepetUrunleri
                .Include(su => su.Sepet)
                .FirstOrDefaultAsync(su => su.Id == sepetUrunId && su.Sepet.BayiId == bayiId && su.Sepet.Durum == SepetDurumu.Aktif);

            if (satir == null)
                return (false, "Sepet ürünü bulunamadı", null);

            satir.Miktar = request.Miktar;
            await _dbContext.SaveChangesAsync();

            return await GetActiveSepetCoreAsync(bayiId);
        }

        private async Task<(bool Success, string Message)> RemoveItemCoreAsync(Guid bayiId, Guid sepetUrunId)
        {
            var satir = await _dbContext.SepetUrunleri
                .Include(su => su.Sepet)
                .FirstOrDefaultAsync(su => su.Id == sepetUrunId && su.Sepet.BayiId == bayiId && su.Sepet.Durum == SepetDurumu.Aktif);

            if (satir == null)
                return (false, "Sepet ürünü bulunamadı");

            _dbContext.SepetUrunleri.Remove(satir);
            await _dbContext.SaveChangesAsync();

            return (true, "Ürün sepetten kaldırıldı");
        }

        private async Task<(bool Success, string Message)> ClearCoreAsync(Guid bayiId)
        {
            var sepet = await _dbContext.Sepetler
                .Include(s => s.SepetUrunleri)
                .FirstOrDefaultAsync(s => s.BayiId == bayiId && s.Durum == SepetDurumu.Aktif);

            if (sepet == null)
                return (true, "Sepet zaten boş");

            _dbContext.SepetUrunleri.RemoveRange(sepet.SepetUrunleri);
            await _dbContext.SaveChangesAsync();

            return (true, "Sepet temizlendi");
        }

        private async Task<(bool Success, string Message, SiparisDto? Siparis)> PlaceOrderCoreAsync(Guid bayiId)
        {
            var bayi = await _dbContext.Bayiler.AsNoTracking().FirstOrDefaultAsync(b => b.Id == bayiId);
            if (bayi == null)
                return (false, "Bayi kaydı bulunamadı", null);

            if (bayi.SatisBayiiId == null)
                return (false, "Sipariş verebilmek için bayinize atanmış bir satış temsilcisi olmalıdır. Lütfen yönetici ile iletişime geçin.", null);

            var sepet = await _dbContext.Sepetler
                .Include(s => s.SepetUrunleri)
                    .ThenInclude(su => su.Urun)
                .FirstOrDefaultAsync(s => s.BayiId == bayiId && s.Durum == SepetDurumu.Aktif);

            if (sepet == null || sepet.SepetUrunleri.Count == 0)
                return (false, "Sepet boş", null);

            foreach (var satir in sepet.SepetUrunleri)
            {
                if (satir.Miktar > satir.Urun.StokMiktari)
                    return (false, $"'{satir.Urun.Ad}' için yeterli stok bulunmamaktadır.", null);
            }

            var siparis = new Siparis
            {
                Id = Guid.NewGuid(),
                SepetId = sepet.Id,
                OnaylayanSatisBayiiId = bayi.SatisBayiiId.Value,
                Durum = SiparisDurumu.Onaylandi
            };

            var siparisItems = new List<SiparisItemDto>();
            foreach (var satir in sepet.SepetUrunleri)
            {
                var siparisUrun = new SiparisUrun
                {
                    Id = Guid.NewGuid(),
                    SiparisId = siparis.Id,
                    UrunId = satir.UrunId,
                    Miktar = satir.Miktar,
                    BirimFiyat = satir.BirimFiyat,
                    SatirToplam = satir.Miktar * satir.BirimFiyat
                };
                siparis.SiparisUrunleri.Add(siparisUrun);

                siparisItems.Add(new SiparisItemDto
                {
                    Id = siparisUrun.Id,
                    UrunId = satir.UrunId,
                    UrunAdi = satir.Urun.Ad,
                    StokKodu = satir.Urun.StokKodu,
                    Miktar = satir.Miktar,
                    BirimFiyat = satir.BirimFiyat,
                    SatirToplam = siparisUrun.SatirToplam
                });
            }

            sepet.Durum = SepetDurumu.Onaylandi;
            _dbContext.Siparisler.Add(siparis);

            await _dbContext.SaveChangesAsync();

            var dto = new SiparisDto
            {
                Id = siparis.Id,
                Durum = siparis.Durum.ToString(),
                CreatedAt = siparis.CreatedAt,
                Urunler = siparisItems,
                ToplamTutar = siparisItems.Sum(i => i.SatirToplam)
            };

            return (true, "Sipariş başarıyla oluşturuldu", dto);
        }

        private static SepetDto MapToDto(Sepet? sepet)
        {
            if (sepet == null)
            {
                return new SepetDto
                {
                    Id = null,
                    Durum = SepetDurumu.Aktif.ToString(),
                    Urunler = new List<SepetItemDto>(),
                    ToplamTutar = 0
                };
            }

            var items = sepet.SepetUrunleri.Select(su => new SepetItemDto
            {
                Id = su.Id,
                UrunId = su.UrunId,
                UrunAdi = su.Urun.Ad,
                StokKodu = su.Urun.StokKodu,
                Miktar = su.Miktar,
                BirimFiyat = su.BirimFiyat,
                SatirToplam = su.Miktar * su.BirimFiyat
            }).ToList();

            return new SepetDto
            {
                Id = sepet.Id,
                Durum = sepet.Durum.ToString(),
                Urunler = items,
                ToplamTutar = items.Sum(i => i.SatirToplam)
            };
        }
    }
}

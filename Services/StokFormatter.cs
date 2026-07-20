namespace WebApplication1.Services
{
    // StokMiktari 50'den büyükse JSON yanıtında "50+" (string), değilse
    // gerçek adet (number) olarak dönmesini sağlayan yardımcı sınıf.
    public static class StokFormatter
    {
        private const int Esik = 50;

        public static object Formatla(int stokMiktari)
        {
            return stokMiktari > Esik ? "50+" : stokMiktari;
        }
    }
}

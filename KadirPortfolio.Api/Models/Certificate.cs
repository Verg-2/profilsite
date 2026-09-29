namespace KadirPortfolio.Api.Models
{
    public class Certificate
    {
        public int Id { get; set; }
        // Sertifikanın adı
        public string Name { get; set; }=string.Empty;
        // Sertifikayı ne zaman aldığın (Metin veya Tarih olabilir, esneklik için string yapıyoruz)
        public string Date{get;set;}=string.Empty;
        //Serfitikanın görsel linki
        public string ImageUrl{get;set;}=string.Empty;
        //Sertifikanın açıklaması
        public string Description{get;set;}=string.Empty;
        //Sertifikanın sıralaması
        public int Order{get;set;}=0;

        // İngilizce çeviriler için
        public string NameEn { get; set; } = string.Empty;
        public string DateEn { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;

    }
}
using System.ComponentModel.DataAnnotations;

namespace KadirPortfolio.Api.Models
{
    public class IletisimMesaji
    {
        public int Id { get; set; }
        public DateTime GonderimTarihi { get; set; }

        [Required(ErrorMessage = "İsim zorunludur.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "İsim 2-50 karakter arasında olmalıdır.")]
        [RegularExpression(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s'-]+$", ErrorMessage = "İsim geçersiz karakter içeriyor.")]
        public string? Ad { get; set; }

        [Required(ErrorMessage = "Soyisim zorunludur.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Soyisim 2-50 karakter arasında olmalıdır.")]
        [RegularExpression(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s'-]+$", ErrorMessage = "Soyisim geçersiz karakter içeriyor.")]
        public string? Soyad { get; set; }

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Mesaj zorunludur.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Mesaj 10-500 karakter arasında olmalıdır.")]
        public string? Mesaj { get; set; }

        public string? WebSitesi { get; set; }
        // Yeni: Soft Delete için
        public bool IsDeleted { get; set; } = false;
    }
}
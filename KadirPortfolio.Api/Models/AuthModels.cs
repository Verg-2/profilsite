using System.ComponentModel.DataAnnotations;

namespace KadirPortfolio.Api.Models
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        
        public string? CaptchaToken { get; set; }
        
        public string? UsernameHoneypot { get; set; }
        
        public bool RememberMe { get; set; }
    }

    public class Verify2FaRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Code { get; set; }
        
        public bool RememberMe { get; set; }
    }
}

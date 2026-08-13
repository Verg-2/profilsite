namespace KadirPortfolio.Api.Services
{
    public interface ICaptchaService
    {
        Task<bool> VerifyCaptchaAsync(string token);
    }
}

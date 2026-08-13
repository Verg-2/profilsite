using System.Threading.Tasks;

namespace KadirPortfolio.Api.Services
{
    public interface IAiTranslationService
    {
        Task<string> TranslateAsync(string text, string targetLanguage = "English", string section = "Genel");
        Task<string> RefineTranslationAsync(string text, string existingTranslation, string targetLanguage = "English", string section = "Genel", string? userHint = null);
        Task<string> AnalyzeCodeSecurityAsync(string code);
    }
}

using KadirPortfolio.Api.Models;

namespace KadirPortfolio.Api.Services
{
    public interface ITelegramService
    {
        Task<string> MesajGonderAsync(IletisimMesaji model);
    }
}
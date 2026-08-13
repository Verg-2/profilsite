using KadirPortfolio.Api.Models;
using System.Threading.Tasks;

namespace KadirPortfolio.Api.Services
{
    public interface IScannerService
    {
        Task<ScanReport> RunFullScanAsync();
    }
}

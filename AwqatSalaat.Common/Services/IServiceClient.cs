using System.Threading.Tasks;

namespace AwqatSalaat.Services
{
    public interface IServiceClient
    {
        bool SupportMonthlyData { get; }
        Task<ServiceData> GetDataAsync(IRequest request);
    }
}
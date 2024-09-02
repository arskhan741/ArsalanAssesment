using ArsalanAssesment.Web.DTOs;

namespace ArsalanAssesment.Web.Repository.Contracts
{
    public interface IDashBoardMetricsRepository
    {
        Task<ResponseDTO> GetSaleMetrics();
    }
}

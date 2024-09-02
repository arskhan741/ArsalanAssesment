using ArsalanAssesment.Web.Data;
using ArsalanAssesment.Web.DTOs;
using ArsalanAssesment.Web.Helper;
using ArsalanAssesment.Web.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ArsalanAssesment.Web.Repository
{
    public class DashBoardMetricsRepository : IDashBoardMetricsRepository
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<DashBoardMetricsRepository> _logger;

        public DashBoardMetricsRepository(ApplicationDBContext dbContext, ILogger<DashBoardMetricsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }


        public async Task<ResponseDTO> GetSaleMetrics()
        {
            try
            {
                // Find All Sales from DB, no tracking for optimizations
                var sales = await _dbContext.Sales.AsNoTracking().ToListAsync();

                if (sales.Count <= 0)
                {
                    // If no sales are found we return with a message
                    return ResponseHelper.CreateResponse(true, false, ResponseMessages.NoSalesFound);
                }

                //Create new object of SaleMetrics
                SalesMetrics metrics = new SalesMetrics();

                //Perform required calculations
                metrics.TotalSales = sales.Count;
                metrics.TotalAmount = sales.Sum(s => s.Amount);
                metrics.TotalSalesAverage = sales.Average(s => s.Amount);

                return ResponseHelper.CreateResponse(true, false, ResponseMessages.Successful, metrics);
            }
            catch (Exception ex)
            {
                // Log Error Message
                _logger.LogError(ex, "An error occurred while processing the request.");

                // Create response for failure, Show simple error message and return it
                return ResponseHelper.CreateResponse(false, true, ResponseMessages.ExceptionMessage);
            }
        }


        public class SalesMetrics
        {
            public int TotalSales { get; set; }
            public decimal TotalSalesAverage { get; set; }
            public decimal TotalAmount { get; set; }
        }
    }


}

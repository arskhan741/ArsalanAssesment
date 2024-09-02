using ArsalanAssesment.Web.Controllers;
using ArsalanAssesment.Web.DTOs;
using ArsalanAssesment.Web.Helper;
using ArsalanAssesment.Web.Models;
using ArsalanAssesment.Web.Repository.Contracts;
using AutoFixture;
using Moq;
using System.Net;
using static ArsalanAssesment.Web.Repository.DashBoardMetricsRepository;

namespace ArsalanAssesment.UnitTesting.MetricsAPITest
{
    public class MetricsControllerTestCases
    {
        private readonly Fixture _fixture;
        private readonly Mock<IDashBoardMetricsRepository> _dashBoardRepo;
        private readonly MetricsController _metricsController;

        public MetricsControllerTestCases()
        {
            _fixture = new Fixture();
            _dashBoardRepo = new Mock<IDashBoardMetricsRepository>();
            _metricsController = new MetricsController(_dashBoardRepo.Object);
        }

        [Fact]
        public async Task Get_SaleMetrics_ReturnsOkResult_WithCorrectData()
        {
            // Create thre sales to apply calculations on them
            var sales = _fixture.CreateMany<Sale>(3).ToList();

            // Create SaleMetrics, no need to mock it since the values are fixed after calculations
            var salesMetrics = new SalesMetrics()
            {
                TotalSales = sales.Count,
                TotalAmount = sales.Sum(x => x.Amount),
                TotalSalesAverage = sales.Average(x => x.Amount)
            };

            // Create Response which we expect from controller, and pass the expected parameters
            var expectedResponse = ResponseHelper.CreateFakeResponse(ResponseMessages.Successful, HttpStatusCode.OK, salesMetrics);

            // Set up the mock repository to return the expected response
            _dashBoardRepo.Setup(repo => repo.GetSaleMetrics()).ReturnsAsync(expectedResponse);

            //Get Result From Controller
            ResponseDTO result = await _metricsController.Get();

            var saleMatriceFromController = result.Result as SalesMetrics;


            // Assetions
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, expectedResponse.StatusCode);
            Assert.Equal(ResponseMessages.Successful, expectedResponse.Message);
            Assert.True(expectedResponse.IsSuccess);
            Assert.False(expectedResponse.IsException);

            // Verify calculations
            Assert.Equal(saleMatriceFromController?.TotalAmount, salesMetrics.TotalAmount);
            Assert.Equal(saleMatriceFromController?.TotalSales, salesMetrics.TotalSales);
            Assert.Equal(saleMatriceFromController?.TotalSalesAverage, salesMetrics.TotalSalesAverage);
        }

    }
}

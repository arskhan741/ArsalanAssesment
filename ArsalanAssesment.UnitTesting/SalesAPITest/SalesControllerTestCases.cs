using ArsalanAssesment.Web.Controllers;
using ArsalanAssesment.Web.DTOs;
using ArsalanAssesment.Web.DTOs.SaleDTOs;
using ArsalanAssesment.Web.Helper;
using ArsalanAssesment.Web.Repository.Contracts;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace ArsalanAssesment.UnitTesting.SalesAPITest
{
    public class SalesControllerTestCases
    {
        private readonly Mock<ISaleRepository> _salesRepo;
        private readonly Fixture _fixture;
        private readonly SalesController _salesController;  

        public SalesControllerTestCases()
        {
            _salesRepo = new Mock<ISaleRepository>();
            _fixture = new Fixture();
            _salesController = new SalesController(_salesRepo.Object);

            // Customize AutoFixture to generate decimal values within a specific range for both CreateSaleDTO and UpdateSaleDTO
            _fixture.Customize<CreateSaleDTO>(c => c
                .With(dto => dto.Amount, _fixture.Create<decimal>() % 1000m)); // Limit to a range
            _fixture.Customize<UpdateSaleDTO>(c => c
                .With(dto => dto.Amount, _fixture.Create<decimal>() % 1000m)); // Limit to a range
        }

        [Fact]
        public async Task Get_Sales_ReturnsOkResult_WithCorrectData()
        {
            // Arrange
            var saleId = 1;

            // Create a fake GetSaleDTO
            var saleDTO = _fixture.Create<GetSaleDTO>();

            // Create Response which we expect from controller, and pass the expected parameters
            var expectedResponse = ResponseHelper.CreateFakeResponse(ResponseMessages.Successful, HttpStatusCode.OK, saleDTO);

            // Set up the mock repository to return the expected response
            _salesRepo.Setup(repo => repo.GetAsync(saleId)).ReturnsAsync(expectedResponse);

            // Get Result From Controller
            var result = await _salesController.Get(saleId);

            // Assetions
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, expectedResponse.StatusCode);
            Assert.Equal(ResponseMessages.Successful, expectedResponse.Message);
            Assert.True(expectedResponse.IsSuccess);
            Assert.False(expectedResponse.IsException);
        }

        [Fact]
        public async Task Get_All_Sales_ReturnsOkResult_WithCorrectData()
        {
            // Create Fake List
            var saleDTOList = _fixture.CreateMany<GetSaleDTO>(3).ToList();

            // Create Response which we expect from controller, and pass the expected parameters
            var expectedResponse = ResponseHelper.CreateFakeResponse(ResponseMessages.Successful, HttpStatusCode.OK, saleDTOList);

            // Set up the mock repository to return the expected response
            _salesRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedResponse);

            // Get Result From Controller
            var result = await _salesController.GetAll();

            // Assetions
            Assert.NotNull(result);

            Assert.Equal((int)HttpStatusCode.OK, expectedResponse.StatusCode);
            Assert.Equal(ResponseMessages.Successful, expectedResponse.Message);
            Assert.True(expectedResponse.IsSuccess);
            Assert.False(expectedResponse.IsException);

            // Check if the list contains 3 items
            var actualList = expectedResponse.Result as List<GetSaleDTO>;
            Assert.NotNull(actualList);
            Assert.Equal(3, actualList.Count);
        }

        [Fact]
        public async Task Post_Add_Sales_ReturnsOkResult_WithCorrectData()
        {
            // Create a fake Create SaleDTO
            var createSaleDTO = _fixture.Create<CreateSaleDTO>();

            // Create a fake GetSaleDTO
            var saleDTO = _fixture.Create<GetSaleDTO>();

            // Create Response which we expect from controller, and pass the expected parameters
            var expectedResponse = ResponseHelper.CreateFakeResponse(ResponseMessages.Successful, HttpStatusCode.Created, saleDTO);

            // Set up the mock repository to return the expected response
            _salesRepo.Setup(repo => repo.CreateAsync(createSaleDTO)).ReturnsAsync(expectedResponse);

            // Get Result From Controller
            var result = await _salesController.Add(createSaleDTO);

            // Assetions
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Created, expectedResponse.StatusCode);
            Assert.Equal(ResponseMessages.Successful, expectedResponse.Message);
            Assert.True(expectedResponse.IsSuccess);
            Assert.False(expectedResponse.IsException);
        }

        [Fact]
        public async Task Put_Sales_ReturnsOkResult_WithCorrectData()
        {
            // Arrange
            var saleId = 1;

            // Create a fake Create SaleDTO
            var updateSaleDTO = _fixture.Create<UpdateSaleDTO>();

            // Create a fake GetSaleDTO
            var saleDTO = _fixture.Create<GetSaleDTO>();

            // Create Response which we expect from controller, and pass the expected parameters
            var expectedResponse = ResponseHelper.CreateFakeResponse(ResponseMessages.Modified, HttpStatusCode.Created, saleDTO);

            // Set up the mock repository to return the expected response
            _salesRepo.Setup(repo => repo.UpdateAsync(saleId, updateSaleDTO)).ReturnsAsync(expectedResponse);

            // Get Result From Controller
            var result = await _salesController.Update(saleId, updateSaleDTO);

            // Assetions
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, expectedResponse.StatusCode);
            Assert.Equal(ResponseMessages.Modified, expectedResponse.Message);
            Assert.True(expectedResponse.IsSuccess);
            Assert.False(expectedResponse.IsException);
        }

        [Fact]
        public async Task Delete_Sales_ReturnsOkResult_WithCorrectData()
        {
            var saleId = 1;

            // Create a fake GetSaleDTO
            var saleDTO = _fixture.Create<GetSaleDTO>();

            // Create Response which we expect from controller, and pass the expected parameters
            var expectedResponse = ResponseHelper.CreateFakeResponse(ResponseMessages.Deleted, HttpStatusCode.NoContent, saleDTO);

            // Set up the mock repository to return the expected response
            _salesRepo.Setup(repo => repo.DeleteAsync(saleId)).ReturnsAsync(expectedResponse);

            // Get Result From Controller
            var result = await _salesController.Delete(saleId);

            // Assetions
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NoContent, expectedResponse.StatusCode);
            Assert.Equal(ResponseMessages.Deleted, expectedResponse.Message);
            Assert.True(expectedResponse.IsSuccess);
            Assert.False(expectedResponse.IsException);
        }

        [Fact]
        public async Task GetByFilters_Sales_ReturnsOkResult_WithCorrectData()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow;
            var representativeId = 1;

            // Create a list of GetSaleDTO with dates within the range
            var saleDTOList = new List<GetSaleDTO>
            {
                new GetSaleDTO { SaleDate = DateTime.UtcNow.AddDays(-10), RepresentativeID = representativeId, Amount = 100 },
                new GetSaleDTO { SaleDate = DateTime.UtcNow.AddDays(-20), RepresentativeID = representativeId, Amount = 200 },
                new GetSaleDTO { SaleDate = DateTime.UtcNow.AddDays(-5), RepresentativeID = representativeId, Amount = 300 }
            };

            // Create Response which we expect from the controller
            var expectedResponse = ResponseHelper.CreateFakeResponse(ResponseMessages.Successful, HttpStatusCode.OK, saleDTOList);

            // Set up the mock repository to return the expected response
            _salesRepo.Setup(repo => repo.GetSalesByFiltersAsync(startDate, endDate, representativeId)).ReturnsAsync(expectedResponse);

            // Get Result From Controller
            var result = await _salesController.GetByFilters(startDate, endDate, representativeId);

            // Assertions
            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseDTO>(okResult.Value);

            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(ResponseMessages.Successful, response.Message);
            Assert.True(response.IsSuccess);
            Assert.False(response.IsException);

            // Check if the list contains 3 items
            var actualList = response.Result as List<GetSaleDTO>;
            Assert.NotNull(actualList);
            Assert.Equal(3, actualList.Count);
        }

    }
}

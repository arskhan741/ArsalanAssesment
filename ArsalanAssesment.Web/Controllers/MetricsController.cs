using ArsalanAssesment.Web.DTOs;
using ArsalanAssesment.Web.Repository.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ArsalanAssesment.Web.Controllers
{
    [ApiController]
    [Route("api/metrics/")]
    [Authorize(Roles = "manager")]
    public class MetricsController : ControllerBase
    {
        private readonly IDashBoardMetricsRepository _boardMetricsRepository;

        public MetricsController(IDashBoardMetricsRepository boardMetricsRepository)
        {
            _boardMetricsRepository = boardMetricsRepository;
        }

        [HttpGet("sales-summary")]
        public async Task<ResponseDTO> Get()
        {
            var response = await _boardMetricsRepository.GetSaleMetrics();

            response.StatusCode = (response.Result is null) ? (int)HttpStatusCode.NotFound : (int)HttpStatusCode.OK;

            return response;
        }
    }
}

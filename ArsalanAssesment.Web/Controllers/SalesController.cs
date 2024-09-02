using ArsalanAssesment.Web.DTOs.SaleDTOs;
using ArsalanAssesment.Web.Models;
using ArsalanAssesment.Web.Repository.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ArsalanAssesment.Web.Controllers
{
    [Route("api/sales")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISaleRepository _saleRepository;

        public SalesController(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateSaleDTO createSaleDTO)
        {
            var response = await _saleRepository.CreateAsync(createSaleDTO);

            response.StatusCode = (response.Result is null) ? (int)HttpStatusCode.NotFound : (int)HttpStatusCode.Created;

            return Ok(response);
        }

        [HttpGet]
        [Route("GetAllSales")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _saleRepository.GetAllAsync();

            response.StatusCode = (response.Result is null) ? (int)HttpStatusCode.NotFound : (int)HttpStatusCode.OK;

            return Ok(response);
        }

        [HttpGet]
        [Route("{saleId:int}")]
        public async Task<IActionResult> Get(int saleId)
        {
            var response = await _saleRepository.GetAsync(saleId);

            response.StatusCode = (response.Result is null) ? (int)HttpStatusCode.NotFound : (int)HttpStatusCode.OK ;

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetByFilters(DateTime? startDate, DateTime? endDate, int representativeId)
        {
            var response = await _saleRepository.GetSalesByFiltersAsync(startDate, endDate, representativeId);

            response.StatusCode = (response.Result is null) ? (int)HttpStatusCode.NotFound : (int)HttpStatusCode.OK;

            return Ok(response);
        }


        [HttpPut]
        [Route("{saleId:int}")]
        public async Task<IActionResult> Update(int saleId, UpdateSaleDTO updateSaleDTO)
        {
            var response = await _saleRepository.UpdateAsync(saleId, updateSaleDTO);

            response.StatusCode = (response.Result is null) ? (int)HttpStatusCode.NotFound : (int)HttpStatusCode.OK;

            return Ok(response);
        }

        [HttpDelete]
        [Route("{saleId:int}")]
        public async Task<IActionResult> Delete(int saleId)
        {
            var response = await _saleRepository.DeleteAsync(saleId);

            response.StatusCode = (response.Result is null) ? (int)HttpStatusCode.NotFound : (int)HttpStatusCode.NoContent;

            return Ok(response);
        }



    }
}

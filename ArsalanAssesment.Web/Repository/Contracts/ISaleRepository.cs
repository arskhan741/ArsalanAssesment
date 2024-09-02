using ArsalanAssesment.Web.DTOs;
using ArsalanAssesment.Web.DTOs.SaleDTOs;

namespace ArsalanAssesment.Web.Repository.Contracts
{
    public interface ISaleRepository
    {
        Task<ResponseDTO> CreateAsync(CreateSaleDTO createSaleDTO);
        Task<ResponseDTO> GetAsync(int saleId);
        Task<ResponseDTO> GetAllAsync();
        Task<ResponseDTO> UpdateAsync(int saleId, UpdateSaleDTO updateSaleDTO);
        Task<ResponseDTO> DeleteAsync(int saleId);
        Task<ResponseDTO> GetSalesByFiltersAsync(DateTime? startDate, DateTime? endDate, int representativeId);

    }
}

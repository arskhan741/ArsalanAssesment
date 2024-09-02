using ArsalanAssesment.Web.Data;
using ArsalanAssesment.Web.DTOs;
using ArsalanAssesment.Web.DTOs.SaleDTOs;
using ArsalanAssesment.Web.Helper;
using ArsalanAssesment.Web.Models;
using ArsalanAssesment.Web.Repository.Contracts;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ArsalanAssesment.Web.Repository
{
    public class SaleRepository : ISaleRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<SaleRepository> _logger;

        public SaleRepository(IMapper mapper, ApplicationDBContext dbContext, ILogger<SaleRepository> logger)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ResponseDTO> CreateAsync(CreateSaleDTO createSaleDTO)
        {
            try
            {
                // Map from DTO to Sale Object
                var sale = _mapper.Map<Sale>(createSaleDTO);

                // Add Sale model in DB and Save Changes
                await _dbContext.Sales.AddAsync(sale);
                await _dbContext.SaveChangesAsync();

                // Create reponse for success and return it
                return ResponseHelper.CreateResponse(true, false, ResponseMessages.Added, _mapper.Map<GetSaleDTO>(sale));
            }
            catch (Exception ex)
            {
                // Log Error Message
                _logger.LogError(ex, "An error occurred while processing the request.");

                // Create reponse for failure, Show simple error message to user, and return it
                return ResponseHelper.CreateResponse(false, true, ResponseMessages.ExceptionMessage);
            }
        }

        public async Task<ResponseDTO> DeleteAsync(int saleId)
        {
            try
            {
                // Find Sale w.r.t Sale ID
                Sale? sale = await _dbContext.Sales.FindAsync(saleId);

                if (sale is null)
                {
                    // If Sale is not found in DB for given ID, we return with Status Code 404
                    return ResponseHelper.CreateResponse(true, false, ResponseMessages.NotFound);
                }

                // Remove sale with SaleID from DB and Save Changes
                _dbContext.Sales.Remove(sale);
                await _dbContext.SaveChangesAsync();

                // Create reponse for success and return it
                return ResponseHelper.CreateResponse(true, false, ResponseMessages.Deleted, _mapper.Map<GetSaleDTO>(sale));

            }
            catch (Exception ex)
            {
                // Log Error Message
                _logger.LogError(ex, "An error occurred while processing the request.");

                // Create reponse for failure, Show simple error message to user, and return it
                return ResponseHelper.CreateResponse(false, true, ResponseMessages.ExceptionMessage);
            }
        }

        public async Task<ResponseDTO> GetAllAsync()
        {
            try
            {
                // Find All Sales from DB, no tracking for optimizations
                var sales = await _dbContext.Sales.AsNoTracking().ToListAsync();

                if (sales.Count <= 0)
                {
                    // If Sales are found we throw new exception
                    return ResponseHelper.CreateResponse(true, false, ResponseMessages.NotFound);
                }

                // Map the sales list to a list of GetSaleDTO and return it in the response
                var salesDTOs = _mapper.Map<List<GetSaleDTO>>(sales);

                // Create reponse for success and return it
                return ResponseHelper.CreateResponse(true, false, ResponseMessages.Successful, salesDTOs);
            }
            catch (Exception ex)
            {
                // Log Error Message
                _logger.LogError(ex, "An error occurred while processing the request.");

                // Create reponse for failure, Show simple error message to user, and return it
                return ResponseHelper.CreateResponse(false, true, ResponseMessages.ExceptionMessage);
            }
        }


        public async Task<ResponseDTO> GetAsync(int saleId)
        {
            try
            {
                // Find Sale w.r.t Sale ID
                Sale? sale = await _dbContext.Sales.FindAsync(saleId);

                if (sale is null)
                {
                    // If Sale is not found in DB for given ID, we return with Status Code 404
                    return ResponseHelper.CreateResponse(true, false, ResponseMessages.NotFound);
                }

                // Create reponse for success and return it
                return ResponseHelper.CreateResponse(true, false, ResponseMessages.Successful, _mapper.Map<GetSaleDTO>(sale));

            }
            catch (Exception ex)
            {
                // Log Error Message
                _logger.LogError(ex, "An error occurred while processing the request.");

                // Create reponse for failure, Show simple error message to user, and return it
                return ResponseHelper.CreateResponse(false, true, ResponseMessages.ExceptionMessage);
            }
        }

        public async Task<ResponseDTO> UpdateAsync(int saleId, UpdateSaleDTO updateSaleDTO)
        {
            try
            {
                // Find Sale w.r.t Sale ID
                Sale? sale = await _dbContext.Sales.FindAsync(saleId);

                if (sale is null)
                {
                    // If Sale is not found in DB for given ID, we return with Status Code 404
                    return ResponseHelper.CreateResponse(true, false, ResponseMessages.NotFound);
                }

                // Update the found sale properties
                sale.Amount = updateSaleDTO.Amount;
                sale.UpdatedOn = DateTimeOffset.UtcNow;
                sale.RepresentativeID = updateSaleDTO.RepresentativeID;

                // Update the Sale model and save changes in DB
                _dbContext.Update(sale);
                await _dbContext.SaveChangesAsync();

                // Create reponse for success and return it
                return ResponseHelper.CreateResponse(true, false, ResponseMessages.Modified, updateSaleDTO);
            }
            catch (Exception ex)
            {
                // Log Error Message
                _logger.LogError(ex, "An error occurred while processing the request.");

                // Create reponse for failure, Show simple error message to user, and return it
                return ResponseHelper.CreateResponse(false, true, ResponseMessages.ExceptionMessage);
            }
        }

        public async Task<ResponseDTO> GetSalesByFiltersAsync(DateTime? startDate, DateTime? endDate, int representativeId)
        {
            try
            {
                // Find All Sales from DB, no tracking for optimizations
                var query = _dbContext.Sales.AsNoTracking().AsQueryable();

                // Check if both dates are provided and validate the date range
                if (startDate.HasValue && endDate.HasValue && representativeId > 0)                 
                {
                    if (endDate.Value <= startDate.Value)
                    {
                        // If Ending date is less than or equal to Starting Date
                        return ResponseHelper.CreateResponse(true, false, ResponseMessages.InvalidDates);
                    }

                    // Apply date range filter if both dates are valid
                    query = query.Where(sale => sale.SaleDate >= startDate.Value
                                                 && sale.SaleDate <= endDate.Value);
                }
                else if ((!startDate.HasValue || !endDate.HasValue) && representativeId > 0)
                {
                    // if Date range is not entered by User but Representative Id is provided
                    query = query.Where(sale => sale.RepresentativeID == representativeId);
                }
                else
                {
                    // If user has neither entered dates or correct dates and Representative Id
                    return ResponseHelper.CreateResponse(true, false, ResponseMessages.InvalidData);
                }


                // Execute the query and retrieve the filtered results
                var sales = await query.ToListAsync();

                if (sales.Count <= 0)
                {
                    // If no sales are found, return a not found response
                    return ResponseHelper.CreateResponse(true, false, ResponseMessages.NotFound);
                }

                // Map the sales list to a list of GetSaleDTO and return it in the response
                var salesDTOs = _mapper.Map<List<GetSaleDTO>>(sales);

                // Create response for success and return it
                return ResponseHelper.CreateResponse(true, false, ResponseMessages.Successful, salesDTOs);
            }
            catch (Exception ex)
            {
                // Log Error Message
                _logger.LogError(ex, "An error occurred while processing the request.");

                // Create response for failure, Show simple error message and return it
                return ResponseHelper.CreateResponse(false, true, ResponseMessages.ExceptionMessage);
            }
        }



    }
}

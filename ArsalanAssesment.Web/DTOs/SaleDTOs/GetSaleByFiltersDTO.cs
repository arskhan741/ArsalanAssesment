using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ArsalanAssesment.Web.DTOs.SaleDTOs
{
    public class GetSaleByFiltersDTO
    {
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;

        [Required, Range(1, int.MaxValue, ErrorMessage = "Representative ID must be a non-negative integer."), DefaultValue(1)]
        public int? RepresentativeId { get; set; } = 0;
    }
}

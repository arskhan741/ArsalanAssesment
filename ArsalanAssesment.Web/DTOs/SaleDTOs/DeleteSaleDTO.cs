using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ArsalanAssesment.Web.DTOs.SaleDTOs
{
    public class DeleteSaleDTO
    {
        public int Id { get; set; }

        [Required, Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Representative ID must be a non-negative integer."), DefaultValue(1)]
        public int RepresentativeID { get; set; }
    }
}

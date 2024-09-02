using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ArsalanAssesment.Web.DTOs.SaleDTOs
{
    public class GetSaleDTO
    {
        public int Id { get; set; }
        public DateTimeOffset SaleDate { get; set; } = DateTimeOffset.UtcNow;
        public decimal Amount { get; set; }
        public int RepresentativeID { get; set; }
    }
}

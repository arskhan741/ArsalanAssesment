using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ArsalanAssesment.Web.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public DateTimeOffset SaleDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.UtcNow;
        public int RepresentativeID { get; set; }
    }
}

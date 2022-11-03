using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasicBilling.Service.Models
{
    public class Bills
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int Period { get; set; }
        public string Category { get; set; }

        public int ClientId { get; set; }

        public virtual Client Client { get; set; }
    }
}

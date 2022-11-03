using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasicBilling.Service.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Bills> Bills { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intense.Data
{
    internal class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LeaseContract { get; set; }

        [Required]
        public int Stock { get; set; }


        [Required]
        public decimal PricePerKg { get; set; }

        public decimal? InstallationCost { get; set; }
        [MaxLength(100)]

        public string Category { get; set; }




    }
}

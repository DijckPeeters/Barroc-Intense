using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intense.Data
{
    public class Product
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

        [Required]
        public decimal InstallationCost { get; set; }

        [MaxLength(100)]

        public string Category { get; set; }
        [NotMapped]
        public bool IsPlanned { get; set; } = false;
        [NotMapped]
        public string UsedStatusText { get; set; }

        public int UsedCount { get; set; }

        public ObservableCollection<Ingredient> Ingredients { get; set; } = new ObservableCollection<Ingredient>();






    }
}

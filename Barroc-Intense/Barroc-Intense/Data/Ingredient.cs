using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intense.Data
{
    public class Ingredient
    {
        public int Id { get; set; }
        public int ProductId { get; set; } // Voor welke koffieboon dit ingrediënt hoort
        public string Name { get; set; }
        public decimal AmountInKg { get; set; }

        public string AmountFormatted => $"{AmountInKg:0.##} kg";
    }

}

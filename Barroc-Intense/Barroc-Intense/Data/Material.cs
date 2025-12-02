using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intense.Data
{
    internal class Material
    {
            public int Id { get; set; }
            public string Name { get; set; } // Naam van het materiaal
            public decimal Price { get; set; } // Prijs in €
        public string PriceFormatted => $"€{Price:0.##}";

    }
}

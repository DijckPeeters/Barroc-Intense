using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Barroc_Intense.Data
{
    public class UsedProductViewModel
    {
        public int Id { get; set; }
        public int DeliveryID { get; set; }

        public int? MeldingID { get; set; }

        public string ProductName { get; set; }  // Naam van het product
        public string Klant { get; set; }
        public DateTime Datum { get; set; }

        public string Status { get; set; }       // bijv. “In behandeling”, “Afgerond”
        public string Prioriteit { get; set; }   // ← nieuw veld: “Laag”, “Middel”, “Hoog”

        public string Afdeling { get; set; } = "Onderhoud";
        public string Probleemomschrijving { get; set; } = "";
        public bool IsOpgelost { get; set; } = false;

        // Extra property (alleen voor weergave)
        public string BelangrijkMelding
        {
            get
            {
                if (Prioriteit?.ToLower() == "hoog")
                    return "⚠ Belangrijk! Deze melding vereist snelle opvolging.";
                return string.Empty;
            }
        }
    }
}




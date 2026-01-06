using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Barroc_Intense.Data;

namespace Barroc_Intense.Data
{
    public class Melding
    {
        public int Id { get; set; }
        public int MonteurId { get; set; }
        public int MachineId { get; set; }
        public Machine Machine { get; set; }    
        public string Prioriteit { get; set; } = "Laag"; // Laag / Middel / Hoog
        public string Afdeling { get; set; } = string.Empty;
        public DateTime? Datum { get; set; }
        public string Klant { get; set; } = string.Empty;
        public string Product { get; set; } = string.Empty;
        public string Probleemomschrijving { get; set; } = string.Empty;
        public string Status { get; set; } = "Open"; // Open / In behandeling / Gesloten
        public bool IsOpgelost { get; set; } = false;

        // Velden die later ingevuld worden
        public string? Storingscode { get; set; }
        public bool? StoringVerholpen { get; set; }
        public string? Vervolgafspraak { get; set; }
        public string? KorteBeschrijving { get; set; }
        public string? GebruikteOnderdelen { get; set; } // CSV string

        // Voor keuring
        public bool? ChecklistVolledig { get; set; }
        public bool KeuringGoedgekeurd { get; set; }
        public bool IsKeuringVoltooid { get; set; }
        public bool IsKeuring { get; set; }

        public string? KeuringOpmerkingen { get; set; }
        public string? Handtekening { get; set; }





        // Optioneel: koppeling naar Delivery
        public int? DeliveryID { get; set; }
        public Delivery Delivery { get; set; }
    }

}


    //public Machine Machine { get; set; }    






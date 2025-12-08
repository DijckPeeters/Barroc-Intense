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

        public string ProductName { get; set; }  // ← voor weergave van product
        public string Klant { get; set; }
        public DateTime Datum { get; set; }
        public string Status { get; set; }

        public string Afdeling { get; set; } = "Onderhoud";
        public string Probleemomschrijving { get; set; } = "";
        public bool IsOpgelost { get; set; } = false;
    }



}



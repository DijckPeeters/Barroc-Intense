using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intense.Data
{
    public class MaintenanceMaterial
    {
        public int Id { get; set; }

        public int MeldingId { get; set; }   // ← KEURING ID
        public Melding Melding { get; set; } // <--- toevoegen

        public int MaterialId { get; set; }
        public Material Material { get; set; }

        public int QuantityUsed { get; set; }
        public int? DeliveryID { get; set; }  // optioneel
        public Delivery Delivery { get; set; } // optioneel
    }



}

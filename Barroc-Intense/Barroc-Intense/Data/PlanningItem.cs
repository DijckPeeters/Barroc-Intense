using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intense.Data
{
    class PlanningItem
    {
        public int Id { get; set; }
        public int MachineId { get; set; }
        public string Beschrijving { get; set; }
        public DateTime Datum { get; set; }
        public string Type { get; set; } // Keuring / Onderhoud / Reparatie
        public Machine Machine { get; set; }
    }
}

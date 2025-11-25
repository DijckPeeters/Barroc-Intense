using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intense.Data
{
    class Machine
    {
        public int Id { get; set; }
        public string Locatie { get; set; }
        public string Status { get; set; } // Operationeel / Storend / Onderhoud

        // Relatie met meldingen
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public List<Melding> Meldingen { get; set; } = new List<Melding>();
    }
}

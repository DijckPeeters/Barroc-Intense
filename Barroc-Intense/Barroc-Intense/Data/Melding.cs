using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intense.Data
{

        public class Melding
        {
            public int Id { get; set; }
            public string Afdeling { get; set; } 
            public DateTime Datum { get; set; }
            public string Klant { get; set; }
            public string Product { get; set; }
            public string Probleemomschrijving { get; set; }
            public string Status { get; set; }
        public bool IsOpgelost { get; set; } = false;
    }
    }



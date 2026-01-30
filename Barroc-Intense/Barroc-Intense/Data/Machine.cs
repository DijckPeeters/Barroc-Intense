using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intense.Data
{
    public class Machine
    {
        public int Id { get; set; }
        public string Locatie { get; set; }
        public string Status { get; set; }

        public List<Delivery> Deliveries { get; set; } = new List<Delivery>();
        public List<Melding> Meldingen { get; set; } = new List<Melding>();

        // Eerste Delivery (indien aanwezig) voor adres, productnaam en klantnaam
        public string FirstDeliveryAddress => Deliveries.FirstOrDefault()?.DeliveryAddress ?? "Geen adres";
        public string FirstDeliveryProductName => Deliveries.FirstOrDefault()?.ProductName ?? "Geen product";
        public string FirstDeliveryCustomerName => Deliveries.FirstOrDefault()?.CustomerName ?? "Geen klant";
    }



}

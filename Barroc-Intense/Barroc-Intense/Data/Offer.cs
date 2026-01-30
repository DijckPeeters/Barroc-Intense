using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intense.Data
{
    internal class Offer
    {
        public int Id { get; set; }
        public string OfferNumber { get; set; }
        public DateTime Date { get; set; }

        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }

        public List<OfferItem> Items { get; set; } = new List<OfferItem>();

        public decimal TotalAmount =>
            Items.Sum(i => i.TotalPrice);

    }
}

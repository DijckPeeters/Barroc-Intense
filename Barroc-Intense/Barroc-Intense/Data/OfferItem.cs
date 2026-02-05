using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intense.Data
{
    internal class OfferItem
    {
        public int Id { get; set; }

        public int OfferId { get; set; }
        public Offer Offer { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice =>
            Quantity * Product.PricePerKg + Product.InstallationCost;
    }
}

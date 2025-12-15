using Barroc_Intense.Data;
using Barroc_Intense.Pages.Dashboards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intense.Services
{
    internal class InvoiceService
    {
        public static Barroc_Intense.Data.Invoice CreateInvoiceFromOffer(Offer offer)
        {
            var invoice = new Barroc_Intense.Data.Invoice
            {
                InvoiceNumber = $"INV-{offer.OfferNumber}",
                Date = DateTime.Now,
                CustomerName = offer.CustomerName,
                CustomerAddress = offer.CustomerAddress,
                Items = offer.Items?.Select(item => new InvoiceItem
                {
                    Product = item.Product,
                    Quantity = item.Quantity,
                }).ToList() ?? new List<InvoiceItem>()
            };

            foreach (var item in invoice.Items)
            {
                if (item?.Product != null)
                {
                    item.Product.Stock -= item.Quantity;
                }
            }

            return invoice;
        }
    }
}

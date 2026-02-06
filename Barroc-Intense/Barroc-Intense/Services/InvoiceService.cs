using Barroc_Intense.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Barroc_Intense.Services
{
    internal class InvoiceService
    {
        public static Invoice CreateInvoiceFromOffer(Offer offer)
        {
            var invoice = new Invoice
            {
                InvoiceNumber = $"INV-{offer.OfferNumber}",
                Date = DateTime.Now,
                CustomerName = offer.CustomerName,
                CustomerAddress = offer.CustomerAddress,
                Items = offer.Items.Select(item => new InvoiceItem
                {
                    ProductId = item.ProductId,
                    Product = item.Product,
                    Quantity = item.Quantity
                }).ToList()
            };

            foreach (var item in invoice.Items)
            {
                if (item.Product != null)
                {
                    item.Product.Stock -= item.Quantity;
                }
            }

            return invoice;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intense.Data
{
    internal class Invoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime Date { get; set; }

        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }

        public List<InvoiceItem> Items { get; set; } = new();

        public bool IsPaid { get; set; }

        public decimal TotalAmount =>
            Items.Sum(i => i.TotalPrice);

    }
}

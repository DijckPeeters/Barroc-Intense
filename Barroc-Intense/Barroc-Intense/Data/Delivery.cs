using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using System.Threading.Tasks;

namespace Barroc_Intense.Data
{
    internal class Delivery
    {
        public int DeliveryID { get; set; }

        public int? OrderID { get; set; }
        public int? ProductID { get; set; }

        public string ProductName { get; set; }
        public int QuantityDelivered { get; set; }
        public int? QuantityExpected { get; set; }

        public DateTime PlannedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }

        public string Status { get; set; } // planned, underway, delivered, delayed, cancelled

        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string DeliveryAddress { get; set; }
        public string CustomerContact { get; set; }

        public string CarrierName { get; set; }
        public string DriverName { get; set; }
        public string TrackingNumber { get; set; }

        public string Notes { get; set; }
    }
}

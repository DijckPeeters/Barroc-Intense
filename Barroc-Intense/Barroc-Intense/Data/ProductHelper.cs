using System;
using System.Collections.Generic;
using System.Linq;

namespace Barroc_Intense.Data
{
    public static class ProductHelper
    {
        public static string GetUsedStatusText(int productId)
        {
            using var db = new AppDbContext();
            var deliveries = db.Deliveries.Where(d => d.ProductID == productId).ToList();

            int inGebruik = deliveries.Count(d => d.Status == "Delivered");
            int onderweg = deliveries.Count(d => d.Status == "Underway");
            int ingepland = deliveries.Count(d => d.Status == "Planned");
            int moetIngepland = deliveries.Count(d => d.Status == "Not planned");

            var statusLines = new List<string>();

            if (inGebruik > 0) statusLines.Add($"{inGebruik}× in gebruik");
            if (onderweg > 0) statusLines.Add($"{onderweg}× onderweg");
            if (ingepland > 0) statusLines.Add($"{ingepland}× ingepland");
            if (moetIngepland > 0) statusLines.Add($"{moetIngepland}× moet ingepland worden");

            if (statusLines.Count == 0)
                statusLines.Add("0× in gebruik");

            return string.Join(Environment.NewLine, statusLines);
        }
    }
}
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using Barroc_Intense.Data;
using System;
using System.IO;
using System.Linq;

namespace Barroc_Intense.Services
{
    internal class PdfService
    {
        public static void CreateInvoicePdf(Invoice invoice, string filePath)
        {
            if (invoice == null) throw new ArgumentNullException(nameof(invoice));
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("filePath is required", nameof(filePath));

            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using var document = new PdfDocument();
            document.Info.Title = $"Invoice {invoice.InvoiceNumber}";

            var page = document.AddPage();
            using var gfx = XGraphics.FromPdfPage(page);
            var titleFont = new XFont("Verdana", 16, XFontStyleEx.Bold);
            var regularFont = new XFont("Verdana", 10, XFontStyleEx.Regular);

            double margin = 40;
            double y = margin;

            gfx.DrawString("FACTUUR", titleFont, XBrushes.Black, new XRect(margin, y, page.Width - 2 * margin, 24), XStringFormats.TopLeft);
            y += 32;

            gfx.DrawString($"Factuurnummer: {invoice.InvoiceNumber}", regularFont, XBrushes.Black, new XPoint(margin, y)); y += 18;
            gfx.DrawString($"Datum: {invoice.Date:dd-MM-yyyy}", regularFont, XBrushes.Black, new XPoint(margin, y)); y += 18;
            gfx.DrawString($"Klantnaam: {invoice.CustomerName}", regularFont, XBrushes.Black, new XPoint(margin, y)); y += 18;
            gfx.DrawString(invoice.CustomerAddress ?? string.Empty, regularFont, XBrushes.Black, new XRect(margin, y, page.Width - 2 * margin, 36), XStringFormats.TopLeft);
            y += 40;

            foreach (var item in invoice.Items ?? Enumerable.Empty<InvoiceItem>())
            {
                var productName = item?.Product?.ProductName ?? "Onbekend product";
                var qty = item?.Quantity ?? 0;
                var total = item != null ? item.TotalPrice : 0m;

                gfx.DrawString($"{productName} x {qty} - €{total:0.00}", regularFont, XBrushes.Black, new XPoint(margin, y));
                y += 16;
            }

            y += 8;
            gfx.DrawString($"Totaal: €{invoice.TotalAmount:0.00}", regularFont, XBrushes.Black, new XPoint(margin, y));

            document.Save(filePath);
        }
    }
}

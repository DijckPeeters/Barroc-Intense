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
            //  check op null argumenten
            if (invoice == null) throw new ArgumentNullException(nameof(invoice));
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("filePath is required", nameof(filePath));

            //  maak de map aan als die nog niet bestaat
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            //  maak nieuw PDF document aan
            using var document = new PdfDocument();
            document.Info.Title = $"Invoice {invoice.InvoiceNumber}";

            var page = document.AddPage();
            using var gfx = XGraphics.FromPdfPage(page);

            //  definieer fonts
            var titleFont = new XFont("Verdana", 16, XFontStyleEx.Bold);
            var regularFont = new XFont("Verdana", 10, XFontStyleEx.Regular);

            double margin = 40;
            double y = margin;

            //  schrijf header
            gfx.DrawString("FACTUUR", titleFont, XBrushes.Black, new XRect(margin, y, page.Width - 2 * margin, 24), XStringFormats.TopLeft);
            y += 32;

            //  klant- en factuurinformatie
            gfx.DrawString($"Factuurnummer: {invoice.InvoiceNumber}", regularFont, XBrushes.Black, new XPoint(margin, y)); y += 18;
            gfx.DrawString($"Datum: {invoice.Date:dd-MM-yyyy}", regularFont, XBrushes.Black, new XPoint(margin, y)); y += 18;
            gfx.DrawString($"Klantnaam: {invoice.CustomerName}", regularFont, XBrushes.Black, new XPoint(margin, y)); y += 18;
            gfx.DrawString(invoice.CustomerAddress ?? string.Empty, regularFont, XBrushes.Black, new XRect(margin, y, page.Width - 2 * margin, 36), XStringFormats.TopLeft);
            y += 40;

            //  schrijf factuurregels
            foreach (var item in invoice.Items ?? Enumerable.Empty<InvoiceItem>())
            {
                var productName = item?.Product?.ProductName ?? "Onbekend product";
                var qty = item?.Quantity ?? 0;
                var total = item != null ? item.TotalPrice : 0m;

                gfx.DrawString($"{productName} x {qty} - €{total:0.00}", regularFont, XBrushes.Black, new XPoint(margin, y));
                y += 16;
            }

            //  totaalbedrag
            y += 8;
            gfx.DrawString($"Totaal: €{invoice.TotalAmount:0.00}", regularFont, XBrushes.Black, new XPoint(margin, y));

            //  sla PDF op
            document.Save(filePath);
        }
    }
}
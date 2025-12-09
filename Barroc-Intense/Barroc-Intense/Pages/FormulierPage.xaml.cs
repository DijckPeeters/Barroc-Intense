using Barroc_Intense.Data;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace Barroc_Intense.Pages
{
    public sealed partial class FormulierPage : Page
    {
        private Melding _melding;
        private bool _isDrawing = false;
        private Windows.Foundation.Point _lastPoint;

        public FormulierPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            int id = (int)e.Parameter;

            using var db = new AppDbContext();
            _melding = db.Meldingen.First(x => x.Id == id);

            TitleBlock.Text = _melding.IsKeuring ? "Keuring" : "Melding";

            // Handtekening tonen als die al bestaat
            if (!string.IsNullOrEmpty(_melding.Handtekening))
            {
                await LoadSavedSignature(_melding.Handtekening);
            }

            // Materialen inladen
            var usedPartsRaw = _melding.GebruikteOnderdelen ?? "";
            var usedParts = usedPartsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(x => x.Trim())
                                        .ToList();

            var materialEntities = db.Materials.ToList();

            var materials = materialEntities
                .Select(mat =>
                {
                    int quantity = 0;
                    var match = usedParts.FirstOrDefault(u => u.StartsWith(mat.Name));
                    if (match != null)
                    {
                        var parts = match.Split(" x ");
                        if (parts.Length == 2 && int.TryParse(parts[1], out int q))
                            quantity = q;
                    }

                    return new MaterialViewModel
                    {
                        Id = mat.Id,
                        Name = mat.Name,
                        Price = mat.Price,
                        Quantity = quantity,
                        IsSelected = quantity > 0
                    };
                }).ToList();

            OnderdelenList.ItemsSource = materials;

            // Formulier invullen
            if (_melding.IsKeuring)
            {
                //MeldingPanel.Visibility = Visibility.Collapsed;
                ChecklistBox.IsChecked = _melding.ChecklistVolledig;
                GoedgekeurdBox.IsChecked = _melding.KeuringGoedgekeurd;
                KeuringOpmerkingenBox.Text = _melding.KeuringOpmerkingen;
            }
            else
            {
                //KeuringPanel.Visibility = Visibility.Collapsed;
                KlantBox.Text = _melding.Klant;
                //DatumBox.Text = _melding.Datum.ToString("dd-MM-yyyy HH:mm");
                InitieleBox.Text = _melding.Probleemomschrijving;
                MachineBox.Text = _melding.Product;
                StoringscodeBox.Text = _melding.Storingscode;
                VerholpenBox.IsChecked = _melding.StoringVerholpen;
                VervolgBox.Text = _melding.Vervolgafspraak;
                BeschrijvingBox.Text = _melding.KorteBeschrijving;
            }
        }

        // PLUS & MIN knoppen
        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is MaterialViewModel vm)
            {
                vm.Quantity++;
                vm.IsSelected = true;
            }
        }

        private void Min_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is MaterialViewModel vm)
            {
                if (vm.Quantity > 0) vm.Quantity--;
                if (vm.Quantity == 0) vm.IsSelected = false;
            }
        }

        // Handtekening tekenen
        private void SignatureCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _isDrawing = true;
            _lastPoint = e.GetCurrentPoint(SignatureCanvas).Position;
        }

        private void SignatureCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isDrawing) return;

            var currentPoint = e.GetCurrentPoint(SignatureCanvas).Position;

            var line = new Line
            {
                X1 = _lastPoint.X,
                Y1 = _lastPoint.Y,
                X2 = currentPoint.X,
                Y2 = currentPoint.Y,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 2
            };

            SignatureCanvas.Children.Add(line);
            _lastPoint = currentPoint;
        }

        private void SignatureCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _isDrawing = false;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SignatureCanvas.Children.Clear();
        }

        // Veilig opslaan van handtekening
        private async Task<string> SaveSignatureAsync()
        {
            if (SignatureCanvas.Children.Count == 0 || SignatureCanvas.Visibility != Visibility.Visible)
                return null;

            var renderTargetBitmap = new RenderTargetBitmap();
            await Task.Delay(50); // kleine delay om layout te fixen
            await renderTargetBitmap.RenderAsync(SignatureCanvas);

            var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();
            using var stream = new InMemoryRandomAccessStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            encoder.SetPixelData(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied,
                (uint)renderTargetBitmap.PixelWidth,
                (uint)renderTargetBitmap.PixelHeight,
                96, 96,
                pixelBuffer.ToArray()
            );

            await encoder.FlushAsync();

            stream.Seek(0);
            var bytes = new byte[stream.Size];
            await stream.ReadAsync(bytes.AsBuffer(), (uint)stream.Size, InputStreamOptions.None);

            return Convert.ToBase64String(bytes);
        }

        // Handtekening laden
        private async Task LoadSavedSignature(string base64)
        {
            if (string.IsNullOrEmpty(base64)) return;

            byte[] bytes = Convert.FromBase64String(base64);

            using var stream = new InMemoryRandomAccessStream();
            await stream.WriteAsync(bytes.AsBuffer());
            stream.Seek(0);

            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(stream);

            SavedSignatureImage.Source = bitmap;
            SavedSignatureImage.Visibility = Visibility.Visible;
            SignatureCanvas.Visibility = Visibility.Collapsed;
        }

        // Opslaan-knop
        private async void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();
            var m = db.Meldingen.First(x => x.Id == _melding.Id);

            // KEURING / MELDING
            if (m.IsKeuring)
            {
                m.ChecklistVolledig = ChecklistBox.IsChecked ?? false;
                m.KeuringGoedgekeurd = GoedgekeurdBox.IsChecked ?? false;
                m.KeuringOpmerkingen = KeuringOpmerkingenBox.Text;
            }
            else
            {
                m.Storingscode = StoringscodeBox.Text;
                m.StoringVerholpen = VerholpenBox.IsChecked ?? false;
                m.Vervolgafspraak = VervolgBox.Text;
                m.KorteBeschrijving = BeschrijvingBox.Text;
            }

            // Onderdelen
            var materials = OnderdelenList.Items
                .Cast<MaterialViewModel>()
                .Where(vm => vm.IsSelected && vm.Quantity > 0)
                .Select(vm => $"{vm.Name} x {vm.Quantity}");
            m.GebruikteOnderdelen = string.Join(", ", materials);

            // Handtekening opslaan en tonen
            var base64Signature = await SaveSignatureAsync();
            if (base64Signature != null)
            {
                m.Handtekening = base64Signature;
                await LoadSavedSignature(base64Signature);
            }
            m.IsKeuringVoltooid = true;
            db.SaveChanges();

            // Navigeren naar MaintenancePage
     


            Frame?.Navigate(typeof(MaintenancePagee));

        }
    }
}

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
        private int _keuringId;

        // Handtekening
        private bool _isDrawing = false;
        private Windows.Foundation.Point _lastPoint;

        private AppDbContext _context = new AppDbContext();

        public FormulierPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            _keuringId = (int)e.Parameter;
            _melding = _context.Meldingen.First(x => x.Id == _keuringId);

            TitleBlock.Text = _melding.IsKeuring ? "Keuring" : "Melding";

            // ======================
            // MATERIALEN LADEN (DB)
            // ======================
            var materialsFromDb = _context.Materials.ToList();

            var usedMaterials = _context.MaintenanceMaterials
                .Where(x => x.MeldingId == _keuringId)
                .ToList();

            var materials = materialsFromDb
                .Select(mat =>
                {
                    var used = usedMaterials.FirstOrDefault(x => x.MaterialId == mat.Id);
                    return new MaterialViewModel
                    {
                        Id = mat.Id,
                        Name = mat.Name,
                        Price = mat.Price,
                        Quantity = used?.QuantityUsed ?? 0,
                        IsSelected = used != null
                    };
                })
                .ToList();

            OnderdelenList.ItemsSource = materials;

            // ======================
            // FORMULIER INVULLEN
            // ======================
            if (_melding.IsKeuring)
            {
                ChecklistBox.IsChecked = _melding.ChecklistVolledig;
                GoedgekeurdBox.IsChecked = _melding.KeuringGoedgekeurd;
                KeuringOpmerkingenBox.Text = _melding.KeuringOpmerkingen;
            }
            else
            {
                StoringscodeBox.Text = _melding.Storingscode;
                VerholpenBox.IsChecked = _melding.StoringVerholpen;
                VervolgBox.Text = _melding.Vervolgafspraak;
                BeschrijvingBox.Text = _melding.KorteBeschrijving;
            }

            // ======================
            // HANDTEKENING LADEN
            // ======================
            if (!string.IsNullOrEmpty(_melding.Handtekening))
                await LoadSavedSignature(_melding.Handtekening);
        }

        // ======================
        // PLUS KNOP (DB)
        // ======================
        private async void Plus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is MaterialViewModel vm)
            {
                vm.Quantity++;
                vm.IsSelected = true;

                using var context = new AppDbContext();
                var item = context.MaintenanceMaterials
                    .FirstOrDefault(x => x.MeldingId == _keuringId && x.MaterialId == vm.Id);

                if (item == null)
                {
                    item = new MaintenanceMaterial
                    {
                        MeldingId = _keuringId,
                        MaterialId = vm.Id,
                        QuantityUsed = 1
                    };
                    context.MaintenanceMaterials.Add(item);
                }
                else
                {
                    item.QuantityUsed++;
                }

                await context.SaveChangesAsync();
            }
        }

        // ======================
        // MIN KNOP (DB)
        // ======================
        private async void Min_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is MaterialViewModel vm)
            {
                if (vm.Quantity == 0) return;

                vm.Quantity--;
                vm.IsSelected = vm.Quantity > 0;

                var item = _context.MaintenanceMaterials
                    .FirstOrDefault(x => x.MeldingId == _keuringId && x.MaterialId == vm.Id);

                if (item == null) return;

                item.QuantityUsed--;

                if (item.QuantityUsed <= 0)
                    _context.MaintenanceMaterials.Remove(item);

                await _context.SaveChangesAsync();
            }
        }

        // ======================
        // HANDTEKENING TEKENEN
        // ======================
        private void SignatureCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _isDrawing = true;
            _lastPoint = e.GetCurrentPoint(SignatureCanvas).Position;
        }

        private void SignatureCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isDrawing) return;

            var current = e.GetCurrentPoint(SignatureCanvas).Position;

            SignatureCanvas.Children.Add(new Line
            {
                X1 = _lastPoint.X,
                Y1 = _lastPoint.Y,
                X2 = current.X,
                Y2 = current.Y,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 2
            });

            _lastPoint = current;
        }

        private void SignatureCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _isDrawing = false;
        }

        // ======================
        // HANDTEKENING OPSLAAN
        // ======================
        private async Task<string> SaveSignatureAsync()
        {
            if (SignatureCanvas.Children.Count == 0)
                return null;

            var bmp = new RenderTargetBitmap();
            await bmp.RenderAsync(SignatureCanvas);
            var buffer = await bmp.GetPixelsAsync();

            using var stream = new InMemoryRandomAccessStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

            encoder.SetPixelData(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied,
                (uint)bmp.PixelWidth,
                (uint)bmp.PixelHeight,
                96, 96,
                buffer.ToArray());

            await encoder.FlushAsync();

            stream.Seek(0);
            var bytes = new byte[stream.Size];
            await stream.ReadAsync(bytes.AsBuffer(), (uint)stream.Size, InputStreamOptions.None);

            return Convert.ToBase64String(bytes);
        }

        // ======================
        // HANDTEKENING LADEN
        // ======================
        private async Task LoadSavedSignature(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);

            using var stream = new InMemoryRandomAccessStream();
            await stream.WriteAsync(bytes.AsBuffer());
            stream.Seek(0);

            var bmp = new BitmapImage();
            await bmp.SetSourceAsync(stream);

            SavedSignatureImage.Source = bmp;
            SavedSignatureImage.Visibility = Visibility.Visible;
            SignatureCanvas.Visibility = Visibility.Collapsed;
        }

        // ======================
        // OPSLAAN
        // ======================
        private async void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            if (_melding.IsKeuring)
            {
                _melding.ChecklistVolledig = ChecklistBox.IsChecked ?? false;
                _melding.KeuringGoedgekeurd = GoedgekeurdBox.IsChecked ?? false;
                _melding.KeuringOpmerkingen = KeuringOpmerkingenBox.Text;

                // VOORRAAD AFTREKKEN (1x bij goedgekeurde keuring)
                if ((_melding.KeuringGoedgekeurd ?? false) && !_melding.IsKeuringVoltooid)

                {
                    var materialen = _context.MaintenanceMaterials
                        .Where(x => x.MeldingId == _keuringId)
                        .ToList();

                    var alleMaterials = _context.Materials.ToList();

                    foreach (var item in materialen)
                    {
                        var mat = alleMaterials.First(m => m.Id == item.MaterialId);
                        mat.Stock -= item.QuantityUsed;
                    }
                }
            }
            else
            {
                _melding.Storingscode = StoringscodeBox.Text;
                _melding.StoringVerholpen = VerholpenBox.IsChecked ?? false;
                _melding.Vervolgafspraak = VervolgBox.Text;
                _melding.KorteBeschrijving = BeschrijvingBox.Text;
            }

            // HANDTEKENING
            var signature = await SaveSignatureAsync();
            if (signature != null)
                _melding.Handtekening = signature;

            _melding.IsKeuringVoltooid = true;

            // 1 SAVE
            await _context.SaveChangesAsync();

            // NAVIGATIE
            Frame.Navigate(typeof(MaintenanceDashboard));
        }

        // ======================
        // CLEAR HANDTEKENING
        // ======================
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SignatureCanvas.Children.Clear();
            SavedSignatureImage.Source = null;
            SavedSignatureImage.Visibility = Visibility.Collapsed;
            SignatureCanvas.Visibility = Visibility.Visible;
        }
    }
}
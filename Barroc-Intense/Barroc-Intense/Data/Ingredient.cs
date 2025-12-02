using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barroc_Intense.Data
{
    public class Ingredient : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }

        private decimal _amountInKg;
        public decimal AmountInKg
        {
            get => _amountInKg;
            set
            {
                if (_amountInKg != value)
                {
                    _amountInKg = value;
                    OnPropertyChanged(nameof(AmountInKg));
                    OnPropertyChanged(nameof(AmountText)); // Zorg dat UI ook updated
                }
            }
        }

        // Property voor TwoWay binding naar TextBox
        [NotMapped]
        public string AmountText
        {
            get => AmountInKg.ToString("0.##");
            set
            {
                if (decimal.TryParse(value, out var result))
                {
                    AmountInKg = result;
                }
            }
        }

        public string AmountFormatted => $"{AmountInKg:0.##} kg";

        public virtual Product Product { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

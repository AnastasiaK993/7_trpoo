using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cars_Client_seven.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditPage.xaml
    /// </summary>
    public partial class EditPage : Page
    {
        private readonly CarApiService _api = CarApiService.Instance;
        private readonly Car _original;

        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string ReleaseYear { get; set; } = "";
        public string Price { get; set; } = "";
        public bool IsAvailable { get; set; }

        public EditPage(Car car)
        {
            InitializeComponent();
            _original = car;

            Brand = car.Brand;
            Model = car.Model;
            ReleaseYear = car.ReleaseYear.ToString();
            Price = car.Price.ToString();
            IsAvailable = car.IsAvailable;

            DataContext = this;
        }

        private async void ClickSave(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(ReleaseYear, out var year) ||
                !decimal.TryParse(Price, out var price))
            {
                MessageBox.Show("Year и Price должны быть числами");
                return;
            }

            var updated = new Car
            {
                Id = _original.Id,
                Brand = Brand,
                Model = Model,
                ReleaseYear = year,
                Price = price,
                IsAvailable = IsAvailable
            };

            try
            {
                var ok = await _api.UpdateAsync(updated);
                if (ok)
                    NavigationService.Navigate(new MainPage());
                else
                    MessageBox.Show("Ошибка обновления");
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Сервер недоступен");
            }
        }

        private async void ClickPatch(object sender, RoutedEventArgs e)
        {
            var patch = new CarRight
            {
                Brand = string.IsNullOrWhiteSpace(Brand) ? null : Brand,
                Model = string.IsNullOrWhiteSpace(Model) ? null : Model,
                ReleaseYear = int.TryParse(ReleaseYear, out var y) ? y : null,
                Price = decimal.TryParse(Price, out var p) ? p : null,
                IsAvailable = IsAvailable
            };

            try
            {
                var ok = await _api.PatchAsync(_original.Id, patch);
                if (ok) NavigationService.Navigate(new MainPage());
                else MessageBox.Show("Ошибка PATCH");
            }
            catch (HttpRequestException) { MessageBox.Show("Сервер недоступен"); }
            catch (TaskCanceledException) { MessageBox.Show("Таймаут"); }
        }

        private void ClickOut(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }
    }
}
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
    /// Логика взаимодействия для PostPage.xaml
    /// </summary>
    public partial class PostPage : Page
    {
        private readonly CarApiService _api = CarApiService.Instance;
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string ReleaseYear { get; set; } = "";
        public string Price { get; set; } = "";
        public bool IsAvailable { get; set; }

        public PostPage()
        {
            InitializeComponent();
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

            var car = new Car
            {
                Brand = Brand,
                Model = Model,
                ReleaseYear = year,
                Price = price,
                IsAvailable = IsAvailable
            };

            try
            {
                var created = await _api.CreateAsync(car);
                if (created != null)
                    NavigationService.Navigate(new MainPage());
                else
                    MessageBox.Show("Ошибка создания");
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Сервер недоступен");
            }
        }

        private void ClickOut(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }
    }
}
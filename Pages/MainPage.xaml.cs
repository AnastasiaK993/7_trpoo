using Cars_Client_seven;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
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
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
        private readonly CarApiService _api = CarApiService.Instance;

        public ObservableCollection<Car> Cars { get; } = new();
  

        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += async (_, _) => await LoadAsync();

        }
        private Car? _selectedCar;
        public Car? SelectedCar
        {
            get => _selectedCar;
            set { _selectedCar = value; OnPropertyChanged(); }
        }
        private void ClickPost(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PostPage());
        }

        private async void ClickDelete(object sender, RoutedEventArgs e)
        {
            if (SelectedCar == null)
            {
                MessageBox.Show("Выберите запись!");
                return;
            }

            if (MessageBox.Show("Вы действительно хотите удалить запись?", "Удалить?",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            try
            {
                var ok = await _api.DeleteAsync(SelectedCar.Id);
                if (ok)
                {
                    Cars.Remove(SelectedCar);
                    SelectedCar = null;
                    MessageBox.Show("Удалено");
                }
                else
                {
                    MessageBox.Show("Ошибка удаления");
                }
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Сервер недоступен");
            }
        }

        private void ClickEdit(object sender, RoutedEventArgs e)
        {
            if (SelectedCar == null)
            {
               MessageBox.Show("Выберите машину");
                return;
            }else
            NavigationService.Navigate(new EditPage(SelectedCar));
        }
       
        private void Show(object sender, RoutedEventArgs e)
        {
            if (SelectedCar == null)
            {
                MessageBox.Show("Выберите машину");
                return;
            }
            NavigationService.Navigate(new CarPage(SelectedCar));
        }

        private void SortChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void ClickNormal(object sender, RoutedEventArgs e)
        {

            await LoadAsync();
        }

        private string _filterBrand = "";
        public string FilterBrand
        {
            get => _filterBrand;
            set { _filterBrand = value; OnPropertyChanged(); }
        }

        private string _filterMaxPrice = "";
        public string FilterMaxPrice
        {
            get => _filterMaxPrice;
            set { _filterMaxPrice = value; OnPropertyChanged(); }
        }

        private async void ClickFilter(object sender, RoutedEventArgs e)
        {
            decimal? maxPrice = decimal.TryParse(FilterMaxPrice, out var p) ? p : null;
            await LoadAsync(FilterBrand, maxPrice);
        }

        private async Task LoadAsync(string? brand = null, decimal? maxPrice = null)
        {
            try
            {
                var list = await _api.GetAllAsync(brand, maxPrice);
                Cars.Clear();
                foreach (var c in list) Cars.Add(c);
                SelectedCar = null;
            }
            catch (HttpRequestException) { MessageBox.Show("Сервер недоступен"); }
            catch (TaskCanceledException) { MessageBox.Show("Таймаут"); }
            catch (JsonException) { MessageBox.Show("Неверный формат ответа"); }
        }
    }
}

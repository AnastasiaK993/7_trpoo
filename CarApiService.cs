using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Cars_Client_seven
{
    public class CarApiService
    {
        private static CarApiService? _instance;
        public static CarApiService Instance => _instance ??= new CarApiService();


        private readonly HttpClient _http = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        })
        { BaseAddress=new Uri("https://localhost:7165/")
        };
        public async Task<List<Car>> GetAllAsync(string? brand=null,decimal? maxPrice=null)
        {
            var url = "api/v1/cars?";
            if (!string.IsNullOrWhiteSpace(brand))
                url += $"brand={Uri.EscapeDataString(brand)}&";
            if (maxPrice.HasValue)
                url += $"maxPrice={maxPrice.Value}";

            var list = await _http.GetFromJsonAsync<List<Car>>(url);
            return list ?? new List<Car>();
        }

        public async Task<Car?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/v1/cars/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<Car>();
        }
        public async Task<Car?> CreateAsync(Car car)
        {
            var response = await _http.PostAsJsonAsync("api/v1/cars", car);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Car>();
        }

        public async Task<bool> UpdateAsync(Car car)
        {
            var response = await _http.PutAsJsonAsync($"api/v1/cars/{car.Id}", car);
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> PatchAsync(int id, CarRight patch)
        {
            var request = new HttpRequestMessage(new HttpMethod("PATCH"),
                $"api/v1/cars/{id}")
            {
                Content = JsonContent.Create(patch)
            };
            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/v1/cars/{id}");
            return response.IsSuccessStatusCode;
        }



    }
}

using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace SpaceWarsServices;

 public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<JoinGameResponse> JoinGameAsync(string name)
        {
             try
            {
                var response = await _httpClient.GetAsync($"/game/join?name={Uri.EscapeDataString(name)}");

                response.EnsureSuccessStatusCode(); // Throw an exception if the status code is not a success code

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<JoinGameResponse>(content);

                return result;
            }
             catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }

using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace SpaceWarsServices;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public int CurrentHeading { get; set; }

    public ApiService(HttpClient httpClient, int currentHeading)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        CurrentHeading = currentHeading;
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

    public async Task ChangeHeadingAsync(string direction, string token)
    {
        if (direction == "left")
        {
            if (CurrentHeading == 0)
            {
                CurrentHeading = 360-15;
                var response = await _httpClient.GetAsync($"/game/{token}/queue/[type=move,request={CurrentHeading}]");
            }
            else
            {
                CurrentHeading -= 15;
                var response = await _httpClient.GetAsync($"/game/{token}/queue/[type=move,request={CurrentHeading}]");
            }
        }
        else
        {
            if (CurrentHeading == 360)
            {
                CurrentHeading = 15;
                var response = await _httpClient.GetAsync($"/game/{token}/queue/[type=move,request={CurrentHeading}]");
            }
            else
            {
                CurrentHeading += 15;
                var response = await _httpClient.GetAsync($"/game/{token}/queue/[type=move,request={CurrentHeading}]");
            }

        }
    }

    public async Task FireAsync(string token, string Weapon)
    {
        var response = await _httpClient.GetAsync($"/game/{token}/queue/[type=fire,request={Weapon}]");
    }
}

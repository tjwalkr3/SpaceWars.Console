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
        // Need to add in the idea that currentHeading could be 360 or greater rather than =
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
        Console.WriteLine($"Current Heading: {CurrentHeading}");
    }

    public async Task FireAsync(string token, string Weapon)
    {
        var response = await _httpClient.GetAsync($"/game/{token}/queue/[type=fire,request={Weapon}]");
        Console.WriteLine("FIRE!");
    }


    // public async Task<QueueActionResponse> QueueAction(string token, QueueActionRequest action)
    // {
    //     QueueActionResponse content = null;
    //     try
    //     {
    //         string url = $"/game/{token}/queue";
    //         var response = await _httpClient.PostAsJsonAsync(url, action);
    //         response.EnsureSuccessStatusCode();

    //         content = await response.Content.ReadFromJsonAsync<QueueActionResponse>();

            
    //     }
    //     catch(Exception ex)
    //     {
    //         Console.WriteLine($"Error: {ex.Message}");
    //     }

    //     if(content == null ) { return new("Error unable to find content for queue action"); }
    //     return content;
    // }

}

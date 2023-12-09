using System;
using System.Net.Http;
using System.Net.Http.Json;
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
        List<QueueActionRequest> request = null;
        string url = $"/game/{token}/queue";
        // Need to add in the idea that currentHeading could be 360 or greater rather than =
        if (direction == "left")
        {
            CurrentHeading -= 10;
            CurrentHeading = ClampRotation(CurrentHeading);
            request = [new("changeHeading", CurrentHeading.ToString())];
            var response = await _httpClient.PostAsJsonAsync(url, request);
            response.EnsureSuccessStatusCode();
        }
        else
        {
            CurrentHeading += 10;
            CurrentHeading = ClampRotation(CurrentHeading);
            request = [new("changeHeading", CurrentHeading.ToString())];
            var response = await _httpClient.PostAsJsonAsync(url, request);
            response.EnsureSuccessStatusCode();
        }
        Console.WriteLine($"Current Heading: {CurrentHeading}");
    }

    public async Task MoveForwardActionAsync(string token)
    {
        List<QueueActionRequest> request = null;
        string url = $"/game/{token}/queue";
        request = [new("move", CurrentHeading.ToString())];
        var response = await _httpClient.PostAsJsonAsync(url, request);
        response.EnsureSuccessStatusCode();
        Console.WriteLine($"Current Heading: {CurrentHeading}");
    }

    public async Task FireAsync(string token, string Weapon)
    {
        List<QueueActionRequest> request = null;
        request = [new("fire", Weapon)];
        string url = $"/game/{token}/queue";
        var response = await _httpClient.PostAsJsonAsync(url, request);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("FIRE!");
        }
        else
        {
            Console.WriteLine("Error: Unable to fire");
        }
    }

    private int ClampRotation(int rotation)
    {
        rotation = rotation % 360;
        if (rotation < 0)
            rotation += 360;
        return rotation;
    }

    public async Task ClearQueue(string token)
    {
        string url = $"/game/{token}/queue/clear";
        var response = await _httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Queue Cleared");
        }
        else
        {
            Console.WriteLine("Couldn't Clear Queue");
        }
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

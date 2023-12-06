using System;
using System.Net.Http;
using System.Net.Http.Json;
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

    public async Task<QueueActionResponse> QueueAction(string token, QueueActionRequest action)
    {
        QueueActionResponse content = null;
        try
        {
            string url = $"/game/{token}/queue";
            var response = await _httpClient.PostAsJsonAsync(url, action);
            response.EnsureSuccessStatusCode();

            content = await response.Content.ReadFromJsonAsync<QueueActionResponse>();

            
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        if(content == null ) { return new("Error unable to find content for queue action"); }
        return content;
    }

}

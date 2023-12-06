using System;
using System.Net.Http;

namespace SpaceWarsServices;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Hello, World!");

        Console.WriteLine("Please enter the URL to access Space Wars");
        // Enter URL to send requests
        var url = Console.ReadLine();

        bool exitGame = false;

        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri(url);

            // Create an instance of your ApiService (assuming ApiService takes HttpClient in its constructor)
            var service = new ApiService(httpClient);

            try
            {
                // Prompt for name
                Console.WriteLine("Please enter your name");
                var username = Console.ReadLine();
                var results = await service.JoinGameAsync(username);

                // Join game and return player token and ship location, board info (saving these)
                // You can call methods on the service to handle this part of the logic
                Console.WriteLine($"Token:{results.Token}");
                Console.WriteLine($"Ship located at: {results.StartingLocation}, Game State is: {results.GameState}, Board Demensions: {results.BoardWidth}, {results.BoardHeight}, ");

                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            while (!exitGame)
            {
                // Moving (need to make a move)
                // When a certain key is pressed, call the move function with the direction to move and update the location of the ship
                // You can handle this part of the logic using methods in the service as well

            }
        }

    }
}

// Assuming ApiService class structure
   
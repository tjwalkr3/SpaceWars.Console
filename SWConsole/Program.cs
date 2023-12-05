using System;
using System.Net.Http;

namespace SpaceWarsServices;

    class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello, World!");

            Console.WriteLine("Please enter the URL to access Space Wars");
            // Enter URL to send requests
            var url = Console.ReadLine();

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(url);

                try
                {
                    // Create an instance of your ApiService (assuming ApiService takes HttpClient in its constructor)
                    var service = new ApiService(httpClient);

                    // Prompt for name
                    Console.WriteLine("Please enter your name");
                    var username = Console.ReadLine();
                    var results = service.JoinGameAsync(username);

                    // Join game and return player token and ship location, board info (saving these)
                    // You can call methods on the service to handle this part of the logic

                    // Moving (need to make a move)
                    // When a certain key is pressed, call the move function with the direction to move and update the location of the ship
                    // You can handle this part of the logic using methods in the service as well
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }

    // Assuming ApiService class structure
   
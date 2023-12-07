using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SpaceWarsServices
{
    class Program
    {
        static void Main()
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            Console.WriteLine("Hello, World!");

            Console.WriteLine("Please enter the URL to access Space Wars");
            var url = Console.ReadLine();


            bool exitGame = false;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(url);
                var currentHeading = 0;
                var service = new ApiService(httpClient, currentHeading);
                var token = "";
                var CurrentWeapon = "";

                try
                {
                    Console.WriteLine("Please enter your name");
                    var username = Console.ReadLine();
                    var results = await service.JoinGameAsync(username);
                    token = results.Token;
                    service.CurrentHeading = results.Heading;
                    CurrentWeapon = "BasicCannon";
                    
                    Console.WriteLine($"Token:{results.Token}, Heading: {results.Heading}");
                    Console.WriteLine($"Ship located at: {results.StartingLocation}, Game State is: {results.GameState}, Board Dimensions: {results.BoardWidth}, {results.BoardHeight}");
                    
                    OpenUrlInBrowser($"{url}/hud?token={token}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                while (!exitGame)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true); // Read key without displaying it

                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.W:
                            // Handle 'W' key
                            break;
                        case ConsoleKey.A:
                            service.ChangeHeadingAsync("left", token);
                            break;
                        case ConsoleKey.S:
                            // Handle 'S' key
                            break;
                        case ConsoleKey.D:
                            service.ChangeHeadingAsync("right", token);
                            break;
                        case ConsoleKey.Spacebar:
                            service.FireAsync(token, CurrentWeapon);
                            break;
                    }
                }
            }
        }
        static void OpenUrlInBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening URL in browser: {ex.Message}");
            }
        }
    }
}

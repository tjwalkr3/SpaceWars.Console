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
                List<PurchasableItem> Shop = new List<PurchasableItem>(); ;

                try
                {
                    Console.WriteLine("Please enter your name");
                    var username = Console.ReadLine();
                    var results = await service.JoinGameAsync(username);
                    token = results.Token;
                    service.CurrentHeading = results.Heading;
                    CurrentWeapon = "Basic Cannon";

                    Shop = results.Shop.Select(item => new PurchasableItem
                    {
                        Cost = item.Cost,
                        Name = item.Name,
                        PurchasePrerequisites = item.PurchasePrerequisites
                    }).ToList();

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
                            await service.MoveForwardActionAsync(token);
                            break;
                        case ConsoleKey.A:
                            await service.ChangeHeadingAsync("left", token);
                            break;
                        case ConsoleKey.S:
                            // Handle 'S' key
                            break;
                        case ConsoleKey.D:
                            await service.ChangeHeadingAsync("right", token);
                            break;
                        case ConsoleKey.Spacebar:
                            await service.FireAsync(token, CurrentWeapon);
                            break;
                        case ConsoleKey.C:
                            await service.ClearQueue(token);
                            break;
                        case ConsoleKey.R:
                            await service.RepairAsync(token);
                            break;
                        case ConsoleKey.U:
                            foreach (var item in Shop)
                            {
                                Console.WriteLine($"upgrade: {item.Name}, cost: {item.Cost}");
                            }
                            break;
                        case ConsoleKey.P:
                            Console.WriteLine("enter what you'd like to purphase from the shop, (if you've changed your mind enter x)");
                            var response = Console.ReadLine();
                            if (response == "x")
                            {
                                continue;
                            }
                            else
                            {
                                if (Shop.Any(item => item.Name.Equals(response, StringComparison.OrdinalIgnoreCase) ||
                                                                  (item.PurchasePrerequisites != null && item.PurchasePrerequisites.Contains(response, StringComparer.OrdinalIgnoreCase))))
                                {
                                    await service.PurchaseAction(token, response);
                                }
                                else
                                {
                                    Console.WriteLine("Invalid item. Please choose a valid item from the shop.");
                                }
                            }
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

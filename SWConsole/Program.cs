using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using SWConsole;

namespace SpaceWarsServices;

class Program
{
    static void Main()
    {
        MainAsync().Wait();
    }

    static async Task MainAsync()
    {
        Console.WriteLine("Please enter the URL to access Space Wars");
        var url = Console.ReadLine();

        bool exitGame = false;

        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri(url);
            var currentHeading = 0;
            var token = "";
            var CurrentWeapon = "";
            var service = new ApiService(httpClient);
            List<PurchasableItem> Shop = new List<PurchasableItem>();

            try
            {
                Console.WriteLine("Please enter your name");
                var username = Console.ReadLine();
                var results = await service.JoinGameAsync(username);
                token = results.Token;
                CurrentWeapon = "Basic Cannon";

                Shop = results.Shop.Select(item => new PurchasableItem(item.Cost, item.Name, item.Prerequisites)).ToList();

                Console.WriteLine($"Token:{results.Token}, Heading: {results.Heading}");
                Console.WriteLine($"Ship located at: {results.StartingLocation}, Game State is: {results.GameState}, Board Dimensions: {results.BoardWidth}, {results.BoardHeight}");
                
                OpenUrlInBrowser($"{url}/hud?token={token}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            var gameActions = new GameActions(currentHeading, service, token);

            while (!exitGame)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true); // Read key without displaying it
                bool shiftPressed = keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.W:
                        await gameActions.MoveForwardAsync(shiftPressed);
                        break;
                    case ConsoleKey.A:
                        await gameActions.RotateLeftAsync(shiftPressed);
                        break;
                    case ConsoleKey.D:
                        await gameActions.RotateRightAsync(shiftPressed);
                        break;
                    case ConsoleKey.Spacebar:
                        await gameActions.FireWeaponAsync(CurrentWeapon);
                        break;
                    case ConsoleKey.C:
                        await gameActions.ClearQueueAsync();
                        break;
                    case ConsoleKey.U:
                        foreach (var item in Shop)
                        {
                            Console.WriteLine($"upgrade: {item.Name}, cost: {item.Cost}");
                        }
                        break;
                    case ConsoleKey.P:

                        Console.WriteLine("please enter what you'd like to purchase from the shop, (if you've changed your mind enter x)");
                        var response = Console.ReadLine();
                        if (response == "x")
                        {
                            continue;
                        }

                        if (Shop.Any(item => item.Name.Equals(response, StringComparison.OrdinalIgnoreCase) ||
                            (item.Prerequisites != null && item.Prerequisites.Contains(response, StringComparer.OrdinalIgnoreCase))))
                        {
                            await gameActions.PurchaseItemAsync(response);
                        }
                        else
                        {
                            Console.WriteLine("Invalid item. Please choose a valid item from the shop.");
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

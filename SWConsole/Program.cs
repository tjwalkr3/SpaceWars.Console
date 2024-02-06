using SWConsole;
using System.Diagnostics;

namespace SpaceWarsServices;

class Program
{
    static async Task Main(string[] args)
    {
        Uri baseAddress = getApiBaseAddress(args);

        bool exitGame = false;
        ConsoleKey forwardKey = ConsoleKey.UpArrow;
        ConsoleKey leftKey = ConsoleKey.LeftArrow;
        ConsoleKey rightKey = ConsoleKey.RightArrow;
        ConsoleKey fireKey = ConsoleKey.Spacebar;
        ConsoleKey clearQueueKey = ConsoleKey.C;
        ConsoleKey infoKey = ConsoleKey.I;
        ConsoleKey shopKey = ConsoleKey.S;
        ConsoleKey repairKey = ConsoleKey.R;

        using HttpClient httpClient = new HttpClient() { BaseAddress = baseAddress };

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

            OpenUrlInBrowser($"{baseAddress.AbsoluteUri}hud?token={token}");
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
                case var key when key == forwardKey:
                    await gameActions.MoveForwardAsync(shiftPressed);
                    break;
                case var key when key == leftKey:
                    await gameActions.RotateLeftAsync(shiftPressed);
                    break;
                case var key when key == rightKey:
                    await gameActions.RotateRightAsync(shiftPressed);
                    break;
                case var key when key == fireKey:
                    await gameActions.FireWeaponAsync(CurrentWeapon);
                    break;
                case var key when key == clearQueueKey:
                    await gameActions.ClearQueueAsync();
                    break;
                case var key when key == repairKey:
                    await gameActions.RepairShipAsync();
                    break;
                case var key when key == infoKey:
                    foreach (var item in Shop)
                    {
                        Console.WriteLine($"upgrade: {item.Name}, cost: {item.Cost}");
                    }
                    break;
                case var key when key == shopKey:

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

    private static Uri getApiBaseAddress(string[] args)
    {
        Uri baseAddress;
        if (args.Length == 0)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Please enter the URL to access Space Wars");
                    baseAddress = new Uri(Console.ReadLine());
                    break;
                }
                catch { }
            }
        }
        else
        {
            baseAddress = new Uri(args[0]);
        }
        return baseAddress;
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

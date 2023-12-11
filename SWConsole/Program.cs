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
        const ConsoleKey defaultforwardKey = ConsoleKey.W, defaultleftKey = ConsoleKey.A, defaultrightKey = ConsoleKey.D, defaultfireKey = ConsoleKey.Spacebar, defaultclearQueueKey = ConsoleKey.C, defaultinfoKey = ConsoleKey.U, defaultshopKey = ConsoleKey.P, defaultRepairKey = ConsoleKey.R;
        ConsoleKey forwardKey = defaultforwardKey, leftKey = defaultleftKey, rightKey = defaultrightKey, fireKey = defaultfireKey, clearQueueKey = defaultclearQueueKey, infoKey = defaultinfoKey, shopKey = defaultshopKey, repairKey = defaultRepairKey;


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

                Console.WriteLine("Would you like to set your own key binding?\n or stick with the default?");
                Console.WriteLine("Default: W,A,D,Space,C,U,P,Shift");
                Console.WriteLine("Y or N: ");
                var responseKeyQuestion = Console.ReadLine();
                if (responseKeyQuestion.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("What key would you like to move forward: ");
                    forwardKey = Console.ReadKey().Key;
                    Console.WriteLine("\nhat key would you like to move left: ");
                    leftKey = Console.ReadKey().Key;
                    Console.WriteLine("\nWhat key would you like to move right: ");
                    rightKey = Console.ReadKey().Key;
                    Console.WriteLine("\nWhat key would you like to fire: ");
                    fireKey = Console.ReadKey().Key;
                    Console.WriteLine("\nWhat key would you like to clear the queue: ");
                    clearQueueKey = Console.ReadKey().Key;
                    Console.WriteLine("\nWhat key would you like to see upgrad info: ");
                    infoKey = Console.ReadKey().Key;
                    Console.WriteLine("\nWhat key would you like to open the shop: ");
                    shopKey = Console.ReadKey().Key;
                    Console.WriteLine("\nWhat key would you like to repair: ");
                    repairKey = Console.ReadKey().Key;
                }

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

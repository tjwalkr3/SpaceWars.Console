﻿using SWConsole;
using System.Diagnostics;

namespace SpaceWarsServices;

class Program
{
    static async Task Main(string[] args)
    {
        //**************************************************************************************
        //***  |    |    |    |                                            |    |    |    |    |
        //***  |    |    |    |       Change your key mappings here        |    |    |    |    |
        //***  V    V    V    V                                            V    V    V    V    V
        //**************************************************************************************
        const ConsoleKey forwardKey = ConsoleKey.UpArrow;
        const ConsoleKey forwardFastKey = ConsoleKey.W;
        const ConsoleKey backKey = ConsoleKey.DownArrow;
        const ConsoleKey backFastKey = ConsoleKey.S;
        const ConsoleKey leftKey = ConsoleKey.LeftArrow;
        const ConsoleKey leftFastKey = ConsoleKey.A;
        const ConsoleKey rightKey = ConsoleKey.RightArrow;
        const ConsoleKey rightFastKey = ConsoleKey.D;
        const ConsoleKey fireKey = ConsoleKey.Spacebar;
        const ConsoleKey clearQueueKey = ConsoleKey.C;
        const ConsoleKey randomWalkKey = ConsoleKey.Q;
        const ConsoleKey infoKey = ConsoleKey.I;
        const ConsoleKey shopKey = ConsoleKey.F;
        const ConsoleKey repairKey = ConsoleKey.R;
        const ConsoleKey repairFastKey = ConsoleKey.Tab;
        const ConsoleKey readAndEmptyMessagesKey = ConsoleKey.M;
        const ConsoleKey triangleKey = ConsoleKey.F1;
        const ConsoleKey squareKey = ConsoleKey.F2;
        const ConsoleKey octagonKey = ConsoleKey.F3;

        Uri baseAddress = getApiBaseAddress(args);
        using HttpClient httpClient = new HttpClient() { BaseAddress = baseAddress };
        bool exitGame = false;
        var currentHeading = 0;
        var token = "";
        var service = new ApiService(httpClient);
        List<PurchasableItem> Shop = new List<PurchasableItem>();
        JoinGameResponse joinGameResponse = null;

        Console.WriteLine("Please enter your name");
        var username = Console.ReadLine();
        try
        {
            joinGameResponse = await service.JoinGameAsync(username);
            token = joinGameResponse.Token;

            Shop = joinGameResponse.Shop.Select(item => new PurchasableItem(item.Cost, item.Name, item.Prerequisites)).ToList();

            Console.WriteLine($"Token:{joinGameResponse.Token}, Heading: {joinGameResponse.Heading}");
            Console.WriteLine($"Ship located at: {joinGameResponse.StartingLocation}, Game State is: {joinGameResponse.GameState}, Board Dimensions: {joinGameResponse.BoardWidth}, {joinGameResponse.BoardHeight}");

            OpenUrlInBrowser($"{baseAddress.AbsoluteUri}hud?token={token}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        var gameActions = new GameActions(username, joinGameResponse, service);
        gameActions.Weapons.Add("Basic Cannon");
        gameActions.CurrentWeapon = "Basic Cannon";

        while (!exitGame)
        {
            printStatus();
            ConsoleKeyInfo keyInfo = Console.ReadKey(true); // Read key without displaying it
            bool shiftPressed = keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift);

            switch (keyInfo.Key)
            {
                case var key when key == forwardKey:
                    await gameActions.MoveForwardAsync(1);
                    break;
                case var key when key == forwardFastKey:
                    if (shiftPressed)
                        await gameActions.MoveForwardAsync(20);
                    else
                        await gameActions.MoveForwardAsync(10);
                    break;
                case var key when key == backKey:
                    await gameActions.Rotate(Direction.Left, 180);
                    await gameActions.MoveForwardAsync(1);
                    break;
                case var key when key == backFastKey:
                    await gameActions.Rotate(Direction.Left, 180);
                    if (shiftPressed)
                        await gameActions.MoveForwardAsync(20);
                    else
                        await gameActions.MoveForwardAsync(10);
                    break;
                case var key when key == leftKey:
                    await gameActions.Rotate(Direction.Left, 1);
                    break;
                case var key when key == leftFastKey:
                    if (shiftPressed)
                        await gameActions.Rotate(Direction.Left, 45);
                    else
                        await gameActions.Rotate(Direction.Left, 10);
                    break;
                case var key when key == rightKey:
                    await gameActions.Rotate(Direction.Right, 1);
                    break;
                case var key when key == rightFastKey:
                    if (shiftPressed)
                        await gameActions.Rotate(Direction.Right, 45);
                    else
                        await gameActions.Rotate(Direction.Right, 10);
                    break;
                case var key when key == fireKey:
                    await gameActions.FireWeaponAsync();
                    if (shiftPressed)
                    {
                        await gameActions.ClearQueueAsync();
                        for (int i = 0; i < 9; i++)
                        {
                            await gameActions.FireWeaponAsync();
                        }
                    }
                    break;
                case var key when key == clearQueueKey:
                    await gameActions.ClearQueueAsync();
                    break;
                case var key when key == repairKey:
                    await gameActions.RepairShipAsync();
                    Console.WriteLine("Ship repair requested.");
                    break;
                case var key when key == repairFastKey:
                    for (int i = 0; i < 6; i++)
                    {
                        await gameActions.RepairShipAsync();
                    }
                    Console.WriteLine("6x Repair Requested");
                    break;
                case var key when key == infoKey:
                    foreach (var item in Shop)
                    {
                        Console.WriteLine($"upgrade: {item.Name}, cost: {item.Cost}");
                        Console.WriteLine("Press any key to continue.");
                        Console.ReadKey();
                    }
                    break;
                case var key when key == shopKey:

                    Console.WriteLine("please enter what you'd like to purchase from the shop, (if you've changed your mind enter x)");
                    var response = Console.ReadLine();
                    if (response == "x")
                    {
                        continue;
                    }

                    if (Shop.Any(item => item.Name.Equals(response, StringComparison.OrdinalIgnoreCase)))
                    {
                        await gameActions.PurchaseItemAsync(response);
                        Console.WriteLine($"Purchase of {response} requested.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid item. Please choose a valid item from the shop.");
                    }
                    break;
                case var key when key == readAndEmptyMessagesKey:
                    await gameActions.ReadAndEmptyMessagesAsync();
                    Console.WriteLine("Message queue read.");
                    break;
                case var key when key >= ConsoleKey.D0 && key <= ConsoleKey.D9: // The 0 key through the 9 key
                    gameActions.SelectWeapon(key);
                    Console.WriteLine($"Selected weapon {((char)key) - '1'} ({gameActions.CurrentWeapon}");
                    break;
                //**************************************************************************************
                //***  |    |    |    |                                            |    |    |    |    |
                //***  |    |    |    |       Add any other custom keys here       |    |    |    |    |
                //***  V    V    V    V                                            V    V    V    V    V
                //**************************************************************************************
                case var key when key == randomWalkKey:
                    if (shiftPressed)
                        RandomWalk(gameActions, 40);
                    else
                        RandomWalk(gameActions, 20);
                    break;
                case var key when key == triangleKey:
                    Polygon(gameActions, 3, 30);
                    break;
                case var key when key == squareKey:
                    Polygon(gameActions, 4, 30);
                    break;
                case var key when key == octagonKey:
                    Polygon(gameActions, 8, 20);
                    break;
            }
        }

        async void RandomWalk(GameActions gameActions, int numOfActions)
        {
            Random rnd = new Random();

            for (int i = 0; i < numOfActions; i++)
            {
                bool leftRight = rnd.Next(2) == 0;
                int angle = rnd.Next(360);
                int distance = rnd.Next(15);

                if (leftRight)
                    await gameActions.Rotate(Direction.Left, angle);
                else
                    await gameActions.Rotate(Direction.Right, angle);

                await gameActions.MoveForwardAsync(distance);
            }
        }

        async void Polygon(GameActions gameActions, int numOfSides, int sideLength)
        {
            Random rnd = new Random();

            for (int i = 0; i < numOfSides; i++)
            {
                await gameActions.MoveForwardAsync(sideLength);
                await gameActions.Rotate(Direction.Right, (360 / numOfSides));
            }
        }

        void printStatus()
        {
            Console.Clear();
            Console.WriteLine($"Name: {username,-32} Token: {gameActions.Token}");
            Console.WriteLine($"Left: {leftKey,-12} Right: {rightKey,-12} Forward: {forwardKey,-12} Fire: {fireKey,-24} Clear Queue: {clearQueueKey,-6}");
            Console.WriteLine($"Fast Left: {leftFastKey,-7} Fast Right: {rightFastKey,-7} Fast Forward: {forwardFastKey,-7} Rapid Fire: Shift+{fireKey,-12} Fast Repair: {repairFastKey,-6}");
            Console.WriteLine($"Info: {infoKey,-11}  Shop: {shopKey,-12}  Repair: {repairKey,-13} Read & Empty Messages: {readAndEmptyMessagesKey,-6}  Random Walk: {randomWalkKey,-6}");

            for (int i = 0; i < gameActions.Weapons.Count; i++)
            {
                string? weapon = gameActions.Weapons[i];
                if (weapon == gameActions.CurrentWeapon)
                {
                    weapon = $"**{weapon}**";
                }
                Console.Write($"{i + 1}: {weapon}   ");
            }
            Console.WriteLine();


            if (gameActions.GameMessages.Any())
            {
                Console.WriteLine();
                Console.WriteLine("Last 10 messages:");
                Console.WriteLine(new string('-', Console.WindowWidth));
                foreach (var msg in gameActions.GameMessages.TakeLast(10))
                {
                    Console.WriteLine($"{msg.Type,-30} {msg.Message}");
                }
            }
            Console.WriteLine(new string('=', Console.WindowWidth));
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

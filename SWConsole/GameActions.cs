using SpaceWarsServices;

namespace SWConsole;

public class GameActions
{
    private int Heading;
    private ApiService ApiService;
    private string Token;

    public GameActions(int heading, ApiService apiService, string token)
    {
        Heading = heading;
        ApiService = apiService;
        Token = token;
    }

    public async Task RotateLeftAsync(bool quickTurn)
    {
        if (quickTurn)
            Heading -= 10;
        else
            Heading -= 1;

        Heading = ClampRotation(Heading);
        List<QueueActionRequest> action = [new("changeHeading", Heading.ToString())];
        await ApiService.QueueAction(Token, action);
    }

    public async Task RotateRightAsync(bool quickTurn)
    {
        if (quickTurn)
            Heading += 10;
        else
            Heading += 1;

        Heading = ClampRotation(Heading);
        List<QueueActionRequest> action = [new("changeHeading", Heading.ToString())];
        await ApiService.QueueAction(Token, action);
    }

    public async Task MoveForwardAsync(bool lightSpeed)
    {
        List<QueueActionRequest> action = new();
        if (lightSpeed)
        {
            for(int i = 0; i < 10; i++)
            {
                action.Add(new("move", Heading.ToString()));
            }
        }
        else
            action.Add(new("move", Heading.ToString()));

        Heading = ClampRotation(Heading);
        await ApiService.QueueAction(Token, action);
    }

    public async Task FireWeaponAsync(string weapon)
    {
        List<QueueActionRequest> action = [new("fire", weapon)];
        await ApiService.QueueAction(Token, action);
    }

    public async Task RepairShipAsync()
    {
        List<QueueActionRequest> action = [new("repair", null)];
        await ApiService.QueueAction(Token, action);
    }

    public async Task ClearQueueAsync()
    {
        List<QueueActionRequest> action = [new("clear", null)];
        await ApiService.QueueAction(Token, action);
    }

    public async Task PurchaseItemAsync(string item)
    {
        List<QueueActionRequest> action = [new("purchase", item)];
        await ApiService.QueueAction(Token, action);
    }

    private int ClampRotation(int rotation)
    {
        rotation = rotation % 360;
        if (rotation < 0)
            rotation += 360;
        return rotation;
    }
}

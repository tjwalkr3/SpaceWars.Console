namespace SpaceWarsServices;

public record JoinGameResponse(string Token, Location StartingLocation, string GameState, int BoardWidth, int BoardHeight);
public record Location(int X, int Y);
public record GameStateResponse(string GameState, IEnumerable<Location> PlayerLocations);
public record PlayerMessageResponse(string Type, string Message);
public record QueueActionRequest(string Type, string? Request);
public record QueueActionResponse(string Message);
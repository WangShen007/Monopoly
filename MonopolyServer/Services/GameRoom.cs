using MonopolyModels.Entities;
using MonopolyModels.Dtos;
using MonopolyModels.States;

namespace MonopolyServer.Services;

public class GameRoom
{
    public int RoomId { get; init; }
    public string RoomName { get; set; } = string.Empty;
    public int OwnerUserId { get; init; }
    public int MaxPlayers { get; set; }
    public string Status { get; set; } = "Waiting";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime StartedAt { get; set; }
    public int? WinnerUserId { get; set; }
    public string? WinnerUserName { get; set; }
    public string? EndReason { get; set; }
    public List<ClientUser> Players { get; } = [];
    public List<PlayerState> PlayerStates { get; } = [];
    public List<MapCell> MapCells { get; set; } = [];
    public List<PropertyState> PropertyStates { get; set; } = [];
    public List<EventCard> EventCards { get; set; } = [];
    public List<string> Logs { get; } = [];
    public List<PendingActionRecord> PendingActions { get; } = [];
    public List<NetMessage> PendingNotifications { get; } = [];
    public int CurrentPlayerIndex { get; set; }
    public int RoundNumber { get; set; } = 1;
    public bool CurrentPlayerRolled { get; set; }
    public bool RecordsSaved { get; set; }
    public object Gate { get; } = new();
}

public record PendingActionRecord(int UserId, int RoundNumber, string ActionType, string Description, DateTime CreatedAt);

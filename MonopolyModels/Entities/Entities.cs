namespace MonopolyModels.Entities;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int TotalGames { get; set; }
    public int TotalMoney { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class Room
{
    public int Id { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public int OwnerUserId { get; set; }
    public int MaxPlayers { get; set; }
    public string Status { get; set; } = "Waiting";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class MapCell
{
    public int Id { get; set; }
    public int CellIndex { get; set; }
    public string CellName { get; set; } = string.Empty;
    public string CellType { get; set; } = "Empty";
    public string Description { get; set; } = string.Empty;
}

public class Property
{
    public int Id { get; set; }
    public int MapCellId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public int Price { get; set; }
    public int Rent { get; set; }
    public string ColorGroup { get; set; } = string.Empty;
    public MapCell? MapCell { get; set; }
}

public class EventCard
{
    public int Id { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public int Value { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
}

public class GameRecord
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int? WinnerUserId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public string EndReason { get; set; } = string.Empty;
    public List<PlayerGameRecord> PlayerGameRecords { get; set; } = [];
    public List<ActionRecord> ActionRecords { get; set; } = [];
}

public class PlayerGameRecord
{
    public int Id { get; set; }
    public int GameRecordId { get; set; }
    public int UserId { get; set; }
    public int FinalMoney { get; set; }
    public int PropertyCount { get; set; }
    public int Rank { get; set; }
    public bool IsWinner { get; set; }
    public GameRecord? GameRecord { get; set; }
}

public class ActionRecord
{
    public int Id { get; set; }
    public int GameRecordId { get; set; }
    public int UserId { get; set; }
    public int RoundNumber { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public GameRecord? GameRecord { get; set; }
}

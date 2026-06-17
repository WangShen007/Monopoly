namespace MonopolyModels.States;

public class PlayerState
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int Position { get; set; }
    public int Money { get; set; }
    public bool IsBankrupt { get; set; }
    public bool IsReady { get; set; }
    public int FreeRentCards { get; set; }
    public int SkipTurnRounds { get; set; }
    public string TokenImageFile { get; set; } = "棋子-红.png";
}

public class PropertyState
{
    public int PropertyId { get; set; }
    public int MapCellId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public int? OwnerUserId { get; set; }
    public string? OwnerUserName { get; set; }
    public int Price { get; set; }
    public int Rent { get; set; }
    public string ColorGroup { get; set; } = string.Empty;
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonopolyModels.Dtos;

public static class JsonProtocol
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}

public class NetMessage
{
    public string Type { get; set; } = string.Empty;
    public string Data { get; set; } = "{}";

    public static NetMessage Create<T>(string type, T data)
    {
        return new NetMessage
        {
            Type = type,
            Data = JsonSerializer.Serialize(data, JsonProtocol.Options)
        };
    }

    public T ReadData<T>()
    {
        return JsonSerializer.Deserialize<T>(Data, JsonProtocol.Options)
            ?? throw new InvalidOperationException($"消息 {Type} 的Data无法反序列化为 {typeof(T).Name}");
    }
}

public record EmptyDto;
public record AuthRequest(string UserName, string Password, string? TokenImageFile = null);
public record BasicResult(bool Success, string Message);
public record LoginResult(bool Success, string Message, int UserId, string UserName);
public record CreateRoomRequest(string RoomName, int MaxPlayers);
public record JoinRoomRequest(int RoomId);
public record EntityIdRequest(int Id);
public record ChatRequest(string Text);
public record ReactionRequest(string ReactionType);

public record RoomSummaryDto(
    int RoomId,
    string RoomName,
    string OwnerUserName,
    int MaxPlayers,
    string Status,
    int PlayerCount,
    int ReadyCount);

public record RoomListResult(List<RoomSummaryDto> Rooms);

public record MapCellDto(
    int Id,
    int CellIndex,
    string CellName,
    string CellType,
    string Description);

public record PropertyDto(
    int Id,
    int MapCellId,
    int CellIndex,
    string PropertyName,
    int Price,
    int Rent,
    string ColorGroup);

public record EventCardDto(
    int Id,
    string EventName,
    string EventType,
    int Value,
    string Description,
    bool IsEnabled);

public record MapCellEditDto(int Id, int CellIndex, string CellName, string CellType, string Description);
public record PropertyEditDto(int Id, int MapCellId, string PropertyName, int Price, int Rent, string ColorGroup);
public record EventCardEditDto(int Id, string EventName, string EventType, int Value, string Description, bool IsEnabled);

public record PlayerStateDto(
    int UserId,
    string UserName,
    int Position,
    int Money,
    bool IsBankrupt,
    bool IsReady,
    int OwnedProperties,
    int FreeRentCards,
    string TokenImageFile = "棋子-1.png");

public record PropertyStateDto(
    int PropertyId,
    int MapCellId,
    string PropertyName,
    int Price,
    int Rent,
    string ColorGroup,
    int? OwnerUserId,
    string? OwnerUserName);

public record GameStateDto(
    int RoomId,
    string Status,
    int RoundNumber,
    int? CurrentPlayerUserId,
    string CurrentPlayerName,
    List<MapCellDto> MapCells,
    List<PropertyStateDto> Properties,
    List<PlayerStateDto> Players,
    List<string> Logs,
    bool CanBuyProperty);

public record DiceResultDto(int UserId, string UserName, int Dice, int OldPosition, int NewPosition);
public record MoveResultDto(int UserId, string UserName, int OldPosition, int NewPosition, bool PassedStart, int Money);
public record BuyPropertyResultDto(bool Success, string Message, int UserId, int PropertyId);
public record RentPaidDto(int FromUserId, int ToUserId, int Amount, bool UsedFreeRentCard);
public record ChanceResultDto(int UserId, string EventName, string EventType, int Value, string Description);
public record TaxResultDto(int UserId, int Amount);
public record PlayerBankruptDto(int UserId, string UserName);
public record GameOverDto(int WinnerUserId, string WinnerUserName, string EndReason, List<PlayerStateDto> Ranking);
public record ChatMessageDto(
    int RoomId,
    int SenderUserId,
    string SenderUserName,
    string TokenImageFile,
    string MessageType,
    string Text,
    DateTime SentAt);

public record ManageDataDto(
    List<MapCellDto> MapCells,
    List<PropertyDto> Properties,
    List<EventCardDto> EventCards);

public record RankUserDto(
    int UserId,
    string UserName,
    int Wins,
    int Losses,
    int TotalGames,
    int TotalMoney,
    double WinRate);

public record RankListResult(List<RankUserDto> Users);

public record HistoryRecordDto(
    int GameRecordId,
    int RoomId,
    string WinnerName,
    DateTime StartedAt,
    DateTime EndedAt,
    string EndReason);

public record HistoryResult(List<HistoryRecordDto> Records);

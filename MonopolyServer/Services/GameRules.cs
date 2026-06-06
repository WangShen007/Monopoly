using MonopolyModels.Entities;
using MonopolyModels.States;

namespace MonopolyServer.Services;

public static class GameRules
{
    public const int InitialMoney = 1500;
    public const int StartReward = 200;
    public const int TaxAmount = 150;
    public const int MaxRounds = 60;
    public const int MapCellCount = 28;

    public static (int NewPosition, bool PassedStart) Move(PlayerState player, int steps, int mapCellCount = MapCellCount)
    {
        var raw = player.Position + steps;
        var passedStart = steps > 0 && raw >= mapCellCount;
        player.Position = ((raw % mapCellCount) + mapCellCount) % mapCellCount;

        if (passedStart)
        {
            player.Money += StartReward;
        }

        return (player.Position, passedStart);
    }

    public static int CalculateAsset(PlayerState player, IEnumerable<PropertyState> properties)
    {
        return player.Money + properties.Where(x => x.OwnerUserId == player.UserId).Sum(x => x.Price);
    }

    public static PlayerState? GetWinnerByAsset(IEnumerable<PlayerState> players, IEnumerable<PropertyState> properties)
    {
        return players
            .OrderBy(x => x.IsBankrupt)
            .ThenByDescending(x => CalculateAsset(x, properties))
            .ThenBy(x => x.UserId)
            .FirstOrDefault();
    }

    public static void ApplyEvent(PlayerState player, EventCard card, Action<int> moveRelative, Action goToStart)
    {
        switch (card.EventType)
        {
            case "AddMoney":
                player.Money += card.Value;
                break;
            case "MinusMoney":
                player.Money -= card.Value;
                break;
            case "MoveForward":
                moveRelative(Math.Abs(card.Value));
                break;
            case "MoveBackward":
                moveRelative(-Math.Abs(card.Value));
                break;
            case "GoToStart":
                goToStart();
                player.Money += card.Value;
                break;
            case "FreeRent":
                player.FreeRentCards += Math.Max(1, card.Value);
                break;
        }
    }
}

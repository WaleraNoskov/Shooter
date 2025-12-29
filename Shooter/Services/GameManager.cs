using Shooter.Contracts;

namespace Shooter.Services;

public class GameManager
{
    public GameStatus Status { get; set; }

    public int? WinnerPlayerIndex { get; set; }
    
    public int? LoosePlayerIndex { get; set; }
}
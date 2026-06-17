using System.Media;

namespace MonopolyClient.Visuals;

public sealed class GameSoundPlayer
{
    private DateTime _lastPlayedAtUtc = DateTime.MinValue;

    public bool Enabled { get; set; } = true;

    public void Play(GameSound sound)
    {
        if (!Enabled)
        {
            return;
        }

        var now = DateTime.UtcNow;
        if ((now - _lastPlayedAtUtc).TotalMilliseconds < 80)
        {
            return;
        }

        _lastPlayedAtUtc = now;
        try
        {
            switch (sound)
            {
                case GameSound.Dice:
                    SystemSounds.Asterisk.Play();
                    break;
                case GameSound.Move:
                    SystemSounds.Question.Play();
                    break;
                case GameSound.Success:
                    SystemSounds.Exclamation.Play();
                    break;
                case GameSound.Cost:
                    SystemSounds.Hand.Play();
                    break;
                case GameSound.Message:
                    SystemSounds.Beep.Play();
                    break;
                case GameSound.GameOver:
                    SystemSounds.Exclamation.Play();
                    break;
            }
        }
        catch
        {
            // Sound feedback is optional; keep gameplay responsive if the OS audio device is unavailable.
        }
    }
}

public enum GameSound
{
    Dice,
    Move,
    Success,
    Cost,
    Message,
    GameOver
}

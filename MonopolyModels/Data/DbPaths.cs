namespace MonopolyModels.Data;

public static class DbPaths
{
    public const string EnvironmentVariable = "MONOPOLY_DB_PATH";

    public static string GetDefaultDatabasePath()
    {
        var configured = Environment.GetEnvironmentVariable(EnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return Path.GetFullPath(configured);
        }

        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(appData, "MonopolyGame", "monopoly.db");
    }
}

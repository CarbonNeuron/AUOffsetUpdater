using Config.Net;
using SteamKit2;

namespace AUOffsetUpdater
{
    public static class Settings
    {
        public static IPersistentSettings PSettings =
            new ConfigurationBuilder<IPersistentSettings>().UseJsonFile("Settings.json").Build();
    }

    public interface IPersistentSettings
    {
        [Option(Alias = "Steam.Username")]
        string SteamUsername { get; set; }
        
        [Option(Alias = "Steam.Password")]
        string SteamPassword { get; set; }
        
        [Option(Alias = "LatestManifestDownloaded", DefaultValue = "0")]
        string latestManifestDownloaded { get; set; }
    }
}
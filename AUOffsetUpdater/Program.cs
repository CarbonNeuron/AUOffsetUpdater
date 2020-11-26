using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DepotDownloader;
using SteamKit2;
using Config = Il2CppDumper.Config;

namespace AUOffsetUpdater
{
    class Program
    {

        
        static async Task Main(string[] args)
        {
            ContentDownloader.Config.RememberPassword = true;
            ContentDownloader.Config.DownloadAllLanguages = true;
            ContentDownloader.Config.DownloadAllPlatforms = true;
            ContentDownloader.Config.UsingFileList = true;
            ContentDownloader.Config.FilesToDownload = new List<string>();
            ContentDownloader.Config.FilesToDownloadRegex = new List<Regex>();
            ContentDownloader.Config.FilesToDownload.Add("GameAssembly.dll");
            ContentDownloader.Config.InstallDirectory = "Download";
            if (OperatingSystem.IsWindows())
            {
                
                ContentDownloader.Config.FilesToDownload.Add("Among Us_Data\\il2cpp_data\\Metadata\\global-metadata.dat");
            }
            else
            {
                ContentDownloader.Config.FilesToDownload.Add("Among Us_Data/il2cpp_data/Metadata/global-metadata.dat");
            }

            ContentDownloader.Config.MaxServers = 20;
            ContentDownloader.Config.MaxDownloads = 8;
            ContentDownloader.Config.MaxServers = Math.Max( ContentDownloader.Config.MaxServers, ContentDownloader.Config.MaxDownloads );
            AccountSettingsStore.LoadFromFile( "account.config" );
            
            
            
            if (DepotDownloader.Program.InitializeSteam(Settings.PSettings.SteamUsername,
                Settings.PSettings.SteamPassword))
            {
                uint appID = 945360;
                uint DepotID = 945361;
                ContentDownloader.steam3.RequestAppInfo( appID );
                var LatestmanifestID = ContentDownloader.GetSteam3DepotManifest(DepotID, appID, "public");
                if (LatestmanifestID != ulong.Parse(Settings.PSettings.latestManifestDownloaded)) //New update
                {
                    List<(uint, ulong)> depotManifestIds = new List<(uint, ulong)>();
                    depotManifestIds.Add((DepotID, LatestmanifestID));
                    await ContentDownloader.DownloadAppAsync(appID, depotManifestIds, "public", null, null, null, false, false).ConfigureAwait(false);
                }
                AccountSettingsStore.Save();
                string GameAssemblyPath = Path.Join(ContentDownloader.Config.InstallDirectory, ContentDownloader.Config.FilesToDownload[0]);
                string MetadataPath = Path.Join(ContentDownloader.Config.InstallDirectory, ContentDownloader.Config.FilesToDownload[1]);
                System.IO.Directory.CreateDirectory("Dumped");
                Il2CppDumper.Il2CppDumper.PerformDump(GameAssemblyPath, MetadataPath, "Dumped\\",
                    new Il2CppDumper.Config(), ReportProgressAction);
                ContentDownloader.ShutdownSteam3();
            }
            Console.ReadKey();
        }

        private static void ReportProgressAction(string obj)
        {
            Console.WriteLine(obj);
        }
    }
}

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;

public static class OperatingSystem
{
    public static bool IsWindows() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    public static bool IsMacOS() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    public static bool IsLinux() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
}

namespace KAG
{
    static class AutoUpdate
    {
        static string GitOwner = "kag-rewritten";
        static string GitRepository = "kag-rewritten";
        static string GitDownloadURL = "https://github.com/{0}/{1}/releases/latest/download/KAG.Rewritten-{2}.zip";
        static string GitChecksumURL = "https://github.com/{0}/{1}/releases/latest/download/KAG.Rewritten-{2}-md5.txt";

        static string GameZip = "KAG Rewritten.zip";
        static string GameVersion = "Version.txt";

        static void Main(string[] args)
        {
            string platform = "";

            if (OperatingSystem.IsWindows())
            {
                platform = "windows";
            }
            else if (OperatingSystem.IsLinux())
            {
                platform = "linux";
            }
            else if (OperatingSystem.IsMacOS())
            {
                platform = "mac";
            }

            if (HasUpdate(platform))
            {
                Console.WriteLine("The update is being downloaded...");
                Download(platform);
                Unzip();
                Console.WriteLine("Finished updating.");
            }
            else
            {
                Console.WriteLine("Game is already at latest version.");
            }

            Console.ReadKey();
        }

        static bool HasUpdate(string platform)
        {
            var url = String.Format(GitChecksumURL, GitOwner, GitRepository, platform);
            var web = new System.Net.WebClient();

            if (File.Exists(GameVersion))
            {
                byte[] localVersion = File.ReadAllBytes(GameVersion);

                web.DownloadFile(url, GameVersion);

                if (File.ReadAllBytes(GameVersion).SequenceEqual(localVersion))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                web.DownloadFile(url, GameVersion);

                return true;
            }
        }

        static void Download(string platform)
        {
            var url = String.Format(GitDownloadURL, GitOwner, GitRepository, platform);
            var web = new System.Net.WebClient();
            web.DownloadFile(url, GameZip);
        }

        static void Unzip()
        {
            ZipFile.ExtractToDirectory(GameZip, ".", true);
            File.Delete(GameZip);
        }
    }
}

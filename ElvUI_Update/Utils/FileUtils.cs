using System;
using System.Diagnostics;
using System.IO;

namespace ElvUI_Update.Utils
{
    public sealed class FileUtils
    {
        public static string DataFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ElvUI_Update");
        public static string RepoFolder => Path.Combine(DataFolder, "Source");
        public static string AddonsFolder(string wowPath) => Path.Combine(wowPath, "Interface", "AddOns");

        public static bool IsValidWowDirectory(string path)
        {
            string wowExe = Path.Combine(path, "Wow.exe");
            Debug.WriteLine($"Checking for existence of {wowExe}");
            return File.Exists(wowExe);
        }

        public static bool CheckAppDataFolder()
        {
            if (!Directory.Exists(RepoFolder))
            {
                Debug.WriteLine($"Repo folder {RepoFolder} does not exist, creating...");
                DirectoryInfo ok = Directory.CreateDirectory(RepoFolder);

                // did we create?
                return ok.Exists;
            }

            Debug.WriteLine($"Found existing repo folder at {RepoFolder}");

            // just to double check
            return Directory.Exists(RepoFolder);
        }

        public static bool CheckAddonsFolder(string wowPath)
        {
            return IsValidWowDirectory(wowPath) && Directory.Exists(AddonsFolder(wowPath));
        }

        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            // create directory tree
            foreach(DirectoryInfo dir in source.GetDirectories())
            {
                // create the subdir and copy to it
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            }

            // populate tree with files
            foreach(FileInfo file in source.GetFiles())
            {
                string targetFileName = Path.Combine(target.FullName, file.Name);
                Debug.WriteLine($"comparing {targetFileName} with {file.FullName}");
                FileInfo destFile = new FileInfo(targetFileName);

                // skip if exists and is newer or equal to source
                if (destFile.Exists && file.LastWriteTime <= destFile.LastWriteTime)
                {
                    continue;
                }

                // copy file to target directory
                Debug.WriteLine($"{targetFileName} needs updating");
                file.CopyTo(destFile.FullName, true);
            }
        }
    }
}

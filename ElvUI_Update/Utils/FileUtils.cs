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

        public static bool CheckElvUIInstallation(string wowPath)
        {
            DirectoryInfo elvuiSourceDir = new DirectoryInfo(Path.Combine(RepoFolder, "ElvUI"));
            DirectoryInfo elvuiConfigSourceDir = new DirectoryInfo(Path.Combine(RepoFolder, "ElvUI_Config"));

            DirectoryInfo elvuiTargetDir = new DirectoryInfo(Path.Combine(AddonsFolder(wowPath), "ElvUI"));
            DirectoryInfo elvuiConfigTargetDir = new DirectoryInfo(Path.Combine(AddonsFolder(wowPath), "ElvUI_Config"));

            return
                CompareDirectoriesRecursively(elvuiSourceDir, elvuiTargetDir) &&
                CompareDirectoriesRecursively(elvuiConfigSourceDir, elvuiConfigTargetDir);
        }

        public static bool CompareDirectoriesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            // both must exist, obviously
            if (!source.Exists || !target.Exists)
                return false;

            foreach (DirectoryInfo sourceSubdir in source.GetDirectories())
            {
                // compare subdirectories recursively, too (recursion!)
                string targetSubdirPath = Path.Combine(target.FullName, sourceSubdir.Name);
                return CompareDirectoriesRecursively(sourceSubdir, new DirectoryInfo(targetSubdirPath));
            }

            foreach (FileInfo sourceFile in source.GetFiles())
            {
                // compare files
                string targetFilePath = Path.Combine(target.FullName, sourceFile.Name);

                if (!CompareFiles(sourceFile, new FileInfo(targetFilePath)))
                {
                    // files are not the same, or one does not exist
                    return false;
                }
            }

            // no files failed comparison, so must be OK.
            return true;
        }

        public static bool CompareFiles(FileInfo f1, FileInfo f2)
        {
            return
                f1.Exists &&
                f2.Exists &&
                f1.Name == f2.Name &&
                f1.CreationTimeUtc == f2.CreationTimeUtc;
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
                FileInfo destFile = new FileInfo(targetFileName);

                // skip if exists and is newer or equal to source
                if (destFile.Exists && file.LastWriteTime <= destFile.LastWriteTime)
                {
                    continue;
                }

                // copy file to target directory
                file.CopyTo(destFile.FullName, true);
            }
        }
    }
}

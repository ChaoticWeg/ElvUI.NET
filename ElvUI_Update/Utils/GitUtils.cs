using ElvUI_Update.Properties;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using System;
using System.Diagnostics;
using System.IO;

namespace ElvUI_Update.Utils
{
    public enum GitStatus
    {
        NoUpdates,
        Updated,
        Failed
    }

    public sealed class GitUtils
    {
        private static string GitURI => "https://git.tukui.org/elvui/elvui.git";

        public static GitStatus CloneRepo()
        {
            // TODO git clone  $dataDir
            Debug.WriteLine($"Cloning from git: {GitURI} (default branch)");
            try
            {
                Repository.Clone(GitURI, FileUtils.RepoFolder);
                // if we needed to clone, it didn't exist in the first place, so we've pulled updates
                return GitStatus.Updated;
            }

            catch (NameConflictException)
            {
                if (!Directory.Exists(Path.Combine(FileUtils.RepoFolder, ".git")))
                {
                    Debug.WriteLine($"NameConflictException, but the directory is not a repo!");
                    return GitStatus.Failed;
                }

                // exists and is repo; attempt to pull instead
                return PullRepo();
            }
        }

        public static GitStatus PullRepo()
        {
            using (var repo = new Repository(FileUtils.RepoFolder))
            {
                PullOptions options = new PullOptions
                {
                    FetchOptions = new FetchOptions
                    {
                        CredentialsProvider = new CredentialsHandler((_, __, ___)
                            => new DefaultCredentials())
                    }
                };

                // FIXME not particularly sensitive, but would be nice to extract creds
                // if you're viewing this comment in the commit history, hi!
                // sneaky, but no sensitive creds here.

                MergeResult result = Commands.Pull(repo,
                    new Signature(Resources.GitName, Resources.GitEmail, new DateTimeOffset(DateTime.Now)), options);

                if (result.Status == MergeStatus.FastForward)
                    return GitStatus.Updated;

                return GitStatus.NoUpdates;
            }
        }

        public static void Install(string wowPath)
        {
            // set source paths
            string elvuiSource = Path.Combine(FileUtils.RepoFolder, "ElvUI");
            string elvuiConfigSource = Path.Combine(FileUtils.RepoFolder, "ElvUI_Config");

            // set destination paths
            string addonsDir = FileUtils.AddonsFolder(wowPath);
            string elvuiDir = Path.Combine(addonsDir, "ElvUI");
            string elvuiConfigDir = Path.Combine(addonsDir, "ElvUI_Config");

            // copy $dataDir/ElvUI  -->  $addonsDir/ElvUI
            Debug.WriteLine($"Copying {elvuiSource} --> {elvuiDir}");

            DirectoryInfo sourceDirectory = Directory.CreateDirectory(elvuiSource);
            DirectoryInfo targetDirectory = Directory.CreateDirectory(elvuiDir);
            FileUtils.CopyFilesRecursively(sourceDirectory, targetDirectory);


            // copy $dataDir/ElvUI_Config  -->  $addonsDir/ElvUI_Config
            Debug.WriteLine($"Copying {elvuiConfigSource} --> {elvuiConfigDir}");

            sourceDirectory = Directory.CreateDirectory(elvuiConfigSource);
            targetDirectory = Directory.CreateDirectory(elvuiConfigDir);
            FileUtils.CopyFilesRecursively(sourceDirectory, targetDirectory);
        }
    }
}

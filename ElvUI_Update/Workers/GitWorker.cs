using ElvUI_Update.Utils;
using ElvUI_Update.Utils.Config;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ElvUI_Update.Workers
{
    public class GitWorker : Worker<GitStatus>
    {
        public GitWorker(MainForm form) : base(form) { }

        public async override Task<GitStatus> Run()
        {
            return await Task.Run<GitStatus>(() =>
            {
                // save config
                _form.UpdateStatus(Status.SavingConfig);
                ConfigUtils.Save(_form.Config);

                string wowPath = _form.Config.WowPath;

                // check wow directory
                _form.UpdateStatus(Status.CheckingWow);
                if (!FileUtils.IsValidWowDirectory(wowPath))
                {
                    _form.UpdateStatus(Status.InvalidWow);
                    return GitStatus.Failed;
                }

                // check appdata folder
                _form.UpdateStatus(Status.CheckingDataFolder);
                if (!FileUtils.CheckAppDataFolder())
                {
                    _form.UpdateStatus(Status.InvalidDataFolder);
                    return GitStatus.Failed;
                }

                // clone (or pull) into appdata folder
                _form.UpdateStatus(Status.Pulling);
                GitStatus cloneResult = GitUtils.CloneRepo();

                // bail out if we failed
                if (cloneResult == GitStatus.Failed)
                {
                    _form.UpdateStatus(Status.GitPullFailed);
                    return GitStatus.Failed;
                }

                // check existing elvui installation
                _form.UpdateStatus(Status.CheckingElvUI);
                if (!FileUtils.CheckElvUIInstallation(wowPath))
                {
                    Debug.WriteLine($"Need to reinstall!");

                    // need to copy new files!
                    _form.UpdateStatus(Status.Copying);
                    GitUtils.Install(wowPath);
                }

                return cloneResult;
            });
        }
    }
}

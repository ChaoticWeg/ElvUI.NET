using ElvUINET.Utils;
using ElvUINET.Utils.Config;
using ElvUINET.Utils.Status;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ElvUINET.Workers
{
    public class GitWorker : Worker<GitStatus>
    {
        public GitWorker(MainForm form) : base(form) { }

        public async override Task<GitStatus> Run()
        {
            return await Task.Run<GitStatus>(() =>
            {
                // save config
                _form.UpdateStatus(AppStatus.SavingConfig);
                ConfigUtils.Save(_form.Config);

                string wowPath = _form.Config.WowPath;

                // check wow directory
                _form.UpdateStatus(AppStatus.CheckingWow);
                if (!FileUtils.IsValidWowDirectory(wowPath))
                {
                    _form.UpdateStatus(AppStatus.InvalidWow);
                    return GitStatus.Failed;
                }

                // check appdata folder
                _form.UpdateStatus(AppStatus.CheckingDataFolder);
                if (!FileUtils.CheckAppDataFolder())
                {
                    _form.UpdateStatus(AppStatus.InvalidDataFolder);
                    return GitStatus.Failed;
                }

                // clone (or pull) into appdata folder
                _form.UpdateStatus(AppStatus.Pulling);
                GitStatus cloneResult = GitUtils.CloneRepo();

                // bail out if we failed
                if (cloneResult == GitStatus.Failed)
                {
                    _form.UpdateStatus(AppStatus.GitPullFailed);
                    return GitStatus.Failed;
                }

                // check existing elvui installation
                _form.UpdateStatus(AppStatus.CheckingElvUI);
                if (!FileUtils.CheckElvUIInstallation(wowPath))
                {
                    Debug.WriteLine($"Need to reinstall!");

                    // need to copy new files!
                    _form.UpdateStatus(AppStatus.Copying);
                    GitUtils.Install(wowPath);
                }

                return cloneResult;
            });
        }
    }
}

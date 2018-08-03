using ElvUI_Update.Utils;
using ElvUI_Update.Utils.Config;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElvUI_Update
{
    public partial class MainForm : Form
    {
        public Configuration Config { get; private set; }

        public MainForm()
        {
            InitializeComponent();

            SetUpdateButtonEnabled(false);

            Load += async (_, __) => { await OnFormLoad(); };
            btnSelectWoW.Click += (_, __) => { ChooseWowFolder(); };
            btnGo.Click += async (_, __) => { await StartUpdate(); };
        }

        private async Task OnFormLoad()
        {
            SetUpdateButtonEnabled(false);
            UpdateStatus(Status.Initializing);

            await Task.Run(() =>
            {
                UpdateStatus(Status.CheckingDataFolder);
                FileUtils.CheckAppDataFolder();

                UpdateStatus(Status.LoadingConfig);
                Config = ConfigUtils.Load();
                UpdateWowFolder(Config.WowPath);

                UpdateStatus(Status.Ready);
                SetUpdateButtonEnabled(true);
            });
        }

        // UI updaters
        private delegate void UpdateStatusDelegate(Status status);
        private delegate void UpdateWowFolderDelegate(string path);
        private delegate void SetUpdateButtonEnabledDelegate(bool enabled);

        public void UpdateStatus(Status status)
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateStatusDelegate(UpdateStatus), status);
                return;
            }

            labelStatus.Text = status.Label;
        }

        public void UpdateWowFolder(string path)
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateWowFolderDelegate(UpdateWowFolder), path);
                return;
            }

            txtWowPath.Enabled = false;

            Config.WowPath = path;
            txtWowPath.Text = Config.WowPath;
            
            if (txtWowPath.Text.Length > 0)
            {
                // move caret to end
                txtWowPath.SelectionStart = txtWowPath.Text.Length;
                txtWowPath.SelectionLength = 0;
            }

            txtWowPath.Enabled = true;
            txtWowPath.Focus();
        }

        public void SetUpdateButtonEnabled(bool enabled)
        {
            if (InvokeRequired)
            {
                Invoke(new SetUpdateButtonEnabledDelegate(SetUpdateButtonEnabled), enabled);
                return;
            }

            btnGo.Enabled = enabled;
        }

        private void ChooseWowFolder()
        {
            UpdateStatus(Status.SelectingWow);

            string newPath = ChooseFolder();

            if (newPath != null)
            {
                if (!FileUtils.IsValidWowDirectory(newPath))
                {
                    UpdateStatus(Status.InvalidWow);
                    return;
                }

                UpdateWowFolder(newPath);
            };

            UpdateStatus(Status.Ready);
        }

        private string ChooseFolder()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.SelectedPath;
            }

            return null;
        }

        private async Task StartUpdate()
        {
            Config.WowPath = txtWowPath.Text;
            SetUpdateButtonEnabled(false);

            await Task.Run(() =>
            {
                // save config
                UpdateStatus(Status.SavingConfig);
                ConfigUtils.Save(Config);

                // check wow directory
                UpdateStatus(Status.CheckingWow);
                if (!FileUtils.IsValidWowDirectory(Config.WowPath))
                {
                    UpdateStatus(Status.InvalidWow);
                    SetUpdateButtonEnabled(true);
                    return;
                }

                // check appdata folder
                UpdateStatus(Status.CheckingDataFolder);
                if (!FileUtils.CheckAppDataFolder())
                {
                    UpdateStatus(Status.InvalidDataFolder);
                    SetUpdateButtonEnabled(true);
                    return;
                }

                // clone (or pull) into appdata folder
                UpdateStatus(Status.Pulling);
                GitStatus cloneResult = GitUtils.CloneRepo();

                // bail out if we failed
                if (cloneResult == GitStatus.Failed)
                {
                    UpdateStatus(Status.GitPullFailed);
                    return;
                }

                // check existing elvui installation
                UpdateStatus(Status.CheckingElvUI);
                if (!FileUtils.CheckElvUIInstallation(Config.WowPath))
                {
                    Debug.WriteLine($"Need to reinstall!");

                    // need to copy new files!
                    UpdateStatus(Status.Copying);
                    GitUtils.Install(Config.WowPath);
                }

                // copy ("install") from appdata to wow addons folder

                switch (cloneResult)
                {
                    case GitStatus.NoUpdates:
                        UpdateStatus(Status.NoUpdates);
                        break;
                    default:
                        UpdateStatus(Status.Done);
                        break;
                }

                // guess we gotta assume we're done
                UpdateStatus(Status.Done);
                SetUpdateButtonEnabled(true);
            });
        }
    }
}

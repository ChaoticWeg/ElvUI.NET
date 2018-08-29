using ElvUINET.Utils;
using ElvUINET.Utils.Config;
using ElvUINET.Utils.Status;
using ElvUINET.Workers;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElvUINET
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
            btnGo.Click += async (_, __) => { await DoUpdate(); };
        }

        private async Task OnFormLoad()
        {
            SetUpdateButtonEnabled(false);
            UpdateStatus(AppStatus.Initializing);

            await Task.Run(() =>
            {
                UpdateStatus(AppStatus.CheckingDataFolder);
                FileUtils.CheckAppDataFolder();

                UpdateStatus(AppStatus.LoadingConfig);
                Config = ConfigUtils.Load();
                UpdateWowFolder(Config.WowPath);

                UpdateStatus(AppStatus.Ready);
                SetUpdateButtonEnabled(true);
            });
        }

        // UI updaters
        private delegate void UpdateStatusDelegate(AppStatus status);
        private delegate void UpdateWowFolderDelegate(string path);
        private delegate void SetUpdateButtonEnabledDelegate(bool enabled);

        public void UpdateStatus(AppStatus status)
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

        private void SetUpdateButtonEnabled(bool enabled)
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
            UpdateStatus(AppStatus.SelectingWow);

            string newPath = ChooseFolder();

            if (newPath != null)
            {
                if (!FileUtils.IsValidWowDirectory(newPath))
                {
                    UpdateStatus(AppStatus.InvalidWow);
                    return;
                }

                UpdateWowFolder(newPath);
            };

            UpdateStatus(AppStatus.Ready);
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

        private async Task DoUpdate()
        {
            Config.WowPath = txtWowPath.Text;
            SetUpdateButtonEnabled(false);

            GitWorker worker = new GitWorker(this);
            GitStatus result = await worker.Run();

            switch (result)
            {
                case GitStatus.Updated:
                    UpdateStatus(AppStatus.Done);
                    break;
                case GitStatus.NoUpdates:
                    UpdateStatus(AppStatus.NoUpdates);
                    break;
                default:
                    break;
            }

            SetUpdateButtonEnabled(true);
        }
    }
}

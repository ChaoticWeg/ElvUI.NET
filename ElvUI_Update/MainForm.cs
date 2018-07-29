using ElvUI_Update.Utils;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElvUI_Update
{
    public partial class MainForm : Form
    {
        private string WowPath = "";

        public MainForm()
        {
            InitializeComponent();

            Load += (_, __) => { OnFormLoad(); };
            btnSelectWoW.Click += (_, __) => { ChooseWowFolder(); };
            btnGo.Click += async (_, __) => { await StartUpdate(); };
        }

        private void OnFormLoad()
        {
            FileUtils.CheckAppDataFolder();
            UpdateStatus(Status.Ready);
        }

        private delegate void UpdateStatusDelegate(Status status);
        private delegate void UpdateWowFolderDelegate(string path);

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

            WowPath = path;
            txtWowPath.Text = WowPath;
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
            WowPath = txtWowPath.Text;

            await Task.Run(() =>
            {
                // check wow directory
                UpdateStatus(Status.CheckingWow);
                if (!FileUtils.IsValidWowDirectory(WowPath))
                {
                    UpdateStatus(Status.InvalidWow);
                    return;
                }

                // check appdata folder
                UpdateStatus(Status.CheckingDataFolder);
                if (!FileUtils.CheckAppDataFolder())
                {
                    UpdateStatus(Status.InvalidDataFolder);
                    return;
                }

                // clone (or pull) into appdata folder
                UpdateStatus(Status.Pulling);
                GitStatus cloneResult = GitUtils.CloneRepo();

                switch (cloneResult)
                {
                    case GitStatus.NoUpdates:
                        UpdateStatus(Status.NoUpdates);
                        return;
                    case GitStatus.Failed:
                        UpdateStatus(Status.GitPullFailed);
                        return;
                    default:
                        // need to continue, to install existing updates
                        break;
                }

                // copy ("install") from appdata to wow addons folder
                UpdateStatus(Status.Copying);
                GitUtils.Install(WowPath);

                // guess we gotta assume we're done
                UpdateStatus(Status.Done);
            });
        }
    }
}

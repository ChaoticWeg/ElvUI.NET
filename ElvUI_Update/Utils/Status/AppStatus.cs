namespace ElvUINET.Utils
{
    public sealed class AppStatus
    {
        public string Label { get; private set; }

        private AppStatus(string label)
        {
            Label = label;
        }

        // informational

        public static AppStatus Initializing = new AppStatus("Initializing...");
        public static AppStatus Ready = new AppStatus("Ready.");

        // statuses

        public static AppStatus SelectingWow = new AppStatus("Show me where WoW is installed!");

        public static AppStatus CheckingWow = new AppStatus("Checking WoW directory...");
        public static AppStatus CheckingElvUI = new AppStatus("Checking ElvUI directory...");
        public static AppStatus CheckingDataFolder = new AppStatus("Checking data folder...");

        public static AppStatus LoadingConfig = new AppStatus("Loading configuration...");
        public static AppStatus SavingConfig = new AppStatus("Saving configuration...");

        public static AppStatus Pulling = new AppStatus("Pulling ElvUI updates from Git...");
        public static AppStatus Copying = new AppStatus("Installing ElvUI updates...");

        // result

        public static AppStatus Done = new AppStatus("Updated ElvUI!");
        public static AppStatus NoUpdates = new AppStatus("No ElvUI updates available.");

        // errors

        public static AppStatus InvalidWow = new AppStatus("Invalid WoW directory! Double check.");
        public static AppStatus InvalidDataFolder = new AppStatus("Unable to create data folder. Report this bug to ChaoticWeg.");
        public static AppStatus GitPullFailed = new AppStatus("Unable to download updates. Report this bug to ChaoticWeg.");
    }
}

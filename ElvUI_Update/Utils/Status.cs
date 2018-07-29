namespace ElvUI_Update.Utils
{
    public sealed class Status
    {
        public string Label { get; private set; }
        private Status(string label)
        {
            Label = label;
        }

        // informational
        public static Status Initializing = new Status("Initializing...");
        public static Status Ready = new Status("Ready.");

        // statuses
        public static Status SelectingWow = new Status("Show me where WoW is installed!");
        public static Status CheckingWow = new Status("Checking WoW directory...");
        public static Status CheckingElvUI = new Status("Checking ElvUI directory...");
        public static Status CheckingDataFolder = new Status("Checking data folder...");
        public static Status Pulling = new Status("Pulling ElvUI updates from Git...");
        public static Status Copying = new Status("Installing ElvUI updates...");

        // result
        public static Status Done = new Status("Updated ElvUI!");
        public static Status NoUpdates = new Status("No ElvUI updates available."); // TODO implement

        // errors
        public static Status InvalidWow = new Status("Invalid WoW directory! Double check.");
        public static Status InvalidDataFolder = new Status("Unable to create data folder. Report this bug to ChaoticWeg.");
        public static Status GitPullFailed = new Status("Unable to download updates. Report this bug to ChaoticWeg.");
    }
}

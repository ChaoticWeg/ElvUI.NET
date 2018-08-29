using Newtonsoft.Json;
using System;
using System.IO;

namespace ElvUINET.Utils.Config
{
    public sealed class ConfigUtils
    {
        public static string Name => Path.Combine(FileUtils.DataFolder, "config.json");

        public static Configuration Default => new Configuration
        {
            WowPath = ElvUI.NET.Properties.Settings.Default["WowPath"].ToString()
        };

        public static void CheckConfig()
        {
            if (!FileUtils.CheckAppDataFolder())
            {
                // unable to create or verify existence of AppData folder
                throw new Exception("Unable to create or verify existence of configuration folder.");
            }

            // check that config file exists. save default if it doesn't already exist.
            if (!File.Exists(Name))
            {
                Save(Default);
            }
        }
        
        // load from file
        public static Configuration Load()
        {
            CheckConfig();

            using (StreamReader inFile = File.OpenText(Name))
            {
                JsonSerializer serializer = new JsonSerializer();
                Configuration config = serializer.Deserialize(inFile, typeof(Configuration)) as Configuration;
                return config ?? Default;
            }
        }

        public static void Save(Configuration config)
        {
            using (StreamWriter outStream = File.CreateText(Name))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(outStream, config);
            }
        }
    }
}

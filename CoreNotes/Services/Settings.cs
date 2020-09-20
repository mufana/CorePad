using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace CoreNotes.Services
{
    public class Settings : ISettings
    {
        public bool isAutoSave { get; set; }

        public string AutoSaveLocation { get; set; }

        public class SettingsModel
        {
            public bool isAutoSave { get; set; }

            public string AutoSaveLocation { get; set; }
        }

        public Settings()
        {
            GetCurrentSettings();
        }

        public void GetCurrentSettings()
        {
            var settings = LoadSettingsFromJson();
            isAutoSave = settings.isAutoSave;
            AutoSaveLocation = settings.AutoSaveLocation;
        }

        private SettingsModel LoadSettingsFromJson()
        {
            var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var loadPath = Path.Combine(executableLocation);
            string jsonString = File.ReadAllText($@"{loadPath}\\Resources\\Settings.json");
            var settings = JsonConvert.DeserializeObject<SettingsModel>(jsonString);

            return settings;
        }

    }
}

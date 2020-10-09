namespace CoreNotes.Models
{
    public class SettingsModel
    {
        public bool isAutoSaveToFile { get; set; }

        public string AutoSaveLocation { get; set; }

        public bool isAutoSaveToDatabase { get; set; }
    }
}

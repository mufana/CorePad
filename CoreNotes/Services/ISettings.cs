namespace CoreNotes.Services
{
    public interface ISettings
    {
        string AutoSaveLocation { get; set; }
        bool isAutoSave { get; set; }

        void GetCurrentSettings();
    }
}
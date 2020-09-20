using Caliburn.Micro;
using CoreNotes.Services;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace CoreNotes.ViewModels
{
    public class SettingsViewModel : Screen
    {
        private ISettings _settings;
        private SnackbarMessageQueue _messageQueue;
        public SettingsViewModel(ISettings settings, SnackbarMessageQueue MessageQueue)
        {
            _settings = settings;
            _messageQueue = MessageQueue;
        }

        public string AutoSaveFileLocation
        {
            get
            {
                return _settings.AutoSaveLocation;
            }
            set
            {
                _settings.AutoSaveLocation = value;
                NotifyOfPropertyChange(() => AutoSaveFileLocation);
                _messageQueue.Enqueue($"AutoSave location set to: {_settings.AutoSaveLocation}");
            }
        }

        public bool AutoSave
        {
            get
            {
                return _settings.isAutoSave;
            }
            set
            {
                _settings.isAutoSave = value;
                NotifyOfPropertyChange(() => AutoSave);
                if (AutoSave == true)
                {
                    _messageQueue.Enqueue("AutoSave is set to True");
                }
                else if (AutoSave == false)
                {
                    _messageQueue.Enqueue("AutoSave is set to False");
                }
            }
        }

        public void SaveSettingsToJson()
        {
            try
            {
                var settings = JsonConvert.SerializeObject(_settings);
                var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var savePath = Path.Combine(executableLocation);
                File.WriteAllText($@"{savePath}\\Resources\\Settings.json", settings);
                _messageQueue.Enqueue("Settings saved!");
            }
            catch (Exception ex)
            {
                _messageQueue.Enqueue($"Could not save settings. Error: {ex.Message}");
            }

        }

        public void Close()
        {
            TryClose();
        }
    }
}

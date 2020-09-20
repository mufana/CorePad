using Caliburn.Micro;
using CoreNotes.Models;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace CoreNotes.ViewModels
{
    /// <summary>
    /// SettingsViewModel that inherits from screen
    /// </summary>
    public class SettingsViewModel : Screen
    {
        /// <summary>
        /// The SnackbarMessageQueue that will receive snackbar messages
        /// </summary>
        private SnackbarMessageQueue _messageQueue;

        /// <summary>
        /// The current settings injected in the SettingsViewModel
        /// </summary>
        private SettingsModel _settings;

        /// <summary>
        /// Public field for the 'CurrentOpenFileOrDBEntry' property. Whenever the UI updates, this property will update as well
        /// </summary>
        public string AutoSaveFileLocation
        {
            get { return _settings.AutoSaveLocation; }
            set 
            {
                _settings.AutoSaveLocation = value;
                NotifyOfPropertyChange(() => AutoSaveFileLocation);
            }
        }

        /// <summary>
        /// Public field for the 'CurrentOpenFileOrDBEntry' property. Whenever the UI updates, this property will update as well
        /// </summary>
        public bool AutoSave
        {
            get { return _settings.isAutoSave; }
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

        /// <summary>
        /// Public ctor for SettingsViewModel
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="MessageQueue"></param>
        public SettingsViewModel(SettingsModel settings, SnackbarMessageQueue MessageQueue)
        {
            _settings = settings;
            _messageQueue = MessageQueue;
        }

        /// <summary>
        /// Saves the settings to the settings.json
        /// </summary>
        public void SaveSettingsToJson()
        {
            try
            {
                var settings = new SettingsModel();
                settings.AutoSaveLocation = AutoSaveFileLocation;
                settings.isAutoSave = AutoSave;
                var jsonSettings = JsonConvert.SerializeObject(settings);
                var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var savePath = Path.Combine(executableLocation);
                File.WriteAllText($@"{savePath}\\Resources\\Settings.json", jsonSettings);
                _messageQueue.Enqueue("Settings saved!");
            }
            catch (Exception ex)
            {
                _messageQueue.Enqueue($"Could not save settings. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Closes the SettingsView
        /// </summary>
        public void Close()
        {
            TryClose();
        }
    }
}

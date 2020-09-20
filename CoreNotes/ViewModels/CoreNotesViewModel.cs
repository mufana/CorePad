using Caliburn.Micro;
using CoreNotes.Models;
using CoreNotesLib.Services;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace CoreNotes.ViewModels
{
    /// <summary>
    /// Instantiates a new CoreNotesViewModel that inherits from <Conductor> and <IHandle>. 
    /// </summary>
    public class CoreNotesViewModel : Conductor<object>, IHandle<string>
    {
        /// <summary>
        /// The Settings comming from the Settings.json
        /// </summary>
        private SettingsModel _settings;

        /// <summary>
        /// Private backing field for the public 'Note' property
        /// </summary>
        private string _note;

        /// <summary>
        /// Private backing field for the public 'CurrentOpenFileOrDBEntry' property
        /// </summary>
        private string _currentOpenFileOrDBEntry;

        /// <summary>
        /// SQLite services that's injected in the <CoreNotesViewModel>
        /// </summary>
        private SqliteDataAccessService _sqliteDataAccessService;

        /// <summary>
        /// IEventAggretor that created at Bootstrapper. This will receive messages from other ViewModels
        /// </summary>
        private IEventAggregator _events;

        /// <summary>
        /// Public field for the 'Note' property. Whenever the UI updates, this property will update as well
        /// </summary>
        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                NotifyOfPropertyChange(() => Note);
            }
        }

        /// <summary>
        /// Public field for the 'CurrentOpenFileOrDBEntry' property. Whenever the UI updates, this property will update as well
        /// </summary>
        public string CurrentOpenFileOrDBEntry
        {
            get { return _currentOpenFileOrDBEntry; }
            set
            {
                _currentOpenFileOrDBEntry = value;
                NotifyOfPropertyChange(() => CurrentOpenFileOrDBEntry);
            }
        }

        /// <summary>
        /// The SnackbarMessageQueue that will receive snackbar messages
        /// </summary>
        public SnackbarMessageQueue MessageQueue { get; set; }

        /// <summary>
        /// CoreNotesViewModel ctor.
        /// </summary>
        /// <param name="events"></param>
        public CoreNotesViewModel(IEventAggregator events)
        {
            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(3600));
            _sqliteDataAccessService = new SqliteDataAccessService();
            _settings = LoadSettingsFromJson();
            _events = events;
            _events.Subscribe(this);
        }

        /// <summary>
        /// Public method to retrieve the current settings from JSON
        /// </summary>
        /// <returns>SettingsModel</returns>
        public SettingsModel LoadSettingsFromJson()
        {
            var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var loadPath = Path.Combine(executableLocation);
            string jsonString = File.ReadAllText($@"{loadPath}\\Resources\\Settings.json");
            var settings = JsonConvert.DeserializeObject<SettingsModel>(jsonString);

            return settings;
        }

        /// <summary>
        /// Activates a new SqliteDataViewModel and injects the IEventAggregator
        /// </summary>
        public void OpenSqliteNote()
        {
            ActivateItem(new SqliteDataViewModel(_events, MessageQueue));
        }

        /// <summary>
        /// Save the current note to Sqlite
        /// </summary>
        public void SaveSqliteNote()
        {
            try
            {
                _sqliteDataAccessService.SaveSqliteNote(Note);
                MessageQueue.Enqueue("Note saved to database");
            }
            catch (Exception ex)
            {
                MessageQueue.Enqueue($"Could not save file to database. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deactivates the open Views and Activates a new CoreNotesView with en empty Note string
        /// </summary>
        public void NewFile()
        {
            DeactivateItem(ActiveItem, true);
            ActivateItem(new CoreNotesViewModel(_events));
            Note = "";
        }

        /// <summary>
        /// Save the note to a file
        /// </summary>
        public void SaveFile()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text File (*.txt)|*.txt";
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, Note);
                    CurrentOpenFileOrDBEntry = saveFileDialog.FileName;
                    MessageQueue.Enqueue($"File saved to: {saveFileDialog.FileName}");
                }
            }
            catch (Exception ex)
            {
                MessageQueue.Enqueue($"Could not save file. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Open a note from file
        /// </summary>
        public void OpenFile()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                CurrentOpenFileOrDBEntry = openFileDialog.FileName;
                Note = File.ReadAllText(openFileDialog.FileName);
                DeactivateItem(ActiveItem, true);
            }
        }

        /// <summary>
        /// Closes the application and, when AutoSave is set to True, it automatically saves the note to a file and Sqlite
        /// </summary>
        public void Close()
        {
            try
            {
                var tempFileName = $@"{Guid.NewGuid()}.txt";
                if (_settings.isAutoSave)
                {
                    File.WriteAllText($@"{_settings.AutoSaveLocation}{tempFileName}", Note);
                    SaveSqliteNote();
                }
            }
            catch (Exception ex)
            {
                MessageQueue.Enqueue($"Could not save file. Error: {ex.Message}");
            }

            TryClose();
        }

        /// <summary>
        /// Activates the SettingsViewModel
        /// </summary>
        public void LoadSettingsView()
        {
            ActivateItem(new SettingsViewModel(_settings, MessageQueue));
        }

        /// <summary>
        /// Activates the AboutViewModel
        /// </summary>
        public void LoadAboutView()
        {
            ActivateItem(new AboutViewModel(MessageQueue));
        }

        /// <summary>
        /// Activates a new SqliteDataViewModel and injects the IEventAggregator
        /// </summary>
        public void ManageDatabase()
        {
            ActivateItem(new SqliteDataViewModel(_events, MessageQueue));
        }

        /// <summary>
        /// Receives messages sent to the IEventAggregator from other ViewModels
        /// </summary>
        /// <param name="message"></param>
        public void Handle(string message)
        {
            DeactivateItem(ActiveItem, true);
            ActivateItem(new CoreNotesViewModel(_events));
            Note = message;
        }
    }
}
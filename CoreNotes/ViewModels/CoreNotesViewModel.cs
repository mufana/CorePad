using Caliburn.Micro;
using CoreNotes.Services;
using CoreNotesLib.Services;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.IO;

namespace CoreNotes.ViewModels
{
    public class CoreNotesViewModel : Conductor<object>, IHandle<string>
    {
        private ISettings _settings;
        private string _note;
        private string _currentOpenFileOrDBEntry;
        private SqliteDataAccessService _sqliteDataAccessService;
        private IEventAggregator _events;
        
        public SnackbarMessageQueue MessageQueue { get; set; }

        public CoreNotesViewModel(ISettings settings, IEventAggregator events)
        {
            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(3600));
            _sqliteDataAccessService = new SqliteDataAccessService();
            _settings = settings;
            _events = events;
            _events.Subscribe(this);
        }

        public string Note
        {
            get
            {
                return _note;
            }
            set
            {
                _note = value;
                NotifyOfPropertyChange(() => Note);
            }
        }

        public string CurrentOpenFileOrDBEntry
        {
            get
            {
                return _currentOpenFileOrDBEntry;
            }
            set
            {
                _currentOpenFileOrDBEntry = value;
                NotifyOfPropertyChange(() => CurrentOpenFileOrDBEntry);
            }
        }

        public void OpenSqliteNote()
        {
            ActivateItem(new SqliteDataViewModel(_events));
        }

        public void SaveSqliteNote()
        {
            try
            {
                _sqliteDataAccessService.SaveSqliteNote(Note);
            }
            catch (Exception ex)
            {
                MessageQueue.Enqueue($"Could not save file to datbase. Error: {ex.Message}");
            }
        }

        public void NewFile()
        {
            DeactivateItem(ActiveItem, true);
            ActivateItem(new CoreNotesViewModel(_settings, _events));
            Note = "";
        }

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

        public void OpenFile()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                CurrentOpenFileOrDBEntry = openFileDialog.FileName;
                Note = File.ReadAllText(openFileDialog.FileName);
            }
        }

        public void Close()
        {
            try
            {
                var tempFileName = $@"{Guid.NewGuid()}.txt";
                if (_settings.isAutoSave)
                {
                    File.WriteAllText($@"{_settings.AutoSaveLocation}{tempFileName}", Note);
                }
            }
            catch (Exception ex)
            {
                MessageQueue.Enqueue($"Could not save file. Error: {ex.Message}");
            }

            TryClose();
        }

        public void LoadSettingsView()
        {
            ActivateItem(new SettingsViewModel(_settings, MessageQueue));
        }

        public void LoadAboutView()
        {
            ActivateItem(new AboutViewModel(MessageQueue));
        }

        public void Handle(string message)
        {
            DeactivateItem(ActiveItem, true);
            ActivateItem(new CoreNotesViewModel(_settings, _events));
            Note = message;
        }
    }
}
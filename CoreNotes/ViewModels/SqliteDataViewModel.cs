using Caliburn.Micro;
using CoreNotesLib.Models;
using CoreNotesLib.Services;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;

namespace CoreNotes.ViewModels
{
    /// <summary>
    /// Instantiates a new SqliteDataViewModel that inherits from screen
    /// </summary>
    public class SqliteDataViewModel : Screen
    {
        /// <summary>
        /// Private backing for the SqliteDataAccessService
        /// </summary>
        private SqliteDataAccessService _sqliteDataAccessService;

        /// <summary>
        /// Private backing for the _selectedNote
        /// </summary>
        private CoreNotesSqliteModel _selectNote;

        /// <summary>
        /// The SnackbarMessageQueue that will receive snackbar messages
        /// </summary>
        private SnackbarMessageQueue _messageQueue;

        /// <summary>
        /// Private backing for the IEventAggregator
        /// </summary>
        private IEventAggregator _events;

        /// <summary>
        /// Public field for the 'CurrentOpenFileOrDBEntry' property. Whenever the UI updates, this property will update as wwell
        /// </summary>
        public ObservableCollection<CoreNotesSqliteModel> Notes
        {
            get { return GetAllNotes(); }
            set
            {
                Notes = value;
                NotifyOfPropertyChange(() => Notes);
            }
        }

        /// <summary>
        /// Public field for the 'CurrentOpenFileOrDBEntry' property. Whenever the UI updates, this property will update as wwell
        /// </summary>
        public CoreNotesSqliteModel SelectNote
        {
            get { return _selectNote; }
            set
            {
                _selectNote = value;
                NotifyOfPropertyChange(() => SelectNote);
            }
        }

        /// <summary>
        /// Public SqliteDataViewModel ctor
        /// </summary>
        /// <param name="events"></param>
        public SqliteDataViewModel(IEventAggregator events, SnackbarMessageQueue messageQueue)
        {
            _sqliteDataAccessService = new SqliteDataAccessService();
            _messageQueue = messageQueue;
            _events = events;
        }

        /// <summary>
        /// Retrieves all notes from the database
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<CoreNotesSqliteModel> GetAllNotes()
        {
          return _sqliteDataAccessService.GetSqliteNotes();
        }

        /// <summary>
        /// Edits a note
        /// </summary>
        public void EditNote()
        {
            if (_selectNote?.NoteText == null)
            {
                _messageQueue.Enqueue("No note selected!");
            }
            else
            {
                var noteText = _selectNote?.NoteText;
                _events.PublishOnUIThread(noteText);
                _messageQueue.Enqueue($"Editing note with id {_selectNote.NoteId}");
            }
        }

        /// <summary>
        /// Deletes a note from the database
        /// </summary>
        public void DeleteNote()
        {
            if (_selectNote?.NoteText == null)
            {
                _messageQueue.Enqueue("No note selected!");
            }
            else
            {
                var noteText = _selectNote.NoteText;
                _sqliteDataAccessService.DeleteSqliteNote(noteText);
                _messageQueue.Enqueue($"Deleted note with id {_selectNote.NoteId}");
            }
        }

        /// <summary>
        /// Closes the SqliteDataView
        /// </summary>
        public void Close()
        {
            TryClose();
        }
    }
}

using Caliburn.Micro;
using CoreNotesLib.Models;
using CoreNotesLib.Services;
using System.Collections.Generic;

namespace CoreNotes.ViewModels
{
    public class SqliteDataViewModel : Screen
    {
        private IEventAggregator _events;
        private SqliteDataAccessService _sqliteDataAccessService;
        private CoreNotesSqliteModel _selectNote;

        public SqliteDataViewModel(IEventAggregator events)
        {
            _sqliteDataAccessService = new SqliteDataAccessService();
            _events = events;
        }

        public List<CoreNotesSqliteModel> Notes
        {
            get
            {
                return GetAllNotes();
            }
            set
            {
                Notes = value;
                NotifyOfPropertyChange(() => Notes);
            }
        }

        public CoreNotesSqliteModel SelectNote
        {
            get
            {
                return _selectNote;
            }
            set
            {
                _selectNote = value;
                NotifyOfPropertyChange(() => SelectNote);
            }
        }

        public List<CoreNotesSqliteModel> GetAllNotes()
        {
          return _sqliteDataAccessService.GetSqliteNotes();
        }

        public void EditNote()
        {
            var noteText = _selectNote.NoteText;
            _events.PublishOnUIThread(noteText);
        }

        public void Close()
        {
            TryClose();
        }
    }
}

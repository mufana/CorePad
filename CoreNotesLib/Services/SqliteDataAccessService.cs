using CoreNotesLib.Models;
using Dapper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace CoreNotesLib.Services
{
    public class SqliteDataAccessService
    {
       public ObservableCollection<CoreNotesSqliteModel> GetSqliteNotes()
        {
            using (IDbConnection connection = new SQLiteConnection(LoadConnectionString()))
            {
                var returnData = connection.Query<CoreNotesSqliteModel>("select * from Notes", new DynamicParameters()).OrderByDescending(x => x.NoteId).ToList();
                ObservableCollection<CoreNotesSqliteModel> observableNotes = new ObservableCollection<CoreNotesSqliteModel>(returnData);

                return observableNotes;
            }
        }

        public void SaveSqliteNote(string noteText)
        {
            CoreNotesSqliteModel notesSqliteModel = new CoreNotesSqliteModel();
            notesSqliteModel.NoteText = noteText;

            using (IDbConnection connection = new SQLiteConnection(LoadConnectionString()))
            {
                connection.Execute("insert into Notes (NoteText) values (@NoteText)", notesSqliteModel);
            }
        }

        public void DeleteSqliteNote(string noteText)
        {
            CoreNotesSqliteModel notesSqliteModel = new CoreNotesSqliteModel();
            notesSqliteModel.NoteText = noteText;

            using (IDbConnection connection = new SQLiteConnection(LoadConnectionString()))
            {
                connection.Execute("delete from Notes where NoteText = @NoteText", notesSqliteModel);
            }
        }

        private string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}

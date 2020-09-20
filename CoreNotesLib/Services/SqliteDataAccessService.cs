using CoreNotesLib.Models;
using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace CoreNotesLib.Services
{
    public class SqliteDataAccessService
    {
       public List<CoreNotesSqliteModel> GetSqliteNotes()
        {
            var returnData = new List<CoreNotesSqliteModel>();

            using (IDbConnection connection = new SQLiteConnection(LoadConnectionString()))
            {
                returnData = connection.Query<CoreNotesSqliteModel>("select * from Notes", new DynamicParameters()).ToList();
                return returnData;
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

        private string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}

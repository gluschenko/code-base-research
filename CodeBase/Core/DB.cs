using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*DB database = new DB("test.sqlite", (connection) => {}, (ex) => {
               MessageBox.Show(ex.ToString(), "SQLite Connection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
           });

           database.Query("CREATE TABLE IF NOT EXISTS Catalog (id INTEGER PRIMARY KEY AUTOINCREMENT, author TEXT, book TEXT)");
           database.Query("INSERT INTO Catalog (author, book) VALUES ('gfdb', 'fhfdhfd'), ('gfdb', 'fhfdhfd')");

           var result = database.Query("SELECT * FROM Catalog WHERE 1");
           MessageBox.Show(result.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);*/

/*using System.Data.SQLite;
using System.IO;
using System.Data;

namespace CodeBase
{
    public class DB
    {
        public string FileName { get; private set; }

        private SQLiteConnection connection;
        private SQLiteCommand command;

        private Action<SQLiteConnection> onConnected;
        private Action<SQLiteException> onConnectionFail;

        public DB(string FileName, Action<SQLiteConnection> onConnected, Action<SQLiteException> onConnectionFail)
        {
            this.FileName = FileName;

            connection = new SQLiteConnection();
            command = new SQLiteCommand();

            this.onConnected = onConnected;
            this.onConnectionFail = onConnectionFail;

            Init();
        }

        private void Init()
        {
            if (!File.Exists(FileName))
                SQLiteConnection.CreateFile(FileName);

            try
            {
                connection = new SQLiteConnection("Data Source=" + FileName + ";Version=3;");
                connection.Open();

                command.Connection = connection;

                onConnected?.Invoke(connection);
            }
            catch (SQLiteException ex)
            {
                onConnectionFail?.Invoke(ex);
            }
        }

        public SQLiteDataReader Query(string cmd)
        {
            try
            {
                command.CommandText = cmd;
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    return reader;
                }
            }
            catch (SQLiteException ex)
            {
                onConnectionFail?.Invoke(ex);
            }

            return null;
        }
    }
}*/

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DAO;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{

    internal abstract class AbstractController<TDAO, TKey> where TDAO : absDAO<TKey>
    {
        internal readonly string _connectionString;

        internal readonly ILog dblog;

        public ILoggerRepository LogRepository { get; }

        internal AbstractController()
        {

            LogRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(LogRepository, new System.IO.FileInfo("log4net.config"));

            dblog = LogManager.GetLogger("DB_LOGS");

            // Get the current directory where the application is running
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;

            // Define the relative path to the database file
            string relativePath = "kanban.db";

            // Combine the current directory with the relative path
            string path = Path.GetFullPath(Path.Combine(currentDir, relativePath));
            dblog.Info($"trying to connect to db path : {path}");
            this._connectionString = $"Data Source={path}; Version=3;";

        }


        public bool Update<T>(string _tableName, TKey id, string attributeName, string primarycolname, T attributeValue)
        {
            int rowsAffected = 0;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                // Use parameterized query to avoid SQL injection
                string query = $"UPDATE {_tableName} SET [{attributeName}] = @Value WHERE {primarycolname} = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    // Add parameters
                    command.Parameters.AddWithValue("@Value", attributeValue);
                    command.Parameters.AddWithValue("@Id", id);

                    try
                    {
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Handle exception (log, throw, etc.)
                        dblog.Error($"An error occurred: {ex.Message}");
                        throw new Exception("problem acessing data in Update user function");

                    }
                }
            }

            // Return true if at least one row was affected
            return rowsAffected > 0;
        }


        internal bool Add(string commando, Action<SQLiteCommand> bindParameters)
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    using (var command = new SQLiteCommand(commando, connection))
                    {
                        bindParameters(command); // Call the delegate to bind parameters

                        connection.Open();
                        res = command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    dblog.Error($"An error occurred: {ex.Message}");
                    throw new Exception("Problem accessing data in DB function", ex);
                }
            }

            return res > 0;
        }


        internal List<TDAO> GetAll(string _tablename,Func<SQLiteDataReader, TDAO> convertReaderToObject,string filter = "")
        {
            List<TDAO> results = new List<TDAO>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {_tablename} "+filter+"";
                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        results.Add(convertReaderToObject(dataReader));
                    }
                }
                catch (Exception ex)
                {
                    dblog.Error($"An error occurred: {ex.Message}");
                    throw new Exception("Problem accessing data in GetAll function", ex);
                }
                finally
                {
                    dataReader?.Close();
                }
            }

            return results;
        }

        internal TDAO GetByID(TKey id, string query, Action<SQLiteCommand> bindParameters, Func<SQLiteDataReader, TDAO> convertReaderToObject)
        {
            TDAO result = null;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                using (var command = new SQLiteCommand(query, connection))
                {
                    bindParameters(command);

                    try
                    {
                        connection.Open();

                        using (var dataReader = command.ExecuteReader())
                        {
                            if (dataReader.Read())
                            {
                                result = convertReaderToObject(dataReader);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        dblog.Error($"An error occurred: {ex.Message}");
                        throw new Exception("Problem accessing data in GetByID function", ex);
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// delete board from DB
        /// </summary>
        /// <param name="bdao"></param>
        internal bool Delete(string commando, Action<SQLiteCommand> bindParameters)
        {
            try
            {
                using (var connection = new SQLiteConnection(this._connectionString))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(commando, connection))
                    {
                        bindParameters(command);
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                dblog.Error("Problem accessing data in DB function", ex);
                throw new Exception("Problem accessing data in DB function", ex);
            }
        }

        public void DeleteAllData()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Disable foreign key checks
                        using (var command = new SQLiteCommand("PRAGMA foreign_keys = OFF;", connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        // List of tables to delete data from, in the correct order
                        var tables = new string[] { "UserBoardStatus", "BoardTask", "Task", "Board", "User" };

                        foreach (var table in tables)
                        {
                            var deleteQuery = $"DELETE FROM {table}";
                            using (var command = new SQLiteCommand(deleteQuery, connection, transaction))
                            {
                                command.ExecuteNonQuery();
                            }
                        }

                        // Enable foreign key checks
                        using (var command = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        dblog.Error($"An error occurred: {ex.Message}");
                        throw new Exception("Problem accessing data in DeleteAllData function", ex);
                    }
                }

                // Optionally reset auto-increment counters
                try
                {
                    using (var command = new SQLiteCommand("VACUUM;", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    dblog.Error($"An error occurred during VACUUM: {ex.Message}");
                    throw new Exception("Problem accessing data in DeleteAllData function during VACUUM", ex);
                }
            }
        }

    }

}




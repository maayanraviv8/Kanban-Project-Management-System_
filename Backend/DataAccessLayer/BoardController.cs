using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DAO;
using log4net;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{

    internal class BoardController : AbstractController<BoardDAO, int>
    {

        internal BoardController() : base()
        {
           
        }


        internal BoardDAO ConvertReaderToObject(SQLiteDataReader reader)
        {
            return new BoardDAO(Convert.ToInt32(reader.GetValue(0)), reader.GetString(1), reader.GetString(2), Convert.ToInt32(reader.GetValue(3)), Convert.ToInt32(reader.GetValue(4)), Convert.ToInt32(reader.GetValue(5)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns all boards in DB </returns>
        internal List<BoardDAO> GetAllBoards()
        {
            return GetAll("Board", ConvertReaderToObject);

        }
        /// <summary>
        /// add board to DB
        /// </summary>
        /// <param name="bdao"></param>
        /// 
        internal bool CreateBoard(BoardDAO bdao)
        {
            // Insert into Board table
            string commando = $"INSERT INTO {bdao._tableName} ({bdao.ColumnNames[0]},{bdao.ColumnNames[1]},{bdao.ColumnNames[2]},{bdao.ColumnNames[3]},{bdao.ColumnNames[4]},{bdao.ColumnNames[5]}) values (@bid, @boardtitle, @boardowner,@limit0,@limit1,@limit2)";
            bool boardResult = Add(commando, command =>
            {
                command.Parameters.AddWithValue("@bid", bdao.BOARDID);
                command.Parameters.AddWithValue("@boardtitle", bdao.BOARDTITLE);
                command.Parameters.AddWithValue("@boardowner", bdao.BOARDOWNER);
                command.Parameters.AddWithValue("@limit0", bdao.LIMIT0);
                command.Parameters.AddWithValue("@limit1", bdao.LIMIT1);

                command.Parameters.AddWithValue("@limit2", bdao.LIMIT2);

            });

            if (!boardResult)
            {
                return false;
            }

            // Insert into UserBoardStatus table
            UserBoardStatusDAO ubs = new UserBoardStatusDAO(bdao.BOARDOWNER, bdao.BOARDID, 1);
            string commando3 = $"INSERT INTO {ubs._tableName} ({ubs.ColumnNames[0]},{ubs.ColumnNames[1]},{ubs.ColumnNames[2]}) values (@userid, @boardid, @imtheowner)";
            return Add(commando3, command =>
            {
                command.Parameters.AddWithValue("@userid", ubs.email);
                command.Parameters.AddWithValue("@boardid", ubs.BoardID);
                command.Parameters.AddWithValue("@imtheowner", ubs.relation);
            });
        }



        /// <summary>
        /// delete board from DB
        /// </summary>
        /// <param name="bdao"></param>
        internal bool DeleteBoard(BoardDAO bdao)
        {
            bool success = true;

            // Delete related tasks
            string deleteTasks = $"DELETE FROM Task WHERE boardID = @boardID";
            success &= Delete(deleteTasks, command =>
            {
                command.Parameters.AddWithValue("@boardID", bdao.BOARDID);
            });

            // Delete from BoardTask table
            string deleteBoardTask = $"DELETE FROM BoardTask WHERE boardID = @boardID";
            success &= Delete(deleteBoardTask, command =>
            {
                command.Parameters.AddWithValue("@boardID", bdao.BOARDID);
            });

   
            // Delete from UserBoardStatus table
            string deleteUserBoardStatus = $"DELETE FROM UserBoardStatus WHERE boardID = @boardID";
            success &= Delete(deleteUserBoardStatus, command =>
            {
                command.Parameters.AddWithValue("@boardID", bdao.BOARDID);
            });

            // Finally, delete the board itself
            string deleteBoard = $"DELETE FROM {bdao._tableName} WHERE {bdao.Primarycolumn} = @boardID";
            success &= Delete(deleteBoard, command =>
            {
                command.Parameters.AddWithValue("@boardID", bdao.BOARDID);
            });

            return success;
        }



        /// <summary>
        /// one functoin to update all board parameters
        /// </summary>
        /// <param name="bdao"></param>
        internal void UpdateBoard(int id, string attributeName, string attributeValue)
        {
            Update("Board", id, attributeName, "BoardID",attributeValue);
        }

        internal void UpdateBoard(int id, string attributeName, int attributeValue)
        {
            Update("Board", id, attributeName, "BoardID", attributeValue);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns>specific board byID</returns>
        internal List<BoardDAO> GetBoardsByUserID(string email)
        {
            string filter = $"WHERE owner = '{email}'";
            return GetAll("Board", ConvertReaderToObject, filter);
         }

        /// <summary>
        /// adds that user to the board in table USerBoardStatus
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardID"></param>
        /// <param name="status"></param>
        internal void Join(string email, int boardID, int status=0)
        {
            UserBoardStatusDAO ubs = new UserBoardStatusDAO(email,boardID,status);
            string commando = $"INSERT INTO {ubs._tableName} ({ubs.ColumnNames[0]},{ubs.ColumnNames[1]},{ubs.ColumnNames[2]}) values (@mail,@boardid,@status)";

            Add( commando, command =>
            {
                command.Parameters.AddWithValue("@mail", ubs.email);
                command.Parameters.AddWithValue("@boardid", ubs.BoardID);
                command.Parameters.AddWithValue("@status", ubs.relation);
                // Additional bindings specific to UserDAO if needed
            });
        }

        /// <summary>
        /// removes that user from board in UserBoardStatus
        /// </summary>
        /// <param name="email"></param>
        /// <param name="BoardID"></param>
        internal bool Leave(string email, int BoardID)
        {
            int rowsAffected = 0;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                // Use parameterized query to avoid SQL injection
                string query = $"DELETE FROM UserBoardStatus WHERE email = @Id and BoardID = @bid";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id",email);
                    command.Parameters.AddWithValue("@bid", BoardID);

                    try
                    {
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        dblog.Error($"An error occurred while deleting the object with primary key {email}: {ex.Message}");
                        throw new Exception("Problem accessing data in Delete function", ex);
                    }
                }
            }

            return rowsAffected > 0;
        }



        /// <summary>
        /// changes ststus of previous owner to guest and new user to owner
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardIS"></param>
        internal void NewOwner(string email, int boardID)
        {
            int rowsAffected = 0;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Update the current owner's status to 0
                        string updateCurrentOwnerQuery = "UPDATE UserBoardStatus SET status = 0 WHERE boardID = @BoardID AND status = 1";
                        using (var updateCurrentOwnerCommand = new SQLiteCommand(updateCurrentOwnerQuery, connection, transaction))
                        {
                            updateCurrentOwnerCommand.Parameters.AddWithValue("@BoardID", boardID);

                            rowsAffected = updateCurrentOwnerCommand.ExecuteNonQuery();
                            if (rowsAffected == 0)
                            {
                                throw new Exception("No current owner found with status 1 for the given board ID.");
                            }
                        }

                        // Update the new owner's status to 1
                        string updateNewOwnerQuery = "UPDATE UserBoardStatus SET status = 1 WHERE boardID = @BoardID AND email = @Email";
                        using (var updateNewOwnerCommand = new SQLiteCommand(updateNewOwnerQuery, connection, transaction))
                        {
                            updateNewOwnerCommand.Parameters.AddWithValue("@BoardID", boardID);
                            updateNewOwnerCommand.Parameters.AddWithValue("@Email", email);

                            rowsAffected = updateNewOwnerCommand.ExecuteNonQuery();
                            if (rowsAffected == 0)
                            {
                                throw new Exception("No user found with the given email and board ID.");
                            }
                        }

                        // Commit the transaction if both updates succeeded
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if an error occurred
                        transaction.Rollback();

                        // Log the error and rethrow the exception
                        dblog.Error($"An error occurred: {ex.Message}");
                        throw new Exception("Problem accessing data in NewOwner function", ex);
                    }
                }
            }
        }


        internal List<string> GetMembers(int boardID)
        {
            List<string> emails = new List<string>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                string query = "SELECT email FROM UserBoardStatus WHERE boardID = @BoardID";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BoardID", boardID);

                    try
                    {
                        connection.Open();
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                emails.Add(reader.GetString(reader.GetOrdinal("email")));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        dblog.Error($"An error occurred while retrieving members for board {boardID}: {ex.Message}");
                        throw new Exception("Problem accessing data in GetMembers function", ex);
                    }
                }
            }

            return emails;
        }


    }
}

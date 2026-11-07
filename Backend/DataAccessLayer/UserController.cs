using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer;
using System.Data.SQLite;
using System.IO;
using log4net;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BackendTests")]


namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    internal class UserController : AbstractController<UserDAO, string>
    {

  
        internal UserController() : base () 
        {
      

        }

        internal bool AddUser(UserDAO userDAO)
        {
            string commando = $"INSERT INTO {userDAO._tableName} ({userDAO.ColumnNames[0]}, {userDAO.ColumnNames[1]}) values (@Email, @Password)";

            return Add(commando, command =>
            {
                command.Parameters.AddWithValue("@Email", userDAO.Email);
                command.Parameters.AddWithValue("@Password", userDAO.Password);
                // Additional bindings specific to UserDAO if needed
            });
        }


        internal UserDAO ConvertReaderToObject(SQLiteDataReader reader)
        {
            return new UserDAO(reader.GetString(0), reader.GetString(1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns> returns list of all registered users</returns>
        /// 
        internal List<UserDAO> GetAllUsers()
        {
            return GetAll("User",ConvertReaderToObject);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns>userDAO of requested ID</returns>
        internal UserDAO GetUserByID(string email)
        {
            string query = $"SELECT * FROM User WHERE email = @Email;";

            return GetByID(email, query, command =>
            {
                command.Parameters.AddWithValue("@Email", email);
            }, ConvertReaderToObject);
        }




    }
}

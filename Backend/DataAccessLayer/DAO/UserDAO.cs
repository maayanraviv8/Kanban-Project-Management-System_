using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DAO;
using log4net.Repository.Hierarchy;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    internal class UserDAO : absDAO<string>
    {
        private string email { get; set; }
        private string password { get; set; }
       

        internal UserController userController = new UserController();
        internal override string PrimaryKey => Email;
        internal override string Primarycolumn => "email";

        internal override List<string> ColumnNames => new List<string> { "email", "password" };
        internal override string _tableName => "User";




        internal UserDAO(string mail, string pass)
        {
            this.email = mail;
            this.password = pass;
        }

       
        internal string Email {  get =>email;
            set
            { 
                this.email = value;
                if (Ispersisted)//should never reach here because we dont have option to update email 
                    userController.Update(this._tableName, this.email, this.ColumnNames[0],this.Primarycolumn, value);
            
            }
        }

        internal string Password
        {
            get => password;
        }


        internal void persist()
        {
            if(!Ispersisted)//only insert if hasnt been inserted
                userController.AddUser(this);
                this.Ispersisted = true;
        }

     




    }
}

    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data;
using System.IO;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal class UserBL
    {
        private String email;
        private String password;
        private UserDAO userdao; 
        internal static int MINIMALPASSWORD = 6;
        internal static int MAXIMALPASSWORD = 20;
        
        internal UserBL(string email,string password)
        {
            userdao = new UserDAO(email, password);//order here matters!
            this.Email = email;
            this.Password = password;
            userdao.persist();
        }

        internal UserBL(UserDAO ud) {
            this.Email = ud.Email;
            this.Password = ud.Password; //make sure the correct uppercase
                }
        
        internal UserDAO GetUserDAO { get => userdao; }
        internal String Email { 
            get => email;
            set
            {
                if (value == null)
                {
                    throw new Exception("There is not email");
                }
                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                //string emailPattern = @"^(?=.{1,64}@)[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}(?<=.{1,253})$";
                Regex emailRegex = new Regex(emailPattern);
                if (!emailRegex.IsMatch(value))
                {
                    throw new Exception($"Email {email} is illegal. please try again");
                }
                this.email = value;
                userdao.Email = email;// make sure this works
            }
        }
        internal String Password
        {
            get => password;
            set
            {
                if (value == null)
                {
                    throw new Exception("There is no password");
                }
                if(value.Length<MINIMALPASSWORD || value.Length > MAXIMALPASSWORD) { throw new Exception("The length of the password is invalid"); }
                string patternLow = @"^(?=.*[a-z]).*$";
                Regex passwordRegex = new Regex(patternLow);
                if (!passwordRegex.IsMatch(value)) { throw new Exception("The password is invalid"); }
                string patternUp = @"^(?=.*[A-Z]).*$";
                passwordRegex = new Regex(patternUp);
                if (!passwordRegex.IsMatch(value)) { throw new Exception("The password is invalid"); }
                string patternNumber = @"^(?=.*\d).*$";
                passwordRegex = new Regex(patternNumber);
                if (!passwordRegex.IsMatch(value)) { throw new Exception("The password is invalid"); }
                this.password = value;            }
        }

    }
}

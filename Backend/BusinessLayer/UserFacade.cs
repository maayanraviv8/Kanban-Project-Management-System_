using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    internal class UserFacade
    {
        private readonly ILog log;
        private readonly Dictionary<string, UserBL> _users = new Dictionary<string, UserBL>();
        private readonly List<string> _useresConnected = new List<string>();
        private UserController uc;

        internal UserFacade(ILog log2)
        {
            this.log = log2;
            _users = new Dictionary<string, UserBL>();
            _useresConnected = new List<string>();
            uc = new UserController();
            //LoadUsers();
        }

        internal void LoadUsers()
        {
            List<UserDAO> userDAOs = uc.GetAllUsers();
            foreach (UserDAO user in userDAOs)
            {
                _users.Add(user.Email, new UserBL(user));
            }
        }
        internal void DeleteUsersData()
        {
            uc.DeleteAllData();
        }
        internal UserBL GetUser(string email)
        {
            return _users[email];
        }
        //internal void Register_testsql(string email, string pass) להכניס לכל פונקציה הבהתאם לDAL כדי שישמור בDB את הדברים החדשים
        //{
        //    log.Info($"trying to add user {email} to DB");
        //    try
        //    {
        //        uc.AddUser(new UserDAO(email, pass));
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error($"could add to db got ${ex}");
        //    }
        // }


        //internal void LoadUsers_testsql() להכניס לבנאי שיקלוט את כל המשתמשים מהDB
        //{
        //    log.Info("trying to fetch all users");
        //    try
        //    {
        //        List<UserDAO> userDAOs = uc.GetAllUsers();
        //        foreach (UserDAO user in userDAOs)
        //        {
        //            this._users[user.Email] = new UserBL(user); //add constructor 
        //            log.Info($"{user.Email} was fetched and adding to list in userfacade");
        //        }

        //        log.Info("final list :");

        //        foreach (UserBL user2 in _users.Values)
        //        {
        //            log.Info($"{user2.Email}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error($"could not fetch users from DB got exception ${ex}");
        //    }
        //}




        internal void Register(string email, string password)
        {
            log.Info($"USERFACADE Registering new entity with EMAIL: {email} and password:{password} ");
            if (email == null || email=="")
            {
                log.Error("USERFACADE Email is null or empty");
                throw new Exception("There is no email");
            }
            if (password == null || password=="")
            {
                log.Error("USERFACADE password is null or empty");
                throw new Exception("There is no email");
            }
            if (_users.ContainsKey(email))
            {
                log.Error($"USERFACADE The email {email} already has an account.");
                throw new Exception($"The email {email} has an account");
            }
            UserBL newUser = new UserBL(email, password);
            _users.Add(email, newUser);
            _useresConnected.Add(newUser.Email);
            uc.AddUser(newUser.GetUserDAO);

            log.Info($"USERFACADE User {email} registered successfully.");

        }
        internal bool IsLoggedIn(string email)
        {
            log.Info($"USERFACADE ckecking if username with email:{email} is logged in");
            return _useresConnected.Contains(email);
        }
        internal void Logout(string email)
        {
            if (!_useresConnected.Contains(email)) {
                log.Error($"USERFACADE {email} is not connected. so nothing to logout");
                throw new Exception($"The user {email} is already logged out"); 
            }
            log.Info($"USERFACADE logging {email} out");
            _useresConnected.Remove(email);
        }
        internal void LogIn(string email, string password)
        {
            log.Info($"USERFACADE trying to login with email:{email} and password:{password}");
            if (email == null || password == null) {
                log.Error($"USERFACADE email  or password is null");
                throw new Exception("The entered details are invalid"); 
            }
            if (IsLoggedIn(email))
            {
                log.Error("USERFACADE this user is already logged in ");
                throw new Exception("This user is logged in");
            }
            if (!_users.ContainsKey(email)) {
                log.Error("USERFACADE this user isnt registered");
                throw new Exception($"User {email} is not registered");
            }
            else if (!_users[email].Password.Equals(password)) {
                log.Error("USERFACADE WRONG PASSWORD");
                throw new Exception("Wrong password. Please try again");
            }
            else {
                log.Info($"USERFACADE user {email} just logged in");
                _useresConnected.Add(email);
            }
        }

        internal bool Contains(string email)
        {
            return _users.ContainsKey(email);
        }

        internal List<UserBL> Members(List<string> members)
        {
            List<UserBL> output = new List<UserBL>();
            if (members.Count > 0) {
                foreach (string member in members)
                {
                    if (_users.ContainsKey(member))
                    {
                        output.Add(_users[member]);
                    }
                }
            }
            return output;
        }
    }
}


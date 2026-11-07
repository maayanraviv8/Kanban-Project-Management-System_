using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.ServiceLayer;
using System.Text.RegularExpressions;
using System;
using System.Text.Json;

using IntroSE.Kanban.Backend.BusinessLayer;
using log4net;



namespace IntroSE.Kanban.Backend.ServiceLayer

{

	public class UserService
	{
        private ILog log;
        private UserFacade userFacade;



        internal UserService(UserFacade userFacade,ILog log2)
        {
            this.log = log2;
            this.userFacade = userFacade;

        }
        ///<summary>
        ///this method entring the user to the system
        ///<param "email"> the email address of the user that log in
        ///<returns> a response with the object, unless an error occurs when one of the inputs is wrong according to the data in the program and return an appropriate error massege
        public string Login(string email, string password)
        {
            log.Info($"USERSERVICE login attempt for user: {email} password:{password}");
            try
            {
                userFacade.LogIn(email, password);
                Response response = new Response(null,email);
                log.Info($"USERSERVICE successfull login returning empty JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                //return "Error logging in user: " + ex.Message;
                Response response = new Response(ex.Message);
                log.Error($"USERSERVICE failed login returning json with error message: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }

        ///<summary>
        ///this method create a new user
        ///<param email> the email of the new user
        ///<param "username"> the username of the new user
        ///<param password> the password that a new user choose
        ///<returns> a response with the object, unless an error occurs when one of the inputs is wrong and return an appropriate error massege

        public string Register(string email, string password)
        {
            log.Info($"USERSERVICE tring to REgister user {email} woth password {password}");
            try
            {
                userFacade.Register(email, password);
                Response response = new Response();
                log.Info($"USERSERVICE successfull login returning empty JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex) {
                
                Response response = new Response(ex.Message);
                log.Error($"USERSERVICE registration failed returning this JSON: { JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }

        public string Logout(string email)
        {
            log.Info($"USERSERVICE trying to logout user :{email}");
            try
            {
                userFacade.Logout(email);
                Response response = new Response();
                log.Info($"USERSERVICE successfull logout returning empty json:{JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"USERSERVICE logout failed returning this JSON:{JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }
        internal string LoadUsersData()
        {
            log.Info($"USERSERVICE trying to load all users ");
            try
            {
                userFacade.LoadUsers();
                Response response = new Response();
                log.Info($"USERSERVICE successfull load all users returning empty json:{JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"USERSERVICE load users failed returning this JSON:{JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }

        internal string DeleteUsersData()
        {
            log.Info($"USERSERVICE trying to delete all users ");
            try
            {
                userFacade.DeleteUsersData();
                Response response = new Response();
                log.Info($"USERSERVICE successfull delete all users returning empty json:{JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"USERSERVICE delete users failed returning this JSON:{JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }
    }
}
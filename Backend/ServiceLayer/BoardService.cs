using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Policy;
using System.Text.Json;
using System.Xml.Linq;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.ServiceLayer;
using log4net;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class BoardService
    {
        ILog log;
        internal BoardFacade bf;

        internal BoardService(BoardFacade bf2, ILog log2)
        {
            this.log = log2;
            bf = bf2;
        }

        /// this method creates a new board.
        /// param actor ="user" is the unique name of the user
        /// param String="title" of the board
        /// returns a response with an object
        /// returns an error if the wanted name is taken.
        public string CreateBoard(string email, string title)
        {
            log.Info($"BOARDSERVICE trying to create board {title}{email}");
            try
            {
                bf.CreateBoard(title, email);
                Response response = new Response();
                log.Info($"BOARDSERVICE created board successfully returning empty JSON:  {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error("BOARDSERVICE failed to create board return JSON:  {JsonSerializer.Serialize(response}");
                return JsonSerializer.Serialize(response);
            }
        }



        /// this method gets as input an existing board and it's creator
        /// deletes this board and all its tasks
        /// changes the pointers users have to the board to null.
        public string DeleteBoard(string email, string title)
        {
            log.Info($"BOARDSERVICE trying to delete board {title} for user {email}");
            try
            {
                bf.DeleteBoard(title, email);
                Response response = new Response();
                log.Info($"BOARDSERVICE deleted board {title} successfully for user {email}. Returning empty JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"BOARDSERVICE failed to delete board {title} for user {email}. Error: {ex.Message}. Returning JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }



        /// param "newlimit" = the restriction of the wanted status column.
        /// param " column" = the status between the three: TO DO, IN PROGGRESS, DONE.
        /// this method changes the restriction on the amount of tasks in each status.
        /// returns an error if the newlimit is smaller then the amount of tasks in the specific column.
        public string LimitColumn(string email, string title, int column, int limit)
        {
            log.Info($"BOARDSERVICE trying to set limit for column {column} to {limit} on board {title} for user {email}");
            try
            {
                bf.LimitColumn(title, email, column, limit);
                Response response = new Response();
                log.Info($"BOARDSERVICE successfully set limit for column {column} to {limit} on board {title} for user {email}. Returning empty JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"BOARDSERVICE failed to set limit for column {column} to {limit} on board {title} for user {email}. Error: {ex.Message}. Returning JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }


        public string GetColumnLimit(string email, string boardName, int columnOrdinal)
        {
            log.Info($"BOARDSERVICE trying to get column limit for column {columnOrdinal} on board {boardName} for user {email}");
            try
            {
                int limit = bf.GetColumnLimit(columnOrdinal, boardName, email);
                Response response = new Response(null, limit);
                log.Info($"BOARDSERVICE successfully retrieved column limit for column {columnOrdinal} on board {boardName} for user {email}. Limit: {limit}. Returning JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"BOARDSERVICE failed to retrieve column limit for column {columnOrdinal} on board {boardName} for user {email}. Error: {ex.Message}. Returning JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }



        ///<returns> a new response with a list of boards.
        /// this method goes in loop through all boards and enters a them to a list.
        public string PresentBoards(string email)
        {
            log.Info($"BOARDSERVICE trying to present boards for user {email}");
            try
            {
                List<int> boards = bf.PresentBoards(email);
                int[] boardsID = new int[boards.Count];
                int i = 0;
                foreach (int boardID in boards)
                {
                    boardsID[i] = boardID;
                    i++;
                }
                Response response = new Response(null, boardsID);
                log.Info($"BOARDSERVICE successfully presented boards for user {email}. Returning JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                log.Error($"BOARDSERVICE failed to present boards for user {email}. Error: {ex.Message}. Returning JSON: {JsonSerializer.Serialize(new Response("Error occurred while fetching boards.", ex))}");
                Response errorResponse = new Response(ex.Message);
                return JsonSerializer.Serialize(errorResponse);
            }
        }


        public string InProgressTasks(string email)
        {
            log.Info($"BOARDSERVICE trying to retrieve in-progress tasks for user {email}");
            try
            {
                List<TaskBL> taskBLs = bf.InProgressTasks(email);
                TaskSL[] columnTaskSL = new TaskSL[taskBLs.Count];
                int i = 0;
                foreach (TaskBL taskBL in taskBLs)
                {
                    columnTaskSL[i] = new TaskSL(taskBL);
                    i++;
                }
                Response response = new Response(null, columnTaskSL);
                log.Info($"BOARDSERVICE successfully retrieved in-progress tasks for user {email}. Returning JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                log.Error($"BOARDSERVICE failed to retrieve in-progress tasks for user {email}. Error: {ex.Message}. Returning JSON: {JsonSerializer.Serialize(new Response("Error occurred while fetching in-progress tasks.", null))}");
                Response errorResponse = new Response(ex.Message);
                return JsonSerializer.Serialize(errorResponse);
            }
        }


        public string GetColumnName(int column, string email, string title)
        {
            log.Info($"BOARDSERVICE trying to retrieve column name for column {column} on board {title} for user {email}");
            try
            {
                string columnName = bf.GetColumnName(column, email, title);
                Response response = new Response(null, columnName);
                log.Info($"BOARDSERVICE successfully retrieved column name for column {column} on board {title} for user {email}. Name: {columnName}. Returning JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                log.Error($"BOARDSERVICE failed to retrieve column name for column {column} on board {title} for user {email}. Error: {ex.Message}. Returning JSON: {JsonSerializer.Serialize(new Response($"Error retrieving column name: {ex.Message}", false))}");
                Response response = new Response(ex.Message);
                return JsonSerializer.Serialize(response);
            }
        }
        public string GetColumn(string email, string title, int column)
        {
            try
            {
                List<TaskBL> columnTaskBL = bf.GetColumn(email, title, column);

                TaskSL[] columnTaskSL = new TaskSL[columnTaskBL.Count];
                int i = 0;
                foreach (TaskBL taskBL in columnTaskBL)
                {
                    TaskSL good = new TaskSL(taskBL);
                    columnTaskSL[i] = good;
                    i++;
                    log.Info($"adding task {new TaskSL(taskBL)}");
                    log.Info($"{good.Title} &&&&&&&&&&&&&&&&&&&&&&{good.DueDate}&&&&&&&&&&&&&&&&&&&&&&&&");
                }

                Response response = new Response(columnTaskSL);
                log.Info($"returning Json wwhich should contain array of tasks : {JsonSerializer.Serialize(response.ReturnValue)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="email">the email of the user that want to join a board</param>
        /// <param name="boardID">the ID of the board that the user want to join</param>
        /// if the user is not logged in or not registered the function will throw an exception
        internal string Join(string email, int boardID)
        {
            try
            {
                bf.Join(email, boardID);
                Response response = new Response();
                log.Info($"BOARDSERVICE successfully joined board {boardID} for user {email}. Returning JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"BOARDSERVICE failed to join board {boardID} for user {email}. Error: {ex.Message}. Returning JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="email">the email of the user that want to leave a board</param>
        /// <param name="title">the title of the board that the user want to leave</param>
        /// if the user is not logged in or not registered the function will throw an exception
        internal string Leave(string email, int boardID) {
            log.Info($"BOARDSERVICE trying to leave board {boardID} for user {email}");
            try
            {
                bf.Leave(email, boardID);
                Response response = new Response();
                log.Info($"BOARDSERVICE successfully left board {boardID} for user {email}. Returning JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"BOARDSERVICE failed to leave board {boardID} for user {email}. Error: {ex.Message}. Returning JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email">the email of the owner of the board</param>
        /// <param name="title">the title of the board that the board's owner change</param>
        /// <param name="newOwnerEmail">the email of the new board's owner</param>
        /// if the user is not logged in or not registered or the user is not the owner of the boardthe function will throw an exception
        internal string NewOwner(string email, string title, string newOwnerEmail) {
            log.Info($"BOARDSERVICE trying to change owner of board {title} from {email} to {newOwnerEmail}");
            try
            {
                bf.NewOwner(email, title, newOwnerEmail);
                Response response = new Response();
                log.Info($"BOARDSERVICE successfully changed owner of board {title} to {newOwnerEmail} for user {email}. Returning JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"BOARDSERVICE failed to change owner of board {title} from {email} to {newOwnerEmail}. Error: {ex.Message}. Returning JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }

        internal string BoardTitle(int boardID)
        {
            log.Info($"BOARDSERVICE trying to find board's name ");
            try
            {
                string name = bf.GetBoardTitle(boardID);
                Response response = new Response(null, name);
                log.Info($"BOARDSERVICE successfull find board's name returning empty json:{JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"BOARDSERVICE find board's name failed returning this JSON:{JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }

        internal string LoadBoardsData()
        {
            log.Info($"BOARDSERVICE trying to load all users ");
            try
            {
                bf.LoadBoards();
                Response response = new Response();
                log.Info($"BOARDSERVICE successfull load all boards returning empty json:{JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"BOARDSERVICE load boards failed returning this JSON:{JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }

        internal string DeleteBoardsData() { 
            log.Info($"BOARDSERVICE trying to delete all boards ");
            try
            {
                bf.DeleteBoardData();
                Response response = new Response();
                log.Info($"BOARDSERVICE successfull delete all boards returning empty json:{JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"BOARDSERVICE delete boards failed returning this JSON:{JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }
    }
}


   
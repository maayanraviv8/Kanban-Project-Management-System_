using System;
using System.Text.Json;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.ServiceLayer;
using log4net;
using Microsoft.VisualBasic;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class TaskService
    {
        internal TaskFacade tf;
        private ILog log;

        internal TaskService(TaskFacade tf2,ILog log2)
        {
            tf = tf2;
            this.log = log2;
        }

        ///<summary>
        ///this method create a new task
        ///<param email> the email of the creator
        ///<param title> the tite of the task 
        ///<param boardTitle> the tite of the board 
        ///<param description> the description of the task
        ///<param DueDate> the last date to do the task
        ///<returns> rerurn a new task. appears in the appropriate board. return an error massege if the date is invalid or the string is empty
        public string CreateTask(string email, string title, string boardTitle, string description, DateTime DueDate)
        {
            log.Info($"TASKSERVICE attempting to create task with email: {email} title:{title} boradtitle:{boardTitle} description: {description} duedate:{DueDate}");
            try
            {
                tf.AddTask(email, title, boardTitle, description, DueDate);
                Response response = new Response();
                log.Info($"TASKSERVICE created task successfully returning empty JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"TASKSERVICE: failed to create task send json with errormsg: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }
        ///<summary>
        ///this method moving the task to the next column
        ///<param TID> the unique ID of each task
        public string AdvanceTask(string email, string boardName, int taskID)
        {
            log.Info($"TASKSERVICE attempting to advance task with email: {email}, boardName: {boardName}, taskID: {taskID}");
            try
            {
                tf.AdvanceTask(email, boardName, taskID);
                Response response = new Response();
                log.Info($"TASKSERVICE advanced task successfully returning empty JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"TASKSERVICE: failed to advance task send JSON with error message: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }


        public string UpdateTaskDueDate(string email, string boardName, int taskId, DateTime dueDate)
        {
            log.Info($"TASKSERVICE attempting to update task due date with email: {email}, boardName: {boardName}, taskId: {taskId}, dueDate: {dueDate}");
            try
            {
                tf.UpdateTaskDueDate(email, boardName, taskId, dueDate);
                Response response = new Response();
                log.Info($"TASKSERVICE updated task due date successfully returning empty JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"TASKSERVICE: failed to update task due date send JSON with error message: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }


        /// <summary>
        /// This method updates task title.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="title">New title for the task</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string UpdateTaskTitle(string email, string boardName, int taskId, string title)
        {
            log.Info($"TASKSERVICE attempting to update task title with email: {email}, boardName: {boardName}, taskId: {taskId}, title: {title}");
            try
            {
                tf.UpdateTaskTitle(email, boardName, taskId, title);
                Response response = new Response();
                log.Info($"TASKSERVICE updated task title successfully returning empty JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"TASKSERVICE: failed to update task title send JSON with error message: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }


        /// <summary>
        /// This method updates the description of a task.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="description">New description for the task</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string UpdateTaskDescription(string email, string boardName, int taskId, string description)
        {
            log.Info($"TASKSERVICE attempting to update task description with email: {email}, boardName: {boardName}, taskId: {taskId}, description: {description}");
            try
            {
                tf.UpdateDescription(email, boardName, taskId, description);
                Response response = new Response();
                log.Info($"TASKSERVICE updated task description successfully returning empty JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"TASKSERVICE: failed to update task description send JSON with error message: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email">the email of board member</param>
        /// <param name="boardName">the name of the board</param>
        /// <param name="taskName">the name of the task that add an assugnee</param>
        /// <param name="assignee">the email of the assignee</param>
        /// <returns></returns> if the one of the parameters is invalid or the email is not a member in the board return error message
        /// <exception cref="NotImplementedException"></exception>
        public string Unassigned(string email, string boardName, string assignee, int taskID)
        {
            log.Info($"TASKSERVICE attempting to unassign user {assignee} from task  (ID: {taskID}) on board {boardName} by user {email}");
            try
            {
                tf.Unassigned(email, boardName, assignee, taskID);
                Response response = new Response();
                log.Info($"TASKSERVICE successfully unassigned user {assignee} from task  (ID: {taskID}) on board {boardName}. Returning empty JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"TASKSERVICE failed to unassign user {assignee} from task  (ID: {taskID}) on board {boardName} by user {email}. Error: {ex.Message}. Returning JSON: {JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }

        internal string LoadTasksData()
        {
            log.Info($"TASKSERVICE trying to load all tasks ");
            try
            {
                tf.LoadTasks();
                Response response = new Response();
                log.Info($"TASKSERVICE successfull load all tasks returning empty json:{JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"TASKESERVICE load tasks failed returning this JSON:{JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }

        internal string DeleteTasksData()
        {
            log.Info($"TASKSERVICE trying to delete all tasks ");
            try
            {
                tf.DeleteTasksData();
                Response response = new Response();
                log.Info($"TASKSERVICE successfull delete all tasks returning empty json:{JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(ex.Message);
                log.Error($"TASKESERVICE delete tasks failed returning this JSON:{JsonSerializer.Serialize(response)}");
                return JsonSerializer.Serialize(response);
            }
        }

    }


}

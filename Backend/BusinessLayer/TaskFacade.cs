using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using IntroSE.Kanban.Backend.ServiceLayer;
using log4net;
using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DAO;
using static System.Data.Entity.Infrastructure.Design.Executor;


namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal class TaskFacade
    {
        private readonly Dictionary<string, TaskBL> tasks;//the key is taskTitle+boardTitle
        internal UserFacade uf;
        internal BoardFacade bf;
        TaskController tc;
        internal ILog log;

        internal TaskFacade(UserFacade uf2, BoardFacade bf,ILog log2)
        {
            uf = uf2;
            tasks = new Dictionary<string, TaskBL>();
            this.bf = bf;
            this.log = log2;
            this.tc = new TaskController();
        }
        internal void LoadTasks()
        {
            List<TaskDAO> taskDAOs = tc.GetAllTasks();
            foreach (TaskDAO task in taskDAOs)
            {
                TaskBL newTask = new TaskBL(task);
                string boardName = bf.GetBoard(task.BOARDID).Title;
                tasks.Add(task.TASKTITLE + boardName, newTask);
                bf.AddTaskToBoard(task.BOARDID, newTask);
            }
        }
        internal void DeleteTasksData()
        {
            tc.DeleteAllData();
        }

        private int NextTaskID()
        {
            if (tasks.Count == 0)
            {
                return 0;
            }
            else
            {
                return tasks[tasks.Keys.Last()].GetTaskID() + 1;
            }
        }
        //private void Cheking(string email, string boardTitle)
        //{
        //    log.Info($"TASKFACADE trying checking email : {email} and boardTitle:{boardTitle} validity by sending it to BOARDFACADE");
        //    bf.BoardChecking(email, boardTitle);
        //}
        private TaskBL FindTask(string email, string boardName, int taskID)
        {
            log.Info($"TASKFACADE trying to findtask with email:  {email} boardName : {boardName} taskID: {taskID}");

            if (!bf.BoardChecking(email, boardName)) { throw new Exception("User is not a member of the board"); }
            List<TaskBL> tasks = bf.GetTasksOfBoard(email, boardName);
            TaskBL currentTask = null;
            log.Info($"looping through the tasks of board {boardName} that bwlongs to {email}");
            foreach (TaskBL task in tasks)
            {
                log.Info($"checking task with id  {task.GetTaskID} ");
                if (task.GetTaskID() == taskID) {
                    log.Info($"{task.GetTaskID()} made a match! returning this task");
                    return currentTask = task; 
                }
                log.Info("no match");
            }
            log.Info($"didnt gind a match returning {boardName}");
            return currentTask;
        }
        
        //internal List<TaskBL> GetAllTasks(int boardID)
        //{
        //    List<TaskBL> tasksList = new List<TaskBL>();
        //    foreach (TaskBL task in tasks.Values)
        //    {
        //        if(task.GetTaskDAO.BoardID == boardID) { tasksList.Add(task); }
        //    }
        //    return tasksList;
        //}
    
        internal void AddTask(string email, string title, string boardTitle, string description, DateTime DueDate)//updated function
        {
            log.Info($"TASKFACADE attempting to add task with email: {email}, title: {title}, boardTitle: {boardTitle}, description: {description}, dueDate: {DueDate}");

            if (email == null)
            {
                log.Error("TASKFACADE: email cannot be null");
                throw new Exception("Email cannot be null");
            }

            if (!bf.BoardChecking(email, boardTitle)) { throw new Exception("User is not a member of the board"); }
            //if (!bf.IsMember(email, bf.GetBoardID(email, title))){ throw new Exception($"User {email} is not a member in the board {boardTitle}"); }
            if (title == null)
            {
                log.Error("TASKFACADE: title cannot be null");
                throw new Exception("Title cannot be null");
            }

            if (tasks.Keys.Contains(title + boardTitle))
            {
                log.Error($"TASKFACADE: There exists a task with title {title} in board {boardTitle}");
                throw new Exception($"There exists a task with title {title} in board {boardTitle}");
            }

            if (description == null)
            {
                log.Error("TASKFACADE: description cannot be null");
                throw new Exception("Description cannot be null");
            }
            int limit = bf.GetColumnLimit(BoardFacade.BACKLOG, boardTitle, email);
            if (limit != -1)
            {
                int taskCounter = bf.GetTaskCount(email, boardTitle, BoardFacade.BACKLOG);
                if (taskCounter == limit) { throw new Exception($"The column is full, the column limit is {limit} tasks"); }
            }
            int boardId = bf.GetBoardID(email, boardTitle);
            TaskBL newTask = new TaskBL(NextTaskID(), title, description, DueDate, boardId);

            tasks.Add(title+boardTitle , newTask);
            bf.AddTaskToBoard(boardId, newTask);
            tc.CreateTask(newTask.GetTaskDAO);

            log.Info($"TASKFACADE successfully added task: {title} to board: {boardTitle}");
        }


        internal void AdvanceTask(string email, string boardName, int taskID)//updated function
        {
            log.Info($"TASKFACADE attempting to advance task with email: {email}, boardName: {boardName}, taskID: {taskID}");

            if (!bf.BoardChecking(email, boardName)) { throw new Exception("User is not a member of the board"); }

            TaskBL currentTask = FindTask(email, boardName, taskID);
            if (currentTask == null)
            {
                log.Error($"TASKFACADE: There is no task with ID {taskID} in board {boardName}");
                throw new Exception($"There is no task with ID {taskID} in board {boardName}");
            }
            if(!currentTask.Assignee.Equals(email))
            {
                log.Error($"TASKFACADE: The task {currentTask.Title} is not assigned to {email}");
                throw new Exception($"The task {currentTask.Title} is not assigned to {email}");
            }
            int currentColumn = currentTask.GetStatus();

            if (currentColumn == BoardFacade.BACKLOG)
            {
                int limit = bf.GetColumnLimit(BoardFacade.BACKLOG, boardName, email);
                if (limit != -1)
                {
                    int taskCounter = bf.GetTaskCount(email, boardName, BoardFacade.BACKLOG);
                    if (taskCounter == limit) { throw new Exception($"The column is full, the column limit is {limit} tasks"); }
                }
                currentTask.Statuse = BoardFacade.INPROGRESS;
                currentTask.GetTaskDAO.STATUS = BoardFacade.INPROGRESS;
                log.Info($"TASKFACADE: Task {taskID} advanced to In Progress");
            }
            else if (currentColumn == BoardFacade.INPROGRESS)
            {
                int limit = bf.GetColumnLimit(BoardFacade.INPROGRESS, boardName, email);
                if (limit != -1)
                {
                    int taskCounter = bf.GetTaskCount(email, boardName, BoardFacade.INPROGRESS);
                    if (taskCounter == limit) { throw new Exception($"The column is full, the column limit is {limit} tasks"); }
                }
                currentTask.Statuse = BoardFacade.DONE;
                currentTask.GetTaskDAO.STATUS = BoardFacade.DONE;
                log.Info($"TASKFACADE: Task {taskID} advanced to Done");
            }
            else
            {
                log.Error($"TASKFACADE: Task {currentTask.Title} is already done");
                throw new Exception($"Task {currentTask.Title} is already done");
            }
        }

        internal void UpdateTaskDueDate(string email, string boardName, int taskID, DateTime dueDate)
        {
            log.Info($"TASKFACADE attempting to update task due date with email: {email}, boardName: {boardName}, taskID: {taskID}, dueDate: {dueDate}");

            if (!bf.BoardChecking(email, boardName)) { throw new Exception("User is not a member of the board"); }

            if (dueDate < DateTime.Now)
            {
                log.Error($"TASKFACADE: The date {dueDate} has already passed");
                throw new Exception($"The date {dueDate} has already passed");
            }

            TaskBL currentTask = FindTask(email, boardName, taskID);
            if (currentTask == null)
            {
                log.Error($"TASKFACADE: There is no task with ID {taskID} in board {boardName}");
                throw new Exception($"There is no task with ID {taskID} in board {boardName}");
            }

            if (!currentTask.Assignee.Equals(email))
            {
                log.Error($"TASKFACADE: The task {currentTask.Title} is not assigned to {email}");
                throw new Exception($"The task {currentTask.Title} is not assigned to {email}");
            }

            if (currentTask.Statuse == BoardFacade.DONE)
            {
                log.Error($"TASKFACADE: Task {currentTask.Title} is already done and cannot be edited");
                throw new Exception($"Task {currentTask.Title} is already done and cannot be edited");
            }

            currentTask.Deadline = dueDate;
           currentTask.GetTaskDAO.DUEDATE = dueDate;
            log.Info($"TASKFACADE: Task {taskID} due date updated successfully to {dueDate}");
        }

        internal void UpdateTaskTitle(string email, string boardName, int taskID, string taskTitle)
        {
            log.Info($"TASKFACADE attempting to update task title with email: {email}, boardName: {boardName}, taskID: {taskID}, taskTitle: {taskTitle}");

            if (!bf.BoardChecking(email, boardName)) { throw new Exception("User is not a member of the board"); }

            if (taskTitle == null || taskTitle.Length < TaskBL.MINIMALTITLE || taskTitle.Length > TaskBL.MAXIMALTITLE)
            {
                log.Error($"TASKFACADE: {taskTitle} is an invalid title");
                throw new Exception(taskTitle + " is an invalid title");
            }

            TaskBL currentTask = FindTask(email, boardName, taskID);
            if (currentTask == null)
            {
                log.Error($"TASKFACADE: There is no task with ID {taskID} in board {boardName}");
                throw new Exception($"There is no task with ID {taskID} in board {boardName}");
            }
            if (!currentTask.Assignee.Equals(email))
            {
                log.Error($"TASKFACADE: The task {currentTask.Title} is not assigned to {email}");
                throw new Exception($"The task {currentTask.Title} is not assigned to {email}");
            }
            if (currentTask.Statuse == BoardFacade.DONE)
            {
                log.Error($"TASKFACADE: Task {currentTask.Title} is already done and cannot be edited");
                throw new Exception("Task " + currentTask.Title + " is already done and cannot be edited");
            }

            currentTask.Title = taskTitle;
            currentTask.GetTaskDAO.TASKTITLE = taskTitle;
            log.Info($"TASKFACADE: Task {taskID} title updated successfully to {taskTitle}");
        }

        internal void UpdateDescription(string email, string boardName, int taskID, string description)
        {
            log.Info($"TASKFACADE attempting to update task description with email: {email}, boardName: {boardName}, taskID: {taskID}, description: {description}");

            if (!bf.BoardChecking(email, boardName)) { throw new Exception("User is not a member of the board"); }
            if (description == null || description.Length<TaskBL.MINIMALDESCREPTION || description.Length>TaskBL.MAXIMALDESCREPTION) { throw new Exception("invalid title"); }

            TaskBL currentTask = FindTask(email, boardName, taskID);
            if (currentTask == null)
            {
                log.Error($"TASKFACADE: There is no task with ID {taskID} in board {boardName}");
                throw new Exception($"There is no task with ID {taskID} in board {boardName}");
            }
            if (!currentTask.Assignee.Equals(email))
            {
                log.Error($"TASKFACADE: The task {currentTask.Title} is not assigned to {email}");
                throw new Exception($"The task {currentTask.Title} is not assigned to {email}");
            }

            if (currentTask.Statuse == BoardFacade.DONE)
            {
                log.Error($"TASKFACADE: Task {currentTask.Title} is already done and cannot be edited");
                throw new Exception("Task " + currentTask.Title + " is already done and cannot be edited");
            }

            currentTask.Description = description;
            currentTask.GetTaskDAO.TASKDESCRIPTION = description;
            log.Info($"TASKFACADE: Task {taskID} description updated successfully");
        }
        internal void DeleteTask(string boardTitle, List<TaskBL> deleteTasks)//new function to delete tasks from deleted board
        {
            foreach (TaskBL task in deleteTasks)
            {
                this.tasks.Remove(task.Title + boardTitle);
                tc.DeleteTask(task.GetTaskDAO);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="email">email of one of the board member</param>
        /// <param name="boardName">the name of the board that want to change a task</param>
        /// <param name="taskName">name of the task</param>
        /// <param name="assignee">email of the new assignee</param>
        internal void Unassigned(string email, string boardName, string assignee, int taskID)
        {
            if(email == null || boardName == null ||  assignee == null || taskID<BoardFacade.EMPTY)
            {
                throw new Exception("One of the parameters is null");
            }
            if (!bf.BoardChecking(email, boardName)) { throw new Exception("User is not a member of the board"); }
            if (!bf.BoardChecking(assignee, boardName)) { throw new Exception($"User{assignee} is not a member of the board"); }
            TaskBL currentTask = FindTask(email, boardName, taskID);
            if (currentTask == null)
            {
                throw new Exception("There is no task with this ID");
            }
            if(currentTask.Assignee != null) { throw new Exception($"The task is already unassigned to {currentTask.Assignee}");}
            if(currentTask.Statuse == BoardFacade.DONE) { throw new Exception("The task is already done");}
            currentTask.Assignee = assignee;
            currentTask.GetTaskDAO.ASSIGNEE = assignee;

        }



    }
}

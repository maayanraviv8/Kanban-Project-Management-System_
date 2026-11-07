using IntroSE.Kanban.Backend.DataAccessLayer.DAO;
using IntroSE.Kanban.Backend.DataAccessLayer;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal class TaskBL
    {
        protected int titleID;
        protected string title;
        private string description;
        private int status;
        private DateTime creationTime;
        private DateTime deadline;
        private string assignee;
        private TaskDAO taskDAO;
        internal static int MINIMALDESCREPTION = 0;
        internal static int MAXIMALDESCREPTION = 300;
        internal static int MINIMALTITLE = 1;
        internal static int MAXIMALTITLE = 50;
      
        internal TaskBL(int titleID, string title, string description, DateTime deadLine, int boardID)
        {   //we need to check here that all elements are correct
            DateTime creation = DateTime.Now;
            this.taskDAO = new TaskDAO(titleID, title, description, creation, deadLine, null, boardID ,BoardFacade.BACKLOG);
            this.titleID = titleID; // in order to call special Title setter that makes the checks maybe should use this.Title = title
            this.Title = title;
            this.Description = description;
            this.Statuse = status;
            this.Assignee = null;
            this.creationTime = creation;
            this.Deadline = deadline;
            taskDAO.persist();
        }

        internal TaskBL(TaskDAO task)
        {
            this.titleID = task.TASKID;
            this.title = task.TASKTITLE;
            this.description = task.TASKDESCRIPTION;
            this.creationTime = task.CREATIONTIME;
            this.deadline = task.DUEDATE;
            this.assignee = task.ASSIGNEE;
            this.status = task.STATUS;




        }
        internal TaskDAO GetTaskDAO { get => taskDAO; }
        internal int Statuse
        {
            get => status;
            set => status = value;
        }

        internal string Assignee
        {
            get => assignee;
            set => assignee = value;
        }
        internal string Title
        {
            get => title;
            set 
            {
                //if (Regex.IsMatch(value, @"^.{1,50}$"))
                if(value.Length>=MINIMALTITLE && value.Length<=MAXIMALTITLE)
                {
                    title = value;
                }
                else
                {
                    throw new ArgumentException("Title must be between 1 and 50 characters long.");
                }
            }

        }
        internal string Description
        {
            get => description;
            set
            {
                if (string.IsNullOrEmpty(value) || (value.Length > MINIMALDESCREPTION && value.Length <= MAXIMALDESCREPTION))
                {
                    description = value;
                }
                else
                {
                    throw new ArgumentException("Description must be between 1 and 300 characters if provided.");
                }
            }
        }
        internal DateTime CreationTime { get => creationTime; }
      
        internal DateTime Deadline
        {
            get => deadline;
            set
            {
                if (value < creationTime || value < DateTime.Now)
                {
                    throw new ArgumentException("Deadline cannot be earlier than the creation time or the current date/time.");
                }
                deadline = value;
            }
        }

        internal int GetTaskID()
        {
            return this.titleID;
        }
        internal int GetStatus()
        {
            return status;
        }
        
        
    }
}


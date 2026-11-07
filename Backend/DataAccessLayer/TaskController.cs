using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DAO;
using log4net;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    internal class TaskController : AbstractController<TaskDAO, int>
    {

        internal TaskController() : base()
        {

        }


        public TaskDAO ConvertReaderToObject(SQLiteDataReader reader)
        {
            int taskID = reader.GetInt32(0);              
            string taskTitle = reader.GetString(1);
            int status = reader.GetInt32(2);
            DateTime creationTime = reader.GetDateTime(3);
            DateTime dueDate = reader.GetDateTime(4);
            string taskDescription = reader.GetString(5);
            string assignee = reader.GetString(6);       
            int boardID = reader.GetInt32(7);            

            return new TaskDAO(taskID, taskTitle, taskDescription, creationTime, dueDate, assignee, boardID, status);
        }



        /// <summary>
        /// returns all tasks i nDB
        /// </summary>
        /// <returns></returns>
        internal List<TaskDAO>  GetAllTasks()
        {
            return GetAll("Task", ConvertReaderToObject);

        }


        /// <summary>
        /// adds task to DB
        /// </summary>
        /// <param name="tdao"></param>
        internal bool CreateTask(TaskDAO tdao)
        {
            string commando = $"INSERT INTO {tdao._tableName} ({tdao.ColumnNames[0]}, {tdao.ColumnNames[1]}, {tdao.ColumnNames[2]}, {tdao.ColumnNames[3]}, {tdao.ColumnNames[4]}, {tdao.ColumnNames[5]},{tdao.ColumnNames[6]}, {tdao.ColumnNames[7]}) " +
                              "VALUES (@taskID, @taskTitle, @status,  @creationTime, @dueDate,@taskDescription, @assignee, @boardID)";

            Add(commando, command =>
            {
                command.Parameters.AddWithValue("@taskID", tdao.TASKID);
                command.Parameters.AddWithValue("@taskTitle", tdao.TASKTITLE);
                command.Parameters.AddWithValue("@taskDescription", tdao.TASKDESCRIPTION);
                command.Parameters.AddWithValue("@creationTime", tdao.CREATIONTIME);
                command.Parameters.AddWithValue("@dueDate", tdao.DUEDATE);
                command.Parameters.AddWithValue("@assignee", tdao.ASSIGNEE);
                command.Parameters.AddWithValue("@boardID", tdao.BOARDID);
                command.Parameters.AddWithValue("@status", tdao.STATUS);
            });

            string commando2 = $"INSERT INTO BoardTask (BoardID, TaskID) VALUES (@boardID, @taskID)";
            return Add(commando2, command =>
            {
                command.Parameters.AddWithValue("@boardID", tdao.BOARDID);
                command.Parameters.AddWithValue("@taskID", tdao.TASKID);
            });
        }



        /// <summary>
        /// deletes task from DB
        /// </summary>
        /// <param name="TaskID"></param>
        internal bool DeleteTask (TaskDAO tasko)
        {

            string deleteTasks = $"DELETE FROM {tasko._tableName} WHERE {tasko.ColumnNames[0]} = @taskID";
            Delete(deleteTasks, command =>
            {
                command.Parameters.AddWithValue("@taskID",tasko.TASKID);
            });

            string deltetasks2 = $"DELETE FROM BoardTask WHERE  TaskID = @taskID";
            return Delete(deltetasks2, command =>
            {
                command.Parameters.AddWithValue("@taskID", tasko.TASKID);
            });

        }

        /// <summary>
        /// one function to update all a tasks parameters
        /// </summary>
        /// <param name="tdao"></param>
        internal void  UpdateTask(int id, string attributeName, string attributeValue)
        {
            Update("Task", id, attributeName,  "TaskID",attributeValue);

        }
        internal void UpdateTask(int id, string attributeName, int attributeValue)
        {
            Update("Task", id, attributeName, "TaskID", attributeValue);

        }



    
    }
}

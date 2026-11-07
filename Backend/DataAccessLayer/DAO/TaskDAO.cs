using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DAO
{
    internal class TaskDAO:absDAO<int>
    {
        private int TaskID;
        private string TaskTitle;
        private string TaskDescription;
        private DateTime CreationTime;
        private DateTime Duedate;
        private string assignee;
        private int BoardID;
        private int status;//0 for backlog 1 in progress 2 done
        TaskController tc = new TaskController();

        internal override int PrimaryKey => TaskID;
        internal override string Primarycolumn => "taskID";

        internal override List<string> ColumnNames => new List<string> { "taskID", "tasktitle", "status","creationtime","duedate","description","asignee","boardID" };
        internal override string _tableName => "Task";


        //constructor with no boardid for business layer
        internal TaskDAO(int taskID, string taskTitle, string taskDescription, DateTime creationTime, DateTime duedate, string assignee, int status)
        {

            this.TaskID = taskID;
            this.TaskTitle = taskTitle;
            this.TaskDescription = taskDescription;
            this.CreationTime = creationTime;
            this.Duedate = duedate;
            this.assignee = assignee;
            this.status = status;
        }

        //constructor with boardid for dataaccesslayer to buld dao objects
        internal TaskDAO( int taskID, string taskTitle, string taskDescription, DateTime creationTime, DateTime duedate, string assignee, int boardID, int status)
        {
 
            this.TaskID = taskID;
            this.TaskTitle = taskTitle;
            this.TaskDescription = taskDescription;
            this.CreationTime = creationTime;
            this.Duedate = duedate;
            this.assignee = assignee;
            this.status = status;
        }

        internal int TASKID
        {
            get => TaskID;
        }

        internal string TASKTITLE
        {
            get => TaskTitle;
            set
            {
                this.TaskTitle = value;
                if (Ispersisted)
                    tc.UpdateTask(BoardID, ColumnNames[1], value);
            }
        }



        internal DateTime CREATIONTIME
        {
            get => CreationTime;
            set
            {
                this.CreationTime = value;
                if (Ispersisted)
                    tc.UpdateTask(BoardID, ColumnNames[3], value.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        internal DateTime DUEDATE
        {
            get => Duedate;
            set
            {
                this.Duedate = value;
                if (Ispersisted)
                    tc.UpdateTask(BoardID, ColumnNames[4], value.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }


        internal string TASKDESCRIPTION
        {
            get => TaskDescription;
            set
            {
                this.TaskDescription = value;
                if (Ispersisted)
                    tc.UpdateTask(BoardID, ColumnNames[5], value);
            }
        }

        internal string ASSIGNEE
        {
            get => assignee;
            set
            {
                this.assignee = value;
                if (Ispersisted)
                    tc.UpdateTask(BoardID, ColumnNames[6], value);
            }
        }

        internal int BOARDID
        {
            get => BoardID;
            set
            {
                this.BoardID = value;
                if (Ispersisted)
                    tc.UpdateTask(BoardID, ColumnNames[7], value.ToString());
            }
        }

        internal int STATUS
        {
            get => status;
            set
            {
                this.status = value;
                if (Ispersisted)
                    tc.UpdateTask(BoardID, ColumnNames[2], value.ToString());
            }
        }


        internal void persist()
        {
           
        }

        internal void persist(int BoardID)
        {
            if (BoardID < 0)
                throw new ArgumentException("trying to persist task with boardid no good");

            this.BoardID = BoardID;


            if (!Ispersisted)//only insert if hasnt been inserted
            {
                tc.CreateTask(this);
                this.Ispersisted = true;
            }
        }
    }
}

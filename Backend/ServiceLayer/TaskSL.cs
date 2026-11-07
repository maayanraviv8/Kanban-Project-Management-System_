using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    internal class TaskSL
    {

        protected long id;
        public DateTime creationTime;
        public String description;
        public String title;
        public DateTime dueDate;
        public string assignee;

        public TaskSL(TaskBL t1)
        {
            this.id = t1.GetTaskID();
            this.title = t1.Title;
            this.description = t1.Description;
            this.creationTime = t1.CreationTime;
            this.dueDate = t1.Deadline;
            this.assignee = t1.Assignee;
        }
        public long Id { get => this.id; }
        public DateTime CreationTime { get => creationTime; }
        public String Description { get => description; }
        public String Title { get => title; }
        public DateTime DueDate { get => dueDate; }

    }
}

using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    internal class BoardSL
    {
        protected int boardID;
        public string title;
        public List<TaskSL> tasks;
        public int[] limitColumn ;
        public string owner;
        public List<UserSL> members;

        public BoardSL (BoardBL b1)
        {
            this.boardID = b1.BoradID;
            this.title = b1.Title; 
            this.owner = b1.Owner;
            tasks = new List<TaskSL>();
            foreach (TaskBL task in b1.tasks)
                this.tasks.Add(new TaskSL(task));
            this.limitColumn = b1.LimitColumn;
            members = new List<UserSL>();
            foreach (UserBL user in b1.Members)
            {
                this.members.Add(new UserSL(user));
            }

         }

    }
}

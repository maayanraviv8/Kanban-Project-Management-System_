using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DAO;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.BusinessLayer
{

    internal class BoardBL

    {
        protected string title;
        private int boardID;
        internal readonly List<TaskBL> tasks;
        internal int[] Limit_colummn;
        private string owner;
        private List<UserBL> members;
        private BoardDAO boardDAO;



        internal BoardBL(string t, int boardID, string owner)

        { // ill have the right board id from board facade
            this.boardDAO = new BoardDAO(boardID, t, owner, -1, -1, -1);
            this.Title = t;
            this.boardID = boardID;
            this.Owner = owner;
            this.tasks = new List<TaskBL>();
            Limit_colummn = new int[3];
            Limit_colummn[0] = -1;
            Limit_colummn[1] = -1;
            Limit_colummn[2] = -1;
            this.members = new List<UserBL>();
            boardDAO.Persist();
        }
        internal BoardBL (BoardDAO board, List<UserBL> members)
        {
            this.title = board.BOARDTITLE;
            this.boardID = board.BOARDID;
            this.owner = board.BOARDOWNER;
            this.boardDAO = board;
            this.tasks = new List<TaskBL>();
            this.Limit_colummn = new int[3];
            this.LimitColumn[0] = board.LIMIT0;
            this.LimitColumn[1] = board.LIMIT1;
            this.LimitColumn[2] = board.LIMIT2;
            this.members = members;

        
        }

        internal string Owner { get => owner;
            set { if (value == null || value.Equals("")) { throw new Exception("illeagal board owner"); } owner = value; }
                 }
        internal void ChangeLimit(int column, int limit)
        {
            this.LimitColumn[column] = limit;
        }

        internal string Title
        {
            get { return title; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Board title cant be null or empty ");
                title = value;
            }
        }
        internal int BoradID { get => boardID; }
        internal int[] LimitColumn { get => Limit_colummn; }
        internal List<UserBL> Members { get => members; }
        internal int GetBoardID()
        {

        return boardID; 
        }
        internal List<TaskBL> Tasks { get => tasks; }
    

        internal int TaskCounter(int column)
        {
            int output = 0;
            foreach (TaskBL task in tasks)
            {
                if (task.GetStatus() == column)
                {
                    output++;
                }
            }
            return output;
        }

      
        internal BoardDAO GetBoardDAO { get => boardDAO; }
    }
}

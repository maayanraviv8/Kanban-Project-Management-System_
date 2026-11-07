using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DAO
{
    internal class BoardDAO:absDAO<int>
    {
        private int BoardID;
        private string BoardTitle;
        private string owner;
        private int Limit0;
        private int Limit1;
        private int Limit2;
        internal override int PrimaryKey => BoardID;
        internal override string Primarycolumn => "boardID";
        internal override List<string> ColumnNames => new List<string> { "boardID", "title","owner","Limit0","Limit1","Limit2" };
        internal override string _tableName => "Board";

        BoardController bordc = new BoardController();


        internal BoardDAO(int bid,string btitle,string owner2,int limit0,int limit1,int limit2)
        {
            BoardID = bid;
            BoardTitle = btitle;
            owner = owner2;
            this.Limit0 = limit0;
            this.Limit1 = limit1;
            this.Limit2 = limit2;
        }

        internal int BOARDID { get => BoardID;
           }

        internal string BOARDTITLE { get => BoardTitle;
                set {
                this.BoardTitle = value;
                if (Ispersisted)
                    bordc.UpdateBoard(BoardID, ColumnNames[1], value);
            } }

        internal string BOARDOWNER
        {
            get => owner;
            set
            {
                this.BoardTitle = value;
                if (Ispersisted)
                    bordc.UpdateBoard(BoardID, ColumnNames[2], value);
            }
        }

        internal int LIMIT0
        {
            get => Limit0;
            set {
                this.Limit0 = value;
                if (Ispersisted)
                    bordc.UpdateBoard(BoardID, ColumnNames[3], value);
            }
        }

        internal int LIMIT1
        {
            get => Limit1;
            set
            {
                this.Limit1 = value;
                if (Ispersisted)
                    bordc.UpdateBoard(BoardID, ColumnNames[4], value);
            }
        }

        internal int LIMIT2
        {
            get => Limit2;
            set
            {
                this.Limit2 = value;
                if (Ispersisted)
                    bordc.UpdateBoard(BoardID, ColumnNames[5], value);
            }
        }




        internal void Persist()
        {
            if (!Ispersisted)//only insert if hasnt been inserted
            {
                bordc.CreateBoard(this);
                this.Ispersisted = true;
            }
        }

        internal void HlepTaskPersist(TaskDAO tdao)
        {
            tdao.persist(this.BoardID);
        }
    }
}

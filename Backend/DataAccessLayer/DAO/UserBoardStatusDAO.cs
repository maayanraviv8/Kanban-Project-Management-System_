using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DAO
{
    internal class UserBoardStatusDAO:absDAO<string>
    {
        internal string email;
        internal int BoardID;
        internal int relation;

        internal UserBoardStatusDAO(string email, int boardID, int relation)
        {
            this.email = email;
            this.BoardID = boardID;
            this.relation = relation;
        }

        internal override string PrimaryKey => email;

        internal override string Primarycolumn =>"email";

        internal override List<string> ColumnNames => new List<string> { "email", "BoardID","status" };

        internal override string _tableName => "UserBoardStatus";
    }
}

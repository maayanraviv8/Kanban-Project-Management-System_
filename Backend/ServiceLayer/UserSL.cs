using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    internal class UserSL
    {
        public String email;
        public List<BoardSL> boards;

        public UserSL(UserBL u1)
        {
            this.email = u1.Email; 
            this.boards = new List<BoardSL>();
        }
    }
}

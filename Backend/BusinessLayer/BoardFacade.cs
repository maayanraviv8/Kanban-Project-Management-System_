using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DAO;
using IntroSE.Kanban.Backend.ServiceLayer;
using log4net;
using log4net.Repository.Hierarchy;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal class BoardFacade
    {
        private ILog log;
        private readonly Dictionary<string, BoardBL> boardsList;
        private readonly UserFacade userFacade;
        private readonly TaskFacade taskFacade;
        private BoardController bc;
        internal static int EMPTY=0;
        internal static int BACKLOG=0;
        internal static int INPROGRESS=1;
        internal static int DONE=2;
        internal static int MINIMALLIMIT = 1;
        internal BoardFacade(UserFacade userFacade,ILog log2, TaskFacade taskFacade)
        {            
            this.log= log2;
            boardsList = new Dictionary<string, BoardBL>();
            this.userFacade = userFacade;
            bc = new BoardController();
            
        }

        internal void LoadBoards()
        {
            List<BoardDAO> boardDAOs = bc.GetAllBoards();
            foreach (BoardDAO board in boardDAOs)
            {
                BoardBL boardBL = new BoardBL(board, userFacade.Members(bc.GetMembers(board.BOARDID)));
                boardsList.Add(board.BOARDTITLE, boardBL);
            }
        }

        internal void DeleteBoardData()
        {
            bc.DeleteAllData();
        }


        internal void UserChecking(string email)
        {
            log.Info($"BOARDFACADE making sure {email} is registered");
            log.Info($"BOARDFACADE making sure {email} is loggedin");
            if (!userFacade.Contains(email)) {
                throw new Exception($"User {email} is not register");
            }
            if (!userFacade.IsLoggedIn(email)) {
                throw new Exception($"User {email} is logged out");
            }
            log.Info($"BOARDFACADE sees that {email} is registered and loggedin");

        }

        //internal void BoardChecking(string email, string title)
        //{
        //    log.Info($"BOARDFACADE checking if the board ||{title}{email}|| exists");


        //        if (!boardsList.ContainsKey(title + email))
        //        {
        //            log.Error($"BOARDFACADE found that the board {email}{title} DOESNT EXIST ");
        //            throw new Exception($"There is no board with {title} name");
        //        }
        //        log.Info($"BOARDFACADE found the board {title}{email}");

        //}

        internal bool BoardChecking(string email, string title){ //return true if the user is a member of the board and false otherwise
            if (!boardsList.ContainsKey(title))
            {
                throw new Exception("Board does not exist");
            }
            foreach (string t in boardsList.Keys)
            {
                if (t.Equals(title))
                {
                    foreach (UserBL user in boardsList[t].Members)
                    {
                        if (user.Email.Equals(email))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private int NextBoardID()//new function for board id
        {
            if (boardsList.Count == 0)
            {
                return 0;
            }
            else
            {
                return boardsList[boardsList.Keys.Last()].GetBoardID() + 1;
            }
        }
        internal BoardBL GetBoard(int id)//new function for get the board
        {
            if(id==null || id<EMPTY) { throw new Exception("Invalid board ID"); }
            foreach (BoardBL board in boardsList.Values)
            {
                if (board.GetBoardID() == id)
                {
                    return board;
                }
            }
            return null;
        }

        internal int GetBoardID(string email, string title)//new function for get the board id
        {
            if(!BoardChecking(email, title)) { throw new Exception("User is not a member of the board"); }
            foreach (BoardBL board in boardsList.Values)
            {
                if (board.Title.Equals(title) && board.Members.Any(member => member.Email.Equals(email)))
                {
                    return board.GetBoardID();
                }
            }
            return -1;
        }
        private bool IsOwner(string email, int id)//new function for checking if the user is the owner of the board
        {
            if (!(IsMember(email, id)))
            {
                throw new Exception("User is not a member of the board");
            }
            BoardBL board = GetBoard(id);
            if(board==null) { throw new Exception("Board does not exist"); }
            return board.Owner.Equals(email);
        }

        internal bool IsMember(string email, int id)//new function for checking if the user is a member of the board
        {
            BoardBL board = GetBoard(id);
            if (board == null) { throw new Exception("Board does not exist"); }
            foreach (UserBL user in board.Members)
            {
                if (user.Email.Equals(email))
                {
                    return true;
                }
            }
            return false;
        }

        internal string FindEmailCreator(string boardTitle, int boardID)//new function to find the mail of the board creator
        {
            BoardBL board = GetBoard(boardID);
            foreach(KeyValuePair<string, BoardBL> bl in boardsList) {
                if (bl.Value.Equals(board)) {
                    if (bl.Key.StartsWith(boardTitle))
                    {
                        return bl.Key.Substring(boardTitle.Length);
                    }
                }
            }
            return null;

        }

        internal void CreateBoard(string title, string email)
        {
            log.Info($"BOARDFACADE trying to create board {title}{email}");
            if (email == null) {
                log.Error("BOARDFACADE email was null so cant create board");
                throw new Exception("There is no email"); 
            }
            UserChecking(email);
            if (title == null) {
                log.Error("BOARDFACADE no title so cant create board");

                throw new Exception("There is no title"); 
            }
            if (boardsList.ContainsKey(title)) {
                BoardBL b = boardsList[title];
                if(IsMember(email, b.BoradID)){
                    log.Error("BOARDFACADE board with such a name already exists!");
                    throw new Exception($"You already have a board with {title} name");
                }
            }
            int id = NextBoardID();
            BoardBL boardBL = new BoardBL(title, id, email);//in the background the constructor will string title and mail to create is
            boardBL.Owner = email;
            boardBL.Members.Add(userFacade.GetUser(email));
            log.Info("BOARDFACADE created the board successfully");
            boardsList.Add(title , boardBL);
            bc.CreateBoard(boardBL.GetBoardDAO);
        }
        internal void DeleteBoard(string boardName, string email)//updated- delete board's tasks from all tasks
        {
            log.Info($"BOARDFACADE trying to delete board {boardName}{email}");

            if (email == null) {
                log.Error("BOARDFACADE email was null so cant delete board");
                throw new Exception("There is no email");
            }
            UserChecking(email);
            int boardID = GetBoardID(email, boardName);
            BoardBL board = GetBoard(boardID);
            if (board == null)
            {
                log.Error("BOARDFACADE no title so cant delete board");
                throw new Exception("There is no title");
            }
            string title = board.Title;
            
            //if (!boardsList.ContainsKey(title))
            //{
            //    log.Error("BOARDFACADE no such board exists so cant delete it");
            //     throw new Exception($"You dont have a board eith {title} name so cant delete it");
            //}
            if (!IsOwner(email, boardID))
            {
                throw new Exception("You are not the owner of this board");
            }
            List<TaskBL> tasks = board.Tasks;
            string boardTitle = board.Title;
            taskFacade.DeleteTask(boardTitle, tasks);
            int id = board.GetBoardID();
            log.Info("BOARDFACADE successfully deleted this board");
            foreach(string key in boardsList.Keys)
            {
                BoardBL boardBL = boardsList[key];
                if (boardBL.BoradID == id)
                {
                    boardsList.Remove(key);
                    bc.DeleteBoard(boardBL.GetBoardDAO);
                    break;
                }
            }
        }

        internal void LimitColumn(string title, string email, int column, int limit)
        {
            log.Info($"BOARDFACADE going to board {title}{email} and trying to limit column number{column} to {limit} tasks");
            if (email == null) {
                log.Error("BOARDFACADE no email given so cant limitcolumn");
                throw new Exception("There is no email"); 
            }
            UserChecking(email);
            if (title == null) {
                log.Error("BOARDFACADE no valid title given so cant limitcolumn");
                throw new Exception("The title is invalid"); 
            }
            if(!BoardChecking(email, title)) { throw new Exception("User is not a member of the board"); }

            if(column!=BACKLOG && column != INPROGRESS && column!=DONE) {
                log.Error("BOARDFACADE invalid column");
                throw new Exception("Limit value is invalid. For 'backlog' column enter 0 and for 'in progress' column enter 1");
            }
            if (limit < MINIMALLIMIT) { throw new Exception($"Limit {limit} is invalid"); }
            BoardBL board = GetBoard(GetBoardID(email,title));
            if (board.TaskCounter(column) > limit) {
                log.Error($"BOARDFACADE you have {board.TaskCounter} tasks in this column and you are trying to limit it to {limit} ");

                throw new Exception($"You have too many tasks in this column for this {limit} limit");
            }
            log.Info("BOARDFACADE change limit successfully");
            board.ChangeLimit(column, limit);
            if (column == BACKLOG) { board.GetBoardDAO.LIMIT0 = limit; }
            else if (column == INPROGRESS) { board.GetBoardDAO.LIMIT1 = limit; }
            else { board.GetBoardDAO.LIMIT2 = limit;}
        }
        internal int GetColumnLimit(int column, string title, string email)
        {
            log.Info($"BOARDFACADE retrieving Column limit of bard:{title}{email} column {column}");
            if (email == null) {
                log.Error("BOARDFACADE email is null");
                throw new Exception("There is no email"); 
            }
            UserChecking(email);
            if (title == null) {
                log.Error("BOARDFACADE title is null");
                throw new Exception("The title is invalid"); 
            }
            if (!BoardChecking(email, title)) { throw new Exception("User is not a member of the board"); }

            log.Info($"BOARDFACADE returning value {boardsList[title].TaskCounter(column)}");
            return GetBoard(GetBoardID(email, title)).LimitColumn[column];
        }

        internal string GetColumnName(int column, string email, string title) {
            log.Info($"BOARDFACADE tring to return column name of board{title}{email} at the index of {column}");
            if (email == null) {
                log.Error("BOARDFACADE email is null");
                throw new Exception("There is no email"); 
            }

            UserChecking(email);
            if (title == null) {
                log.Error("BOARDFACADE title is null");
                throw new Exception("The title is invalid"); }
            if (!BoardChecking(email, title)) { throw new Exception("User is not a member of the board"); }
            if (column == BACKLOG) {
                log.Info("BOARDFACADE returning backlog");
                return "backlog"; 
            }
            else if (column == INPROGRESS) {
                log.Info("BOARDFACADE returning in progress");
                return "in progress"; 
            }
            else if (column == DONE) {
                log.Info("BOARDFACADE returning done");
                return "done"; 
            }
            else {
                log.Error("BOARDFACADE invalid column number");
                throw new Exception("The column number is invalid"); 
            }
        }
        internal List<int> PresentBoards(string email)
        {
            log.Info($"attempting to return string of all bboards that belong to {email}");
            List<int> output = new List<int>();
            if (email == null) {
                log.Error("email is null");
                throw new Exception("There is no email"); 
            }
            UserChecking(email);
            foreach (BoardBL boardId in boardsList.Values)
            {
                log.Info($"checking if {boardId} belongs to {email}");
                if (boardId.Members.Any(member => member.Email.Equals(email)))
                {

                    log.Info("this board does");
                    output.Add(boardId.GetBoardID());

                }
                log.Info("this board doesnt");
            }
            log.Info($"seccessfully fetched these boards {output}");
            return output;
        }
        internal List<TaskBL> InProgressTasks(string email)
        {
            log.Info($"trying to lost all inprogress tasks for {email}");
            if (email == null) {
                log.Error("email is null");
                throw new Exception("There is no email"); }
            UserChecking(email);
            List<TaskBL> inProgress = new List<TaskBL>();
            foreach (BoardBL boardId in boardsList.Values)
            {
                log.Info($"checking if {boardId} belongs to {email}");
                if (boardId.Members.Any(member => member.Email.Equals(email)))
                {
                    log.Info("this board does");
                    List<TaskBL> find = boardId.tasks;
                    foreach (TaskBL task in find)
                    {
                        log.Info($"checking if a {task.Title} is inprogress");
                        if (task.GetStatus() == INPROGRESS && task.Assignee.Equals(email))
                        {
                            log.Info("this task is");
                            inProgress.Add(task);
                        }
                        log.Info("this task is not");
                    }
                    log.Info("this board doesnt");
                }
            }
            log.Info($"returning this list of tasks i found {inProgress}");
            return inProgress;
        }
        internal List<TaskBL> GetColumn(string email, string boardName, int column)
        {
            UserChecking(email);
            if (!BoardChecking(email, boardName)) { throw new Exception("User is not a member of the board"); }
            if (column != BACKLOG && column != INPROGRESS && column != DONE) { throw new Exception($"Column with {column} index dose not exists. For 'backlog' column enter 0, for 'in progress' column enter 1, for 'done' column enter 2"); }
            List<TaskBL> columnTasks = new List<TaskBL>();
            List<TaskBL> boardTask = GetTasksOfBoard(email, boardName);
            foreach (TaskBL task in boardTask)
            {
                if (task.GetStatus() == column) { columnTasks.Add(task); }
            }
            return columnTasks;
        }
        internal List<TaskBL> GetTasksOfBoard(string email, string title)
        {
            return GetBoard(GetBoardID(email, title)).tasks;
        }
        internal void AddTaskToBoard(int boardID, TaskBL task)
        {
            BoardBL board = GetBoard(boardID);
            board.tasks.Add(task);

        }

        internal int GetTaskCount(string email, string boardName, int column)
        {
            BoardBL board = GetBoard(GetBoardID(email, boardName));
            return board.TaskCounter(column);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email">the email of the user that want to join a board</param>
        /// <param name="title">the title of the board that the user want to join</param>
        /// if the user is not logged in or not registered the function will throw an exception
        internal void Join(string email, int boardID) {
            if(email == null) { throw new Exception("There is no email"); }
            UserChecking(email);
            BoardBL board = GetBoard(boardID);
            if (board == null) { throw new Exception("Board does not exist"); }
            if (IsMember(email, boardID)) { throw new Exception("User is already a member of the board"); }
            string boardTitle = board.Title;
            foreach(BoardBL b in boardsList.Values)
            {
                if (b.Title.Equals(boardTitle))
                {
                    if (IsMember(email, b.BoradID)) { throw new Exception($"you alredy have a board with {boardTitle} name"); }
                }
            }
            board.Members.Add(userFacade.GetUser(email));
            bc.Join(email, boardID);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="email">the email of the user that want to leave a board</param>
        /// <param name="title">the title of the board that the user want to leave</param>
        /// if the user is not logged in or not registered the function will throw an exception
        internal void Leave(string email, int boardID) {
            if (email == null) { throw new Exception("There is no email"); }
            UserChecking(email);
            BoardBL board = GetBoard(boardID);
            if (board == null) { throw new Exception("Board does not exist"); }
            if(!IsMember(email, boardID)) { throw new Exception("User is not a member of the board"); }
            if (IsOwner(email, boardID)) { throw new Exception("Owner can not leave a board"); }
            UserBL user = userFacade.GetUser(email);
            foreach(TaskBL task in board.tasks)
            {
                if (task.Assignee.Equals(user.Email) && task.GetStatus()!=DONE)
                {
                    task.Assignee = null;
                }
            }
            board.Members.Remove(userFacade.GetUser(email));
            bc.Leave(email, boardID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email">the email of the owner of the board</param>
        /// <param name="title">the title of the board that the board's owner change</param>
        /// <param name="newOwnerEmail">the email of the new board's owner</param>
        /// if the user is not logged in or not registered or the user is not the owner of the boardthe function will throw an exception
        internal void NewOwner(string email, string boardTitle, string newOwnerEmail) {
            if (email == null) { throw new Exception("There is no email"); }
            UserChecking(email);
            int boardID = GetBoardID(email, boardTitle);
            BoardBL board = GetBoard(boardID);
            if (board == null) { throw new Exception("Board does not exist"); }
            if (!IsMember(email, boardID)) { throw new Exception("User is not a member of the board"); }
            if (!IsOwner(email, boardID)) { throw new Exception("only owners can transfer board ownership"); }
            if (!IsMember(newOwnerEmail, boardID)) { throw new Exception("The new owner must be a member in the board"); }
            if(email.Equals(newOwnerEmail)) { throw new Exception("The new owner is already the owner of the board"); }
            board.Owner = newOwnerEmail;
            bc.NewOwner(email, boardID);
        }

        internal string GetBoardTitle(int boardID)
        {
            BoardBL board = GetBoard(boardID);
            if(board == null) { throw new Exception("Board does not exist"); }
            return board.Title;
        }
    }
}

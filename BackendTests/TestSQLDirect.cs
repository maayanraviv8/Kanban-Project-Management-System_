using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DAO;

namespace BackendTests
{
    internal class TestSQLDirect
    {
        UserController uc = new UserController();
        BoardController boardc = new BoardController();
        TaskController taskc = new TaskController();


        public void adduser()
        {
            UserDAO sdao = new UserDAO("secondiman@gmail.com", "aaaaaaa");
            sdao.persist();

            UserDAO udao = new UserDAO("thirdof@gmail.com", "bbbbbbbbbbbbb");
            udao.persist();

            UserDAO mdao = new UserDAO("fourthman@gmail.com", "ccccccccccc");
            mdao.persist();


        }

        public void getallusers()
        {
            List<UserDAO> systemusers = uc.GetAllUsers();

            foreach (UserDAO user in systemusers)
                Console.WriteLine(user.Email + " - " + user.Password);
        }

        public void userbyID()
        {
            UserDAO man = uc.GetUserByID("thirdof@gmail.com");
            Console.WriteLine(man.Email + "-" + man.Password);
        }

        /////////BOARD
        public void addboard()
        {
            BoardDAO bd2 = new BoardDAO(222, "bannanas", "secondiman@gmail.com",-1,-1,-1);
            bd2.Persist();
            BoardDAO bd3 = new BoardDAO(333, "kiwis", "secondiman@gmail.com", -1, -1, -1);
            bd3.Persist();

        }


        public void getallboards()
        {
            List<BoardDAO> myboards = boardc.GetAllBoards();
            foreach (BoardDAO boardies in myboards)
                Console.WriteLine($"{boardies.BOARDID} - {boardies._tableName} - {boardies.BOARDOWNER}");
        }


        public void deleteboard()
        {
            BoardDAO bd4 = new BoardDAO(444, "pomelas", "secondiman@gmail.com", -1, -1, -1);
            bd4.Persist();
            getallboards();
            boardc.DeleteBoard(bd4);
            Console.WriteLine("---------");
            getallboards();
        }


        public void updateboard() {
            boardc.UpdateBoard(222, "title", "updatedoranges");
        }


        public void boardsbyuserid() {
            //BoardDAO bd5 = new BoardDAO(555, "naples", "userwithlots@gmail.com", -1, -1, -1);
            //bd5.Persist();
            BoardDAO bd6 = new BoardDAO(666, "milan", "userwithlots@gmail.com", -1, -1, -1);
            bd6.Persist();
            BoardDAO bd7 = new BoardDAO(777, "sicily", "userwithlots@gmail.com", -1, -1, -1);
            bd7.Persist();


            foreach (BoardDAO bd in boardc.GetBoardsByUserID("userwithlots@gmail.com"))
                Console.WriteLine(bd.BOARDID + " - " + bd.BOARDTITLE);

        }


        public void joinboard() {
            boardc.Join("thirdof@gmail.com", 777);
        }

        public void newowner() {
            boardc.NewOwner("thirdof@gmail.com", 777);
        }
        public void leaveboard() {
            boardc.Leave("secondiman@gmail.com", 222);
        }


        ///task
        ///
        public void createtask()
        {
            BoardDAO hastasks = new BoardDAO(910,"randtasks", "thirdof@gmail.com",-1,-1,-1);
            TaskDAO td1 = new TaskDAO(1,"mitlw","take squirrel on a walk",DateTime.Now,DateTime.UtcNow,"shlomo",0);
            td1.persist();
            hastasks.HlepTaskPersist(td1);

            TaskDAO td2 = new TaskDAO(2, "conza", "chop leaves", DateTime.Now, DateTime.UtcNow, "annee", 0);
            td2.persist();
            hastasks.HlepTaskPersist(td2);
        }


        public void updatetask()
        {
            taskc.UpdateTask(2,"description","this is the newest desription good");
        }
        public void getalltasks() {
            List<TaskDAO> allmytasking = taskc.GetAllTasks();
            foreach (TaskDAO ted in allmytasking)
                Console.WriteLine($"{ted.TASKTITLE}-{ted.TASKID}-{ted.TASKDESCRIPTION}");
        }

        public void deletetask() {
            BoardDAO manytasks = new BoardDAO(828, "beach", "thirdof@gmail.com", -1, -1, -1);
            TaskDAO td5 = new TaskDAO(5, "cut watermellon", "use spoon not knife ", DateTime.Now, DateTime.UtcNow, "maxim", 0);
            td5.persist();
            manytasks.HlepTaskPersist(td5);

            TaskDAO td6 = new TaskDAO(6, "boat", "dont forget ore", DateTime.Now, DateTime.UtcNow, "yuvgeni", 0);
            td6.persist();
            manytasks.HlepTaskPersist(td6);

            getalltasks();

            Console.WriteLine("-------------");
            taskc.DeleteTask(td5);

            getalltasks();
        }

        public void deletedata()
        {
            taskc.DeleteAllData();
        }
       
    }
}

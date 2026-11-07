
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using BackendTests;
using IntroSE.Kanban.Backend.ServiceLayer;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TestSQLDirect ts = new TestSQLDirect();
            //ts.adduser();
            //ts.getallusers();
            //ts.userbyID();
            //ts.addboard();
            //ts.getallboards();
            //ts.deleteboard();
            //ts.updateboard();
            //ts.boardsbyuserid();
            //ts.joinboard();
            //ts.newowner();
            //ts.leaveboard();
            //ts.createtask();
            //ts.updatetask();
            //ts.getalltasks();
            //ts.deletedata();
            //ts.deletetask();



            ServiceFactory server = new ServiceFactory();
            UserService us = server.us;

            //System.Console.WriteLine("HERE!");
            //Console.Out.WriteLine(us + "33");


            TestUserRegistration tur = new TestUserRegistration(us);
            tur.registrationruntests();
            tur.loginouttests();


            //BoardService bs = server.bs;
            //TestBoards tb = new TestBoards(bs, us);
            //tb.addeleteboardruntests();


            //TaskService ts = server.ts;
            //TestTasks tt = new TestTasks(ts, us, bs);
            //tt.taskruntests();

        }
    }
}
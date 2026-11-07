using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace BackendTests
{
    internal class TestTasks
    {
        TaskService taskserve;
        UserService userserve;
        BoardService boardi;

        public TestTasks(TaskService ts2, UserService us2, BoardService b2)
        {
            this.taskserve = ts2;
            this.userserve = us2;
            boardi = b2;
        }

        public void taskruntests()
        {
            System.Console.WriteLine(taskserve);
            System.Console.WriteLine(DateTime.Now);
            //adding an empty task
            userserve.Register("miami@silco.com", "38YYTbuniji");
            boardi.CreateBoard("miami@silco.com", "MUFFINS");
            boardi.CreateBoard("miami@silco.com", "FRUIT");
            System.Console.WriteLine(boardi.PresentBoards("miami@silco.com"));
            Response resp = JsonSerializer.Deserialize<Response>(taskserve.CreateTask("miami@silco.com", "task1", "MUFFINS", "chocolat chip", DateTime.Now.AddHours(1)));
            if (resp.isError())
            {
                System.Console.WriteLine("1.FAILED : should be able to add task but cant" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("1.SUCSESSFULL: REGULAR ADDING OF TASK "); }



            Console.WriteLine("----------------------- GET COLUMNS ------------------------------");
            userserve.Register("e8Tub@fkj.com", "tkjgFG5");
            resp = JsonSerializer.Deserialize<Response>(boardi.GetColumn("e8Tub@fkj.com", "132456", 1));
            if (resp.isError())
            {
                System.Console.WriteLine("1.SUCSESSFUL" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("1.FAILED" + resp.ReturnValue); }
            boardi.CreateBoard("e8Tub@fkj.com", "ABCD");
            resp = JsonSerializer.Deserialize<Response>(boardi.GetColumn("e8Tub@fkj.com", "ABCD", 1));
            if (resp.isError())
            {
                System.Console.WriteLine("2.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("2.SUCCESS" + resp.ReturnValue); }


            //getcolumn with tasks in it
            taskserve.CreateTask("e8Tub@fkj.com", "TASK111", "ABCD", "", DateTime.Now.AddHours(42));
            taskserve.CreateTask("e8Tub@fkj.com", "TASK222", "ABCD", "", DateTime.Now.AddHours(43));
            taskserve.CreateTask("e8Tub@fkj.com", "TASK333", "ABCD", "", DateTime.Now.AddHours(44));
            taskserve.CreateTask("e8Tub@fkj.com", "TASK444", "ABCD", "", DateTime.Now.AddHours(45));
            System.Console.WriteLine(boardi.GetColumn("e8Tub@fkj.com", "ABCD", 0));

            resp = JsonSerializer.Deserialize<Response>(boardi.GetColumn("e8Tub@fkj.com", "ABCD", 0));
            if (resp.isError())
            {
                System.Console.WriteLine("2.FAILED" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("2.SUCCESS" + resp.ReturnValue);
            }


            //AdvanceTask - 3

            //success
            resp = JsonSerializer.Deserialize<Response>(taskserve.AdvanceTask("e8Tub@fkj.com", "ABCD", 0));
            if (resp.isError())
            {
                System.Console.WriteLine("3.1.FAILED" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("3.1.SUCCESS" + resp.ReturnValue);
            }

            //seccess
            resp = JsonSerializer.Deserialize<Response>(taskserve.AdvanceTask("e8Tub@fkj.com", "ABCD", 1));
            if (resp.isError())
            {
                System.Console.WriteLine("3.2.FAILED" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("3.2.SUCCESS" + resp.ReturnValue);
            }

            // fail-unregister user
            resp = JsonSerializer.Deserialize<Response>(taskserve.AdvanceTask("notRegisteredUser@walla.co.il", "ABCD", 0));
            if (resp.isError())
            {
                System.Console.WriteLine("3.3.SUCCESS" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("3.3.FAIL" + resp.ReturnValue);
            }


            //seccess
            resp = JsonSerializer.Deserialize<Response>(taskserve.AdvanceTask("e8Tub@fkj.com", "ABCD", 1));
            if (resp.isError())
            {
                System.Console.WriteLine("3.4.FAILED" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("3.4.SUCCESS" + resp.ReturnValue);
            }

            //fail-trying to advance a task that in DONE column
            resp = JsonSerializer.Deserialize<Response>(taskserve.AdvanceTask("e8Tub@fkj.com", "ABCD", 1));
            if (resp.isError())
            {
                System.Console.WriteLine("3.5.SUCCESS" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("3.5.FAIL" + resp.ReturnValue);
            }



            //UpdateTaskDueDate - 4

            // success- valid Update
            resp = JsonSerializer.Deserialize<Response>(taskserve.UpdateTaskDueDate("e8Tub@fkj.com", "ABCD", 0, DateTime.Now.AddHours(45)));
            if (resp.isError())
            {
                System.Console.WriteLine("4.1.FAILED" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("4.1.SUCCESS" + resp.ReturnValue);
            }

            // fail-attempt to change the to an earlier DateTime
            resp = JsonSerializer.Deserialize<Response>(taskserve.UpdateTaskDueDate("e8Tub@fkj.com", "ABCD", 0, DateTime.Now.AddHours(-10)));
            if (resp.isError())
            {
                System.Console.WriteLine("4.2.SUCCESS" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("4.2.FAIL" + resp.ReturnValue);
            }

            // success-no changes
            resp = JsonSerializer.Deserialize<Response>(taskserve.UpdateTaskDueDate("e8Tub@fkj.com", "ABCD", 0, DateTime.Now.AddHours(0)));
            if (resp.isError())
            {
                System.Console.WriteLine("4.3.FAILED" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("4.3.SUCCESS" + resp.ReturnValue);
            }

            // fail-unregister user
            resp = JsonSerializer.Deserialize<Response>(taskserve.UpdateTaskDueDate("notRegisteredUser@walla.co.il", "ABCD", 0, DateTime.Now.AddHours(45)));
            if (resp.isError())
            {
                System.Console.WriteLine("4.4.SUCCESS" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("4.4.FAIL" + resp.ReturnValue);
            }




            //UpdateTaskTitle - 5

            //fail-empty title
            resp = JsonSerializer.Deserialize<Response>(taskserve.UpdateTaskTitle("e8Tub@fkj.com", "ABCD", 0, ""));
            if (resp.isError())
            {
                System.Console.WriteLine("5.1.SUCCESS" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("5.1.FAIL" + resp.ReturnValue);
            }

            //fail-over 50 charachters
            resp = JsonSerializer.Deserialize<Response>(taskserve.UpdateTaskTitle("e8Tub@fkj.com", "ABCD", 0, "Complete the annual financial report for 2023 review"));
            if (resp.isError())
            {
                System.Console.WriteLine("5.2.SUCCESS" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("5.2.FAIL" + resp.ReturnValue);
            }

            // success-valid title
            resp = JsonSerializer.Deserialize<Response>(taskserve.UpdateTaskTitle("e8Tub@fkj.com", "ABCD", 0, "updated valid title"));
            if (resp.isError())
            {
                System.Console.WriteLine("5.3.FAIL" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("5.3.SUCCESS" + resp.ReturnValue);
            }

            // fail-no such board
            resp = JsonSerializer.Deserialize<Response>(taskserve.UpdateTaskTitle("e8Tub@fkj.com", "NoSuchBoard", 0, "failAttempt"));
            if (resp.isError())
            {
                System.Console.WriteLine("5.4.SUCCESS" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("5.4.FAIL" + resp.ReturnValue);
            }

            // fail-unregister user
            resp = JsonSerializer.Deserialize<Response>(taskserve.UpdateTaskTitle("notRegisteredUser", "ABCD", 0, "iCan'tDoThis"));
            if (resp.isError())
            {
                System.Console.WriteLine("5.5.SUCCESS" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("5.5.FAIL" + resp.ReturnValue);
            }



            //UpdateTaskDescription - 6

            // success- description is optional
            resp = JsonSerializer.Deserialize<Response>(taskserve.UpdateTaskDescription("e8Tub@fkj.com", "ABCD", 0, ""));
            if (resp.isError())
            {
                System.Console.WriteLine("6.1.FAIL" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("6.1.SUCCESS" + resp.ReturnValue);
            }

            // fail-over 300 charachters
            resp = JsonSerializer.Deserialize<Response>(taskserve.UpdateTaskDescription("e8Tub@fkj.com", "ABCD", 0, "Develop a comprehensive project plan for the new software development initiative. The plan should include timelines, resource allocation, risk management strategies, and milestones. Ensure to incorporate feedback from all stakeholders and schedule regular review meetings to track progress and address any issues."));
            if (resp.isError())
            {
                System.Console.WriteLine("6.2.SUCCESS" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("6.2.FAIL" + resp.ReturnValue);
            }

            // fail- no such id
            resp = JsonSerializer.Deserialize<Response>(taskserve.UpdateTaskDescription("e8Tub@fkj.com", "ABCD", 888, "hey"));
            if (resp.isError())
            {
                System.Console.WriteLine("6.3.SUCCESS" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("6.3.FAIL" + resp.ReturnValue);
            }


            // Unassigned - 7
            Console.WriteLine("----------------------- Unassigned ------------------------------");

            // Register and create board for unassigned tests
            userserve.Register("user1@example.com", "Password1");
            userserve.Login("user1@example.com", "Password1");
            boardi.CreateBoard("user1@example.com", "Board1");
            taskserve.CreateTask("user1@example.com", "Task1", "Board1", "Description1", DateTime.Now.AddDays(1));

            // Fail - Unregistered user
            resp = JsonSerializer.Deserialize<Response>(taskserve.Unassigned("unregistered@example.com", "Board1", "user1@example.com", 0));
            if (resp.isError())
            {
                Console.WriteLine("7.1 SUCCESS: Unregistered user cannot unassign" + resp.ErrorMessage);
            }
            else
            {
                Console.WriteLine("7.1 FAIL: Unregistered user was able to unassign" + resp.ReturnValue);
            }

            // Register another user for further tests
            userserve.Register("user2@example.com", "Password2");
            userserve.Login("user2@example.com", "Password2");

            // Fail - Registered user but not a board member
<<<<<<< HEAD
            resp = JsonSerializer.Deserialize<Response>(taskserve.Unassigned("user2@example.com", "Board1", "user1@example.com", 0));
=======
            resp = JsonSerializer.Deserialize<Response>(taskserve.Unassigned("user2@example.com", "Board1",  "user1@example.com", 0));
>>>>>>> newSkeleton
            if (resp.isError())
            {
                Console.WriteLine("7.2 SUCCESS: Non-board member cannot unassign" + resp.ErrorMessage);
            }
            else
            {
                Console.WriteLine("7.2 FAIL: Non-board member was able to unassign" + resp.ReturnValue);
            }

            // Add user2 to the board
            boardi.JoinBoard("user2@example.com", "Board1");

            // Fail - Registered user in board but not logged in
            userserve.Logout("user2@example.com");
<<<<<<< HEAD
            resp = JsonSerializer.Deserialize<Response>(taskserve.Unassigned("user2@example.com", "Board1", "user1@example.com", 0));
=======
            resp = JsonSerializer.Deserialize<Response>(taskserve.Unassigned("user2@example.com", "Board1",  "user1@example.com", 0));
>>>>>>> newSkeleton
            if (resp.isError())
            {
                Console.WriteLine("7.3 SUCCESS: Not logged-in user cannot unassign" + resp.ErrorMessage);
            }
            else
            {
                Console.WriteLine("7.3 FAIL: Not logged-in user was able to unassign" + resp.ReturnValue);
            }

            // Success - Registered user in board and logged in
            userserve.Login("user2@example.com", "Password2");
<<<<<<< HEAD
            resp = JsonSerializer.Deserialize<Response>(taskserve.Unassigned("user2@example.com", "Board1", "user1@example.com", 0));
=======
            resp = JsonSerializer.Deserialize<Response>(taskserve.Unassigned("user2@example.com", "Board1",  "user1@example.com", 0));
>>>>>>> newSkeleton
            if (resp.isError())
            {
                Console.WriteLine("7.4 FAIL: Logged-in board member could not unassign" + resp.ErrorMessage);
            }
            else
            {
                Console.WriteLine("7.4 SUCCESS: Logged-in board member successfully unassigned" + resp.ReturnValue);
            }

            // Edge case - Fail - Unassigning a task that doesn't exist
<<<<<<< HEAD
            resp = JsonSerializer.Deserialize<Response>(taskserve.Unassigned("user2@example.com", "Board1", "user1@example.com", 999));
=======
            resp = JsonSerializer.Deserialize<Response>(taskserve.Unassigned("user2@example.com", "Board1",  "user1@example.com", 999));
>>>>>>> newSkeleton
            if (resp.isError())
            {
                Console.WriteLine("7.5 SUCCESS: Unassigning non-existent task failed" + resp.ErrorMessage);
            }
            else
            {
                Console.WriteLine("7.5 FAIL: Unassigning non-existent task succeeded" + resp.ReturnValue);
            }

            // Edge case - Fail - Unassigning by a user who is not the creator or assignee
            taskserve.CreateTask("user1@example.com", "Task2", "Board1", "Description2", DateTime.Now.AddDays(1));
            userserve.Register("user3@example.com", "Password3");
            userserve.Login("user3@example.com", "Password3");
            boardi.Join("user3@example.com", "Board1");
            resp = JsonSerializer.Deserialize<Response>(taskserve.Unassigned("user3@example.com", "Board1", "Task2", "user1@example.com", 1));
            if (resp.isError())
            {
                Console.WriteLine("7.6 SUCCESS: Non-creator/assignee cannot unassign" + resp.ErrorMessage);
            }
            else
            {
                Console.WriteLine("7.6 FAIL: Non-creator/assignee was able to unassign" + resp.ReturnValue);
            }

            // Edge case - Two tasks with very similar details, unassign from the correct one
            taskserve.CreateTask("miami@silco.com", "task2", "MUFFINS", "vanilla chip", DateTime.Now.AddHours(1));
            boardi.Join("anothermember@domain.com", 1);
            taskserve.Unassigned("miami@silco.com", "MUFFINS", 1, "anothermember@domain.com"); // Task 1 is assigned to anothermember
            resp = JsonSerializer.Deserialize<Response>(taskserve.Unassigned("miami@silco.com", "MUFFINS", "task2", "anothermember@domain.com", 1)); // Unassign task 2
            if (resp.isError())
            {
                System.Console.WriteLine("Unassigned 6.FAIL: " + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("Unassigned 6.SUCCESS: " + resp.ReturnValue);
            }

            // Edge case - Successful unassign but do it twice (fail for the second time)
            resp = JsonSerializer.Deserialize<Response>(taskserve.Unassigned("miami@silco.com", "MUFFINS", "task1", "member@domain.com", 0));
            if (resp.isError())
            {
                System.Console.WriteLine("Unassigned 7.1.SUCCESS: " + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("Unassigned 7.1.FAIL: " + resp.ReturnValue);
            }
            resp = JsonSerializer.Deserialize<Response>(taskserve.Unassigned("miami@silco.com", "MUFFINS", "task1", "member@domain.com", 0));
            if (resp.isError())
            {
                System.Console.WriteLine("Unassigned 7.2.SUCCESS: " + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("Unassigned 7.2.FAIL: " + resp.ReturnValue);
            }

            // Edge case - The person that was the assignee left the board
            taskserve.CreateTask("miami@silco.com", "task3", "MUFFINS", "strawberry chip", DateTime.Now.AddHours(1));
            boardi.Join("anothermember2@domain.com", 1);
            taskserve.Unassigned("miami@silco.com", "MUFFINS", "anothermember2@domain.com",2); // Task 2 is assigned to anothermember2
            boardi.Leave("anothermember2@domain.com", 1);
            resp = JsonSerializer.Deserialize<Response>(taskserve.Unassigned("miami@silco.com", "MUFFINS", "task3", "anothermember2@domain.com", 2));
            if (resp.isError())
            {
                System.Console.WriteLine("Unassigned 8.SUCCESS: " + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("Unassigned 8.FAIL: " + resp.ReturnValue);
            }
        }
    }
}

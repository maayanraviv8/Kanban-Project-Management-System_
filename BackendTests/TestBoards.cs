using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace BackendTests
{
    internal class TestBoards
    {
        BoardService boardserve;
        UserService userserve;


        public TestBoards(BoardService bs2, UserService us2)
        {
            this.boardserve = bs2;
            this.userserve = us2;
        }

        public void addeleteboardruntests()
        {
            Console.WriteLine("---------------ADD AND DELET BOARDS----------------------");
            Response resp;
            System.Console.WriteLine(boardserve);

            //regular board creation
            System.Console.WriteLine("##################");
            System.Console.WriteLine(userserve.Register("antwerp@mako.com", "263RRuej"));

            resp = JsonSerializer.Deserialize<Response>(boardserve.CreateBoard("antwerp@mako.com", "TOWERS"));

            if (resp.isError())
            {
                System.Console.WriteLine("1.FAILED : should be able to create board but cant" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("1.SUCSESSFULL: CREATED BOARD TOWERS SUCCESSFULLY"); }

            //deleting board that doesnt exist
            resp = JsonSerializer.Deserialize<Response>(boardserve.DeleteBoard("antwerp@mako.com", "NOTABOARD"));
            if (!resp.isError())
            {
                System.Console.WriteLine("2.FAILED : deleted a board that does not exist");
            }
            else { System.Console.WriteLine("2.SUCSESSFULL: returned error for board that does not exist" + resp.ErrorMessage); }

            //regular deletion of board
            resp = JsonSerializer.Deserialize<Response>(boardserve.DeleteBoard("antwerp@mako.com", "TOWERS"));
            if (resp.isError())
            {
                System.Console.WriteLine("3.FAILED : could not delete board" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("3.SUCSESSFULL: deleted board towers"); }

            //creating board with same name
            boardserve.CreateBoard("antwerp@mako.com", "MINERALS");//first creation
            resp = JsonSerializer.Deserialize<Response>(boardserve.CreateBoard("antwerp@mako.com", "MINERALS"));//second creation attempt

            if (!resp.isError())
            {
                System.Console.WriteLine("4.FAILED :created board with the same name for the same user!");
            }
            else { System.Console.WriteLine("4.SUCSESSFULL: did not allow same boardname for same user" + resp.ErrorMessage); }

            //logging in user2
            userserve.Register("madrid@mako.com", "ROLOering9999");

            //same boardname but different user
            resp = JsonSerializer.Deserialize<Response>(boardserve.CreateBoard("madrid@mako.com", "MINERALS"));

            if (resp.isError())
            {
                System.Console.WriteLine("5.FAILED :a different user tried creating a board with a name in the system but wasnt allowed" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("5.SUCSESSFULL: 2 different users can have the same board name"); }

            //6
            userserve.Logout("madrid@mako.com");
            resp = JsonSerializer.Deserialize<Response>(boardserve.CreateBoard("madrid@mako.com", "MINERALS"));

            if (resp.isError())
            {
                System.Console.WriteLine("6.SUCSESSFUL  :user is logged out- cannot create a board" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("6.FAILED: the user is logged out- should be regected"); }

            //7
            userserve.Login("madrid@mako.com", "ROLOering9999");
            resp = JsonSerializer.Deserialize<Response>(boardserve.CreateBoard("madrid@mako.com", "#GOOD_5*"));
            if (!resp.isError())
            {
                System.Console.WriteLine("7.SUCSESSFUL  :user is  create a board");
            }
            else { System.Console.WriteLine("7.FAILED: the user cannot create a board" + resp.ErrorMessage); }

            //8 delete board that doesnt exists
            resp = JsonSerializer.Deserialize<Response>(boardserve.DeleteBoard("antwerp@mako.com", "DKGJ"));
            if (!resp.isError())
            {
                System.Console.WriteLine("8.FAILED : could not delete board" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("8.SUCSESSFULL: deleted board towers"); }

            //add the same board that deleted

            resp = JsonSerializer.Deserialize<Response>(boardserve.CreateBoard("antwerp@mako.com", "TOWERS"));
            if (resp.isError())
            {
                System.Console.WriteLine("9.FAILED : could not add board" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("9.SUCSESSFULL: add board towers"); }

            //10
            userserve.Logout("antwerp@mako.com");
            resp = JsonSerializer.Deserialize<Response>(boardserve.CreateBoard("antwerp@mako.com", "ABCDE"));
            if (resp.isError())
            {
                System.Console.WriteLine("10.SUCSESSFULL : could not create board-user is logged out" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("10.FAILED: create board allthough the user is logged out"); }



            Console.WriteLine("---------------LIMIT COLUMN AND GET COLUMN LIMIT----------------------");

            //limiting board column
            resp = JsonSerializer.Deserialize<Response>(boardserve.LimitColumn("madrid@mako.com", "MINERALS", 1, 7));
            if (resp.isError())
            {
                System.Console.WriteLine("1.FAILED :cant limit column 1 to value 7 " + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("1.SUCSESSFULL: managed to limit column 1 to 7 tasks for board MINERALS of user madrid@mako.com"); }

            //changing the same simit
            resp = JsonSerializer.Deserialize<Response>(boardserve.LimitColumn("madrid@mako.com", "MINERALS", 1, 4));
            if (resp.isError())
            {
                System.Console.WriteLine("2.FAILED :cant edit limit of column in board twice " + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("2.SUCSESSFULL: managed to EDIT limit column 1 from 7 to 4 tasks for board MINERALS of user madrid@mako.com"); }

            resp = JsonSerializer.Deserialize<Response>(boardserve.GetColumnLimit("madrid@mako.com", "MINERALS", 1));
            if (resp.isError())
            {
                System.Console.WriteLine("3.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("3.SUCSESSFULL" + resp.ReturnValue); }

            resp = JsonSerializer.Deserialize<Response>(boardserve.LimitColumn("madrid@mako.com", "MINERALS", 4, 4));
            if (!resp.isError())
            {
                System.Console.WriteLine("4.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("4.SUCSESSFULL" + resp.ReturnValue); }

            boardserve.CreateBoard("madrid@mako.com", "mgmFKJ0");
            resp = JsonSerializer.Deserialize<Response>(boardserve.GetColumnLimit("madrid@mako.com", "mgmFKJ0", 0));
            if (resp.isError())
            {
                System.Console.WriteLine("5.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("5.SUCSESSFULL" + resp.ReturnValue); }

            resp = JsonSerializer.Deserialize<Response>(boardserve.GetColumnName(0, "madrid@mako.com", "mgmFKJ0"));
            if (resp.isError())
            {
                System.Console.WriteLine("6.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("6.SUCSESSFULL" + resp.ReturnValue); }

            resp = JsonSerializer.Deserialize<Response>(boardserve.GetColumnName(1, "madrid@mako.com", "mgmFKJ0"));
            if (resp.isError())
            {
                System.Console.WriteLine("7.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("7.SUCSESSFULL" + resp.ReturnValue); }

            resp = JsonSerializer.Deserialize<Response>(boardserve.GetColumnName(2, "madrid@mako.com", "mgmFKJ0"));
            if (resp.isError())
            {
                System.Console.WriteLine("8.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("8.SUCSESSFULL" + resp.ReturnValue); }

            resp = JsonSerializer.Deserialize<Response>(boardserve.GetColumnName(5, "madrid@mako.com", "mgmFKJ0"));
            if (!resp.isError())
            {
                System.Console.WriteLine("9.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("9.SUCSESSFULL" + resp.ReturnValue); }

            boardserve.LimitColumn("madrid@mako.com", "555", 0, 15);
            resp = JsonSerializer.Deserialize<Response>(boardserve.GetColumnName(0, "madrid@mako.com", "555"));
            if (!resp.isError())
            {
                System.Console.WriteLine("9.5.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("9.5.SUCSESSFULL" + resp.ReturnValue); }


            userserve.Logout("madrid@mako.com");
            resp = JsonSerializer.Deserialize<Response>(boardserve.LimitColumn("madrid@mako.com", "MINERALS", 1, 4));
            if (!resp.isError())
            {
                System.Console.WriteLine("10.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("10.SUCSESSFULL"); }

            resp = JsonSerializer.Deserialize<Response>(boardserve.GetColumnName(0, "madrid@mako.com", "mgmFKJ0"));
            if (!resp.isError())
            {
                System.Console.WriteLine("11.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("11.SUCSESSFULL" + resp.ReturnValue); }


            Console.WriteLine("---------------GET COLUMN AND IN PROGRESS----------------------");
            //Console.WriteLine(JsonSerializer.Deserialize<Response>(JsonSerializer.Serialize(new Response("WNFDSONFOIMFPIOSDNVIOSDNFOISNDOFI",new List<String>() ))));
            userserve.Register("asd@uyr.ui", "fieyuJGJ^56");
            boardserve.CreateBoard("asd@uyr.ui", "123456");
            Console.WriteLine(boardserve.PresentBoards("asd@uyr.ui"));
            resp = JsonSerializer.Deserialize<Response>(boardserve.GetColumn("asd@uyr.ui", "123456", 5));
            if (!resp.isError())
            {
                System.Console.WriteLine("1.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("1.SUCSESSFULL" + resp.ReturnValue); }

            Console.WriteLine(boardserve.GetColumn("asd@uyr.ui", "123456", 2));
            
            resp = JsonSerializer.Deserialize<Response>(boardserve.GetColumn("asd@uyr.ui", "123456", 2));
            
            if (resp.isError())
            {
                System.Console.WriteLine("2.FAILED" + resp);
            }
            else { System.Console.WriteLine("2.SUCSESSFULL  " + resp); }
            
            resp = JsonSerializer.Deserialize<Response>(boardserve.GetColumn("asd@uyr.ui", "123456", 1));
            if (resp.isError())
            {
                System.Console.WriteLine("3.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("3.SUCSESSFULL" + resp.ReturnValue); }

            resp = JsonSerializer.Deserialize<Response>(boardserve.GetColumn("asd@uyr.ui", "1326", 1));
            if (!resp.isError())
            {
                System.Console.WriteLine("4.FAILED" + resp.ReturnValue);
            }
            else { System.Console.WriteLine("4.SUCSESSFULL" + resp.ErrorMessage); }

            resp = JsonSerializer.Deserialize<Response>(boardserve.InProgressTasks("asd@uyr.ui"));
            if (resp.isError())
            {
                System.Console.WriteLine("5.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("5.SUCSESSFULL" + resp.ReturnValue); }

            boardserve.CreateBoard("asd@uyr.ui", "7894");
            resp = JsonSerializer.Deserialize<Response>(boardserve.InProgressTasks("asd@uyr.ui"));
            if (resp.isError())
            {
                System.Console.WriteLine("6.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("6.SUCSESSFULL" + resp.ReturnValue); }

            boardserve.DeleteBoard("asd@uyr.ui", "123456");
            resp = JsonSerializer.Deserialize<Response>(boardserve.InProgressTasks("asd@uyr.ui"));
            if (resp.isError())
            {
                System.Console.WriteLine("7.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("7.SUCSESSFULL" + resp.ReturnValue); }

            resp = JsonSerializer.Deserialize<Response>(boardserve.GetColumn("asd@uyr.ui", "132456", 1));
            if (!resp.isError())
            {
                System.Console.WriteLine("8.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("8.SUCSESSFULL" + resp.ReturnValue); }

            boardserve.CreateBoard("asd@uyr.ui", "7894");
            userserve.Logout("asd@uyr.ui");
            resp = JsonSerializer.Deserialize<Response>(boardserve.GetColumn("asd@uyr.ui", "132456", 1));
            if (!resp.isError())
            {
                System.Console.WriteLine("9.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("9.SUCSESSFULL" + resp.ReturnValue); }

            resp = JsonSerializer.Deserialize<Response>(boardserve.InProgressTasks("asd@uyr.ui"));
            if (!resp.isError())
            {
                System.Console.WriteLine("10.FAILED" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("10.SUCSESSFULL" + resp.ReturnValue); }


            Console.WriteLine("----------------------- PRESENT BOARDS ------------------------------");
            userserve.Register("ekfub@fkj.com", "tkjgFG5");
            boardserve.CreateBoard("ekfub@fkj.com", "123");
            boardserve.CreateBoard("ekfub@fkj.com", "456");
            boardserve.CreateBoard("ekfub@fkj.com", "789");
            Console.WriteLine(boardserve.PresentBoards("ekfub@fkj.com"));

        }

        public void Join_Leave_boardruntests()
        {
            Response resp;
            Console.WriteLine("---------------JOIN AND LEAVE BOARD----------------------");
            boardserve.CreateBoard("madrid@mako.com", "ANOTHERBOARD");

            //1 joining a board while logged out
            userserve.Logout("antwerp@mako.com");
            resp = JsonSerializer.Deserialize<Response>(boardserve.Join("antwerp@mako.com", 1));
            if (!resp.isError())
            {
                Console.WriteLine("1.FAILED: Join a board although user is logged out");
            }
            else { Console.WriteLine("1.SUCCESFUL: did not join a board - user is logged out" + resp.ErrorMessage); }

            //2 Join a board while logged in
            userserve.Login("antwerp@mako.com", "263RRuej");
            resp = JsonSerializer.Deserialize<Response>(boardserve.Join("antwerp@mako.com", 1));
            if (resp.isError())
            {
                Console.WriteLine("2.FAILED: could not join board " + resp.ErrorMessage);
            }
            else { Console.WriteLine("2.SUCCESFUL: joined board!"); }

            //3 try joining a board already in
            resp = JsonSerializer.Deserialize<Response>(boardserve.Join("antwerp@mako.com", 1));
            if (!resp.isError())
            {
                Console.WriteLine("3.FAILED: joined a board twice");
            }
            else { Console.WriteLine("3.SUCCESFUL: cannot join a board twice " + resp.ErrorMessage); }

            //4 Join a board that doesn't exist
            boardserve.DeleteBoard("madrid@mako.com", "ANOTHERBOARD");
            resp = JsonSerializer.Deserialize<Response>(boardserve.Join("madrid@mako.com", 1));
            if (!resp.isError())
            {
                Console.WriteLine("4.FAILED: joined a board that doesnt exist");
            }
            else { Console.WriteLine("4.SUCCESFUL: cannot join a board that doesnt exist " + resp.ErrorMessage); }


            //5 Join a board that was recreated
            boardserve.CreateBoard("madrid@mako.com", "ANOTHERBOARD");

            resp = JsonSerializer.Deserialize<Response>(boardserve.Join("antwerp@mako.com",1));
            if (resp.isError())
            {
                Console.WriteLine("5.FAILED: could not join board " + resp.ErrorMessage);
            }
            else { Console.WriteLine("5.SUCCESFUL: joined board!"); }


            //leaving Board

            //trying to leave a board that not joined to
            resp = JsonSerializer.Deserialize<Response>(boardserve.Leave("antwerp@mako.com",1));
            if (!resp.isError())
            {
                Console.WriteLine("6.FAILED: left a board not in");
            }
            else { Console.WriteLine("6.SUCCESFUL: cannot leave a board not in " + resp.ErrorMessage); }

            //7  Leave a board
            resp = JsonSerializer.Deserialize<Response>(boardserve.Leave("antwerp@mako.com",1));
            if (resp.isError())
            {
                Console.WriteLine("7.FAILED: cannot leave a board " + resp.ErrorMessage);
            }
            else { Console.WriteLine("7.SUCCESFUL: left a board! "); }

            //8  Leave a board again
            resp = JsonSerializer.Deserialize<Response>(boardserve.Leave("antwerp@mako.com",1));
            if (!resp.isError())
            {
                Console.WriteLine("8.FAILED: left a board twice");
            }
            else { Console.WriteLine("8.SUCCESFUL: cannot leave a board twice " + resp.ErrorMessage); }

        }
        public void TransferOwnershipTests()
        {
            Response resp;
            Console.WriteLine("###############################");
            Console.WriteLine("###### NEW OWNER TESTS#########");
            Console.WriteLine("###############################");

            // 1 Join the new user to the board
            userserve.Register("mutser@guetta.com", "873abJBGGGG");
            resp = JsonSerializer.Deserialize<Response>(boardserve.Join("mutser@guetta.com", 1));
            if (resp.isError())
            {
                Console.WriteLine("1.FAILED: could not join board " + resp.ErrorMessage);
            }
            else { Console.WriteLine("1.SUCCESFUL: joined board!"); }

            // 2 Transfer ownership to a non member
            resp = JsonSerializer.Deserialize<Response>(boardserve.NewOwner("mutser@guetta.com", "MINERALS", "ANOTHERBOARD"));
            if (!resp.isError())
            {
                Console.WriteLine("2.FAILED: transferred ownership to a non-member of the board");
            }
            else { Console.WriteLine("2.SUCCESFUL: cannot transfer ownership to non-member " + resp.ErrorMessage); }

            // 3 Transfer ownership to a member
            resp = JsonSerializer.Deserialize<Response>(boardserve.NewOwner("mutser@guetta.com", "MINERALS", "madrid@mako.com"));
            if (resp.isError())
            {
                Console.WriteLine("3.FAILED: cannot transfer ownership to member" + resp.ErrorMessage);
            }
            else { Console.WriteLine("3.SUCCESFUL: transferred ownership to a member of the board! "); }

            // 4 Transfer ownership to a logged out member
            userserve.Logout("madrid@mako.com");
            resp = JsonSerializer.Deserialize<Response>(boardserve.NewOwner("mutser@guetta.com", "MINERALS", "ANOTHERBOARD"));
            if (!resp.isError())
            {
                Console.WriteLine("4.FAILED: transferred ownership to a logged-out member of the board");
            }
            else { Console.WriteLine("4.SUCCESFUL: cannot transfer ownership to logged-out member of board " + resp.ErrorMessage); }

            // 5 Transfer ownership while logged in
            userserve.Login("madrid@mako.com", "ROLOering9999");
            resp = JsonSerializer.Deserialize<Response>(boardserve.NewOwner("madrid@mako.com", "MINERALS", "mutser@guetta.com"));
            if (resp.isError())
            {
                Console.WriteLine("5.FAILED: cannot transfer ownership to member" + resp.ErrorMessage);
            }
            else { Console.WriteLine("5.SUCCESFUL: transferred ownership to a member of the board! "); }

            // 6 Transfer ownership twice
            resp = JsonSerializer.Deserialize<Response>(boardserve.NewOwner("madrid@mako.com", "MINERALS", "mutser@guetta.com"));
            if (!resp.isError())
            {
                Console.WriteLine("6.FAILED: transferred ownership twice");
            }
            else { Console.WriteLine("6.SUCCESFUL: cannot transfer ownership twice " + resp.ErrorMessage); }
        }
    }
}

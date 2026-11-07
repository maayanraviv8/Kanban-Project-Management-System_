using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend;
using System.Text.Json;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.BusinessLayer;



namespace BackendTests
{
    public class TestUserRegistration
    {
        UserService userservice;
        

        public TestUserRegistration(UserService userservice2)
        {
            this.userservice = userservice2;
        }

        public void registrationruntests()
        {
            System.Console.WriteLine(userservice);
            System.Console.WriteLine(userservice.Register("dfdfdfdfdfdfdfdfdf@gmail.com", ", vjhk3515HBHB"));

            //regular registration
            Response resp = JsonSerializer.Deserialize<Response>(userservice.Register("wendo@gmail.com", "AAklomububo46"));
            if (resp.isError())
            {
                System.Console.WriteLine("1.FAILED : should be able to register this user but cant" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("1.SUCSESSFULL: REGULAR REGISTRATION"); }

            //2
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("yuuu4yy@gmail.com", "15Pumn877"));
            if (resp.isError())
            {
                System.Console.WriteLine("2.FAILED : should be able to register this user but cant" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("2.SUCSESSFULL: REGISTERED");
            }

            //3
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("yu@g.cm", "qwert567J"));
            if (resp.isError())
            {
                System.Console.WriteLine("3.FAILED : should be able to register this user but cant" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("3.SUCSESSFULL: REGISTERED");
            }

            //4
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("yu@ga.m", "qwert567J"));
            if (resp.isError())
            {
                System.Console.WriteLine("4.SUCSESSFULL : invalid email" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("4.FAILED: should be rejected because the email is invalid");
            }

            //5
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("u@gamail.il", "qwert567"));
            if (resp.isError())
            {
                System.Console.WriteLine("5.SUCSESSFULL : invalid password" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("5.FAILED: should be rejected because the password is invalid");
            }

            //6
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("u@gamail.il", "qT6"));
            if (resp.isError())
            {
                System.Console.WriteLine("6.SUCSESSFULL : invalid password" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("6.FAILED: should be rejected because the password is invalid");
            }

            //7
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("yu@g.cm", "qwert567"));
            if (resp.isError())
            {
                System.Console.WriteLine("7.SUCSESSFULL : already registered" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("7.FAILED: should be rejected because the email is exists");
            }

            resp = JsonSerializer.Deserialize<Response>(userservice.Register("wendo@gmail.com", "499WANkjfkdjl"));
            if (!resp.isError())
            {
                System.Console.WriteLine("2.FAILED : this user exists but managed to register");
            }
            else
            {
                System.Console.WriteLine("2.SUCSESSFULL: REJECTED REGISTRATION OF USER THAT EXISTS");
            }


            //empty password - expecting an error
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("mario@mako.com", ""));
            if (!resp.isError())
            {
                System.Console.WriteLine("3.FAILED : this user managed to register with empty password");
            }
            else { System.Console.WriteLine("3.SUCSESSFULL: REJECTED REGISTRATION WITH EMPTY PASSWORD" + resp.ErrorMessage); }

            //empty email - expecting an error
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("", "2RowlR2"));
            if (!resp.isError())
            {
                System.Console.WriteLine("4.FAILED : this user managed to register with empty email");
            }
            else
            {
                System.Console.WriteLine("4.SUCSESSFULL: REJECTED REGISTRATION OF USER WITH EMPTY EMAIL" + resp.ErrorMessage);
            }


            //invalid email but good password
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("ihavenoshtrudel.com", "URT33ksdfn"));
            if (!resp.isError())
            {
                System.Console.WriteLine("5.FAILED : this user signed in with invalid email");
            }
            else { System.Console.WriteLine("5.SUCSESSFULL REJECTED USER WITH UNVALID EMAIL" + resp.ErrorMessage); }

            //password with too few letrers
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("goodemail@yahoo.com", "1Ab"));
            if (!resp.isError())
            { System.Console.WriteLine("6.FAILED: only has 3 letters in password but still registered"); }
            else { System.Console.WriteLine("6. SUCCESS: rejected registration of 3 letter password" + resp.ErrorMessage); }

            //password with too many characters
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("goodemail111@yahoo.com", "1Abhjfn348jnfd;kjnknsdjhfsdjhfksjdhfjaniup"));
            if (!resp.isError())
            { System.Console.WriteLine("7.FAILED: has like 50 letters in password but still registered"); }
            else { System.Console.WriteLine("7. SUCCESS: rejected registration of 50 letter password" + resp.ErrorMessage); }


            //all spaces
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("    ", "    "));
            if (!resp.isError())
            { System.Console.WriteLine("8.FAILED: allowed email that is only spaces to sign up"); }
            else { System.Console.WriteLine("8. SUCCESS: rejected email that is spaces to sign up" + resp.ErrorMessage); }


            //no uppercase
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("relo@outlook.com", "23u8fjnf88"));
            if (!resp.isError())
            { System.Console.WriteLine("9.FAILED: allowed password that has no uppercase to sign up"); }
            else { System.Console.WriteLine("9. SUCCESS: rejected password that has no uppercase" + resp.ErrorMessage); }

            //no lowercase
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("nendo@outlook.com", "AEUNVI3636363"));
            if (!resp.isError())
            { System.Console.WriteLine("10.FAILED: allowed password that has no lowercase to sign up"); }
            else { System.Console.WriteLine("10. SUCCESS: rejected password that has no lowercase to sign up" + resp.ErrorMessage); }


            //no number
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("nendo@outlook.com", "AEUNVIkndk_$$$"));
            if (!resp.isError())
            { System.Console.WriteLine("11.FAILED: allowed password that has no number to sign up - special keys check"); }
            else { System.Console.WriteLine("11. SUCCESS: rejected password that is spaces to sign up" + resp.ErrorMessage); }



            //different email
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("nendo@bgu.ac.il", "ERTJNJnkdnf7989"));
            if (resp.isError())
            { System.Console.WriteLine("12.FAILED: email is in format of ___@bgu.ac.il and didnt allow registration" + resp.ErrorMessage); }
            else { System.Console.WriteLine("12. SUCCESS: allwed registration of mail with complicated format"); }

            Console.WriteLine("---------------NEW REGISTER TESTS----------------------");
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("test.email+regex@gmail.com", "AAklomububo46"));
            if (resp.isError())
            {
                System.Console.WriteLine("1.FAILED : should be able to register this user but cant" + resp.ErrorMessage);
            }
            else { System.Console.WriteLine("1.SUCSESSFULL: REGULAR REGISTRATION"); }

            //2
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("user.name@sub.domain.com", "15Pumn877"));
            if (resp.isError())
            {
                System.Console.WriteLine("2.FAILED : should be able to register this user but cant" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("2.SUCSESSFULL: REGISTERED");
                
            }

            //3
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("email@domain.co.uk", "qwert567J"));
            if (resp.isError())
            {
                System.Console.WriteLine("3.FAILED : should be able to register this user but cant" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("3.SUCSESSFULL: REGISTERED");
            }

            //3.5
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("email@domain.co.uk", "qwert567J"));
            if (resp.isError())
            {
                System.Console.WriteLine("3.5.SUCCESSFUL : user is already registered" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("3.5.FAILED: user is already registered- should be rejected");
            }

            //4
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("plainaddress", "qwert567J"));
            if (resp.isError())
            {
                System.Console.WriteLine("4.SUCSESSFULL : invalid email" + resp.ErrorMessage);
                System.Console.WriteLine(JsonSerializer.Serialize(resp));
            }
            else
            {
                System.Console.WriteLine("4.FAILED: should be rejected because the email is invalid");
            }

            //5
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("@missingusername.com", "qwertPP567"));
            if (resp.isError())
            {
                System.Console.WriteLine("5.SUCSESSFULL : invalid email" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("5.FAILED: should be rejected because the email is invalid");
            }

            //6
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("username@.com", "qTtyee86"));
            if (resp.isError())
            {
                System.Console.WriteLine("6.SUCSESSFULL : invalid email" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("6.FAILED: should be rejected because the email is invalid");
            }

            //7
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("username@domain", "qweUYrt567"));
            if (resp.isError())
            {
                System.Console.WriteLine("7.SUCSESSFULL : invalid email" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("7.FAILED: should be rejected the email is invalid");
            }

            //8
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("u@d.co", "qwert567"));
            if (resp.isError())
            {
                System.Console.WriteLine("8.SUCSESSFULL : invalid email" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("8.FAILED: should be rejected the email is invalid");
            }

            //8.5
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("user@domain with space.com", "qwert567"));
            if (resp.isError())
            {
                System.Console.WriteLine("8.5.SUCSESSFULL : invalid email" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("8.5.FAILED: should be rejected the email is invalid");
            }

            //9
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("user@domain!#$%&'*+/=?^_{}|~.com", "qwerTTt567"));
            if (resp.isError())
            {
                System.Console.WriteLine("9.SUCSESSFULL : invalid email" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("9.FAILED: should be rejected the email is invalid");
            }

            //10
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("uriu6@gmail.co", "Password1"));
            if (resp.isError())
            {
                System.Console.WriteLine("10.FAILED : valid password" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("10.SUCSESSFULL: registered");
            }

            //10.5
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("uri7u6@gmail.co", "P8ssw0rd1"));
            if (resp.isError())
            {
                System.Console.WriteLine("10.5.FAILED : valid password" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("10.5.SUCSESSFULL: registered");
            }

            //11
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("ur5iu6@gmail.co", "Passw0rdWithManyCharacters123"));
            if (resp.isError())
            {
                System.Console.WriteLine("11.SUCCESSFUL : invalid password" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("11.FAILED: invalid password");
            }

            //12
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("u3riu6@gmail.co", "pass"));
            if (resp.isError())
            {
                System.Console.WriteLine("12.SUCSESSFULL : invalid password" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("12.FAILED: should be rejected the password is invalid");
            }

            //13
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("ur65iu6@gmail.co", "P@ssw0rd123456789012345"));
            if (resp.isError())
            {
                System.Console.WriteLine("13.SUCSESSFULL : invalid password" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("13.FAILED: should be rejected the password is invalid");
            }

            //14
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("ur88iu6@gmail.co", "PASSWORD"));
            if (resp.isError())
            {
                System.Console.WriteLine("14.SUCSESSFULL : invalid password" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("14.FAILED: should be rejected the password is invalid");
            }

            //15
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("uriu456@gmail.co", "Passw 1"));
            if (resp.isError())
            {
                System.Console.WriteLine("15.FAILED: should be rejected the password is invalid" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("15.SUCSESSFUL: registered");
            }

            //16
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("uru456@gmail.co", "P1aB2c"));
            if (resp.isError())
            {
                System.Console.WriteLine("16.FAILED : valid password" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("16.SUCSESSFULL: registered");
            }

            //17
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("iu456@gmail.co", "cD3eF4gH5iJ6kL7mN8op"));
            if (resp.isError())
            {
                System.Console.WriteLine("17.FAILED : valid password" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("17.SUCSESSFULL: registered");
            }
            //18
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("", "cD3eF4gH5iJ6kL7mN8op"));
            if (resp.isError())
            {
                System.Console.WriteLine("18.SUCSESSFULL : no email" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("18.FAILED: SHOULD BE REGECTED- THERE IS NO EMAIL");
            }

            //19
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("iu456@gmail.co", ""));
            if (resp.isError())
            {
                System.Console.WriteLine("19.SUCSESSFULL : no PASSWORD" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("19.FAILED: SHOULD BE REGECTED- THERE IS NO PASSWORD");
            }
            Console.WriteLine("---------------NEW TESTS----------------------");
            //20
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("iu4tytt56@gmail.co", "uutieLK878@"));
            if (resp.isError())
            {
                System.Console.WriteLine("20.FAILED : register regected" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("20.SUCSESSFULL: registered");

            }

            //21
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("i6@gmail.co", "uTTut^ie8/78*"));
            if (resp.isError())
            {
                System.Console.WriteLine("21.FAILED : register regected" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("21.SUCSESSFULL: registered");

            }

            //22
            resp = JsonSerializer.Deserialize<Response>(userservice.Register("tt56@gmail.com", "uutאר#ERie=8__78-"));
            if (resp.isError())
            {
                System.Console.WriteLine("22.FAILED : register regected" + resp.ErrorMessage);
            }
            else
            {
                System.Console.WriteLine("22.SUCSESSFULL: registered");

            }
        }


            public void loginouttests()
        {
            Console.WriteLine("---------------LGIN AND OUT TESTS----------------------");
            Response resp;

            ////regular login
            //userservice.Register("alto@gmail.com","WWON68jhjhj");
            //resp = JsonSerializer.Deserialize<Response>(userservice.Login("alto@gmail.com", "WWON68jhjhj"));
            //if (resp.isError())
            //{ System.Console.WriteLine("1.SUCCESS: the user is logged in" + resp.ErrorMessage); }
            //else { System.Console.WriteLine("1. FAILED: the user is logged in should be rejected"); }
            
            ////loging in user that doesnt exist
            //resp = JsonSerializer.Deserialize<Response>(userservice.Login("alto@gmail56789765789.com", "WWON68jhjhj"));
            //if (!resp.isError())
            //{ System.Console.WriteLine("2.FAILED:logged in user that doesnt exist" ); }
            //else { System.Console.WriteLine("2. SUCCESS: blocked user that doesnt exist:: " + resp.ErrorMessage); }


            ////loging out and in
            //resp = JsonSerializer.Deserialize<Response>(userservice.Logout("alto@gmail.com"));
            //if (resp.isError())
            //{ System.Console.WriteLine("3.FAILED: didnt manage to logout user that is logged in" + resp.ErrorMessage); }
            //else { System.Console.WriteLine("3. SUCCESS: logged user out successfully "); }
            //resp = JsonSerializer.Deserialize<Response>(userservice.Login("alto@gmail.com", "WWON68jhjhj"));
            //if (resp.isError())
            //{ System.Console.WriteLine("4.FAILED: didnt manage to log user back in" + resp.ErrorMessage); }
            //else { System.Console.WriteLine("4. SUCCESS: user can log in multiple times"); }

            ////logout a user that doesnt exist
            //resp = JsonSerializer.Deserialize<Response>(userservice.Logout("jojoba1212@gmail.com"));
            //if (!resp.isError())
            //{ System.Console.WriteLine("5.FAILED: logged user out that is NOT logged in" ); }
            //else { System.Console.WriteLine("5. SUCCESS: didnt log user out because he was never logged in! : " + resp.ErrorMessage); }

            ////logout a user that is logged out
            //userservice.Register("arty@gmail.com", "QWIYbufn74837");
            //userservice.Logout("arty@gmail.com");//first logout
            //resp = JsonSerializer.Deserialize<Response>(userservice.Logout("arty@gmail.com"));//second logout
            //if (!resp.isError())
            //{ System.Console.WriteLine("6.FAILED: logged out a user that is logged out and didnt throw an error"); }
            //else { System.Console.WriteLine("6. SUCCESS: didnt log user out because he wasalready logged out! : " + resp.ErrorMessage); }

            //userservice.Register("ar56ty@gmail.com", "QWIYbufn*74837");
            //userservice.Logout("arty@gmail.com");//first logout
            //resp = JsonSerializer.Deserialize<Response>(userservice.Login("ar56ty@gmail.com", "QWIYbufn*74839"));//second logout
            //if (resp.isError())
            //{ System.Console.WriteLine("7.SUCCESS: email is wrong"); }
            //else { System.Console.WriteLine("7. FAILED: the password is worng. should be rejected" + resp.ErrorMessage); }

            //resp = JsonSerializer.Deserialize<Response>(userservice.Login("rtew@gmail.com", "QWIYfgbd 39"));//second logout
            //if (resp.isError())
            //{ System.Console.WriteLine("8.SUCCESS: user is not registered"); }
            //else { System.Console.WriteLine("8. FAILED: the user is not register. should be rejected" + resp.ErrorMessage); }

            ////login with wrong register
            //userservice.Register("fhiq@fkh.com", "565flkFD*-S5");
            //userservice.Logout("fhiq@fkh.com");
            //resp = JsonSerializer.Deserialize<Response>(userservice.Login("fhiq@fkh.com", "565flkFD*-s"));
            //if (resp.isError())
            //{ System.Console.WriteLine("9.SUCCESS: password is wrong"); }
            //else { System.Console.WriteLine("9. FAILED: the password is worng. should be rejected" + resp.ErrorMessage); }


            //Valid Registration and Login
            userservice.Register("valid.email@example.com", "Valid123");
             resp = JsonSerializer.Deserialize<Response>(userservice.Login("valid.email@example.com", "Valid123"));
            if (!resp.isError())
            {
                Console.WriteLine("1. SUCCESS: The user is logged in.");
            }
            else
            {
                Console.WriteLine("1. FAILED: The user should be logged in, but got error: " + resp.ErrorMessage);
            }
            userservice.Logout("valid.email@example.com");
            resp = JsonSerializer.Deserialize<Response>(userservice.Login("valid.email@example.com", "Valid123"));
            if (!resp.isError())
            {
                Console.WriteLine("1. SUCCESS: The user is logged in.");
            }
            else
            {
                Console.WriteLine("1. FAILED: The user should be logged in, but got error: " + resp.ErrorMessage);
            }

            ////Invalid Email Format:
            //resp = JsonSerializer.Deserialize<Response>(userservice.Register("invalid-email.com", "Valid123"));
            //if (resp.isError())
            //{
            //    Console.WriteLine("2. SUCCESS: Invalid email format was rejected.");
            //}
            //else
            //{
            //    Console.WriteLine("2. FAILED: Invalid email format should be rejected.");
            //}

            ////Invalid Password Format (Too Short):
            //resp = JsonSerializer.Deserialize<Response>(userservice.Register("valid.email@example.com", "V1a"));
            //if (resp.isError())
            //{
            //    Console.WriteLine("3. SUCCESS: Password too short was rejected.");
            //}
            //else
            //{
            //    Console.WriteLine("3. FAILED: Password too short should be rejected.");
            //}

            ////Invalid Password Format (No Uppercase Letter):
            //resp = JsonSerializer.Deserialize<Response>(userservice.Register("valid.email@example.com", "valid123"));
            //if (resp.isError())
            //{
            //    Console.WriteLine("4. SUCCESS: Password with no uppercase letter was rejected.");
            //}
            //else
            //{
            //    Console.WriteLine("4. FAILED: Password with no uppercase letter should be rejected.");
            //}

            ////Invalid Password Format (No Lowercase Letter):
            //resp = JsonSerializer.Deserialize<Response>(userservice.Register("valid.email@example.com", "VALID123"));
            //if (resp.isError())
            //{
            //    Console.WriteLine("5. SUCCESS: Password with no lowercase letter was rejected." +resp.ReturnValue);
            //}
            //else
            //{
            //    Console.WriteLine("5. FAILED: Password with no lowercase letter should be rejected.");
            //}

            ////Invalid Password Format (No Number):
            //resp = JsonSerializer.Deserialize<Response>(userservice.Register("valid.email@example.com", "ValidPassword"));
            //if (resp.isError())
            //{
            //    Console.WriteLine("6. SUCCESS: Password with no number was rejected." + resp.ReturnValue);
            //}
            //else
            //{
            //    Console.WriteLine("6. FAILED: Password with no number should be rejected.");
            //}

            ////Non-Existent User Login Attempt:
            //resp = JsonSerializer.Deserialize<Response>(userservice.Login("nonexistent@example.com", "SomePassword1"));
            //if (resp.isError())
            //{
            //    Console.WriteLine("7. SUCCESS: Login attempt with non-existent user was rejected." + resp.ReturnValue);
            //}
            //else
            //{
            //    Console.WriteLine("7. FAILED: Login attempt with non-existent user should be rejected.");
            //}

            ////Incorrect Password:
            //userservice.Register("test.user@example.com", "CorrectPassword1");
            //resp = JsonSerializer.Deserialize<Response>(userservice.Login("test.user@example.com", "WrongPassword1"));
            //if (resp.isError())
            //{
            //    Console.WriteLine("8. SUCCESS: Login attempt with incorrect password was rejected." + resp.ReturnValue);
            //}
            //else
            //{
            //    Console.WriteLine("8. FAILED: Login attempt with incorrect password should be rejected.");
            //}

            ////Case Sensitivity in Email:
            //userservice.Register("case.sensitive@example.com", "Valid123");
            //resp = JsonSerializer.Deserialize<Response>(userservice.Login("Case.Sensitive@example.com", "Valid123"));
            //if (resp.isError())
            //{
            //    Console.WriteLine("9. SUCCESS: Login attempt with case-sensitive email was rejected." + resp.ReturnValue);
            //}
            //else
            //{
            //    Console.WriteLine("9. FAILED: Login attempt with case-sensitive email should be rejected.");
            //}

            ////Password Length Edge Cases:
            //// Password of exactly 6 characters
            //userservice.Register("edge.case6@example.com", "Aa1aaa");
            //resp = JsonSerializer.Deserialize<Response>(userservice.Login("edge.case6@example.com", "Aa1aaa"));
            //if (!resp.isError())
            //{
            //    Console.WriteLine("10a. SUCCESS: Login with 6-character password succeeded." + resp.ReturnValue);
            //}
            //else
            //{
            //    Console.WriteLine("10a. FAILED: Login with 6-character password should succeed, but got error: " + resp.ErrorMessage);
            //}

            //// Password of exactly 20 characters
            //userservice.Register("edge.case20@example.com", "Aa1" + new string('a', 17));
            //resp = JsonSerializer.Deserialize<Response>(userservice.Login("edge.case20@example.com", "Aa1" + new string('a', 17)));
            //if (!resp.isError())
            //{
            //    Console.WriteLine("10b. SUCCESS: Login with 20-character password succeeded." + resp.ReturnValue);
            //}
            //else
            //{
            //    Console.WriteLine("10b. FAILED: Login with 20-character password should succeed, but got error: " );
            //}

            //// Password longer than 20 characters
            //resp = JsonSerializer.Deserialize<Response>(userservice.Register("edge.case21@example.com", "Aa1" + new string('a', 18)));
            //if (resp.isError())
            //{
            //    Console.WriteLine("10c. SUCCESS: Password longer than 20 characters was rejected." + resp.ReturnValue);
            //}
            //else
            //{
            //    Console.WriteLine("10c. FAILED: Password longer than 20 characters should be rejected.");
            //}


        }
    }
}

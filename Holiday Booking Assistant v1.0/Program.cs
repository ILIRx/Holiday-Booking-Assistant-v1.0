using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Data.SQLite;
using System.Threading;

namespace NEA_Project
{
    class Program
    {
        #region HolidayFinder
        static void HolidayFinder()
        {
            List<Holiday> ListOfHolidays = new List<Holiday>();
            ListOfHolidays = GenerateHolidays(ListOfHolidays);


            int count = 0;
            Console.CursorVisible = true;
            string[] questions =
            {
                "How much do you enjoy being in the tropics?: ",
                "How much do you enjoy being in a city?: ",
                "How much do you enjoy exploring new areas?: ",
                "How much do you enjoy nightlife?: ",
                "How much do you enjoy being in a hot climate?: "
            };

            List<double> userValues = new List<double>();

            int userScore = 0;
            Console.WriteLine("On a scale of (1 - 10): ");

            for (int i = 0; i < questions.Length; i++)
            {
                Console.Write(questions[i]);
                int choice = int.Parse(Console.ReadLine());
                userValues.Add(choice);
                userScore += choice;
            }

            Console.WriteLine("Final Question: What is your budget?");
            Console.Write("£");
            double budget = double.Parse(Console.ReadLine());

            Console.Clear();

            Console.WriteLine($"Your score is: {userScore}\n");

            count = 0;

            List<Holiday> SuitableHolidays = new List<Holiday>();
            Dictionary<Holiday, double> SuitabilityOfHoliday = new Dictionary<Holiday, double>();

            Console.WriteLine($"Your choices were: \n > Tropical Level: {userValues[0]},\n > Urban Level: {userValues[1]},\n > Adventure Level: {userValues[2]},\n > Nightlife Level: {userValues[3]},\n > Temperature Level: {userValues[4]}\n");

            Console.WriteLine("!!Remember a holiday's Unique Holiday ID and inform an agent to save holiday for future reference!!");

            Console.WriteLine("Your suitable holidays are: ");



            foreach (Holiday item in ListOfHolidays)
            {
                List<double> HolidayValues = new List<double>();
                HolidayValues.Add(item.getTropical());
                HolidayValues.Add(item.getUrban());
                HolidayValues.Add(item.getAdventure());
                HolidayValues.Add(item.getNightlife());
                HolidayValues.Add(item.getTemperature());

                double angle = getAngleOfVectors(HolidayValues, userValues);

                if (angle < 20 && item.getPricePP() <= budget + budget * 0.125)
                {
                    SuitableHolidays.Add(item);
                    SuitabilityOfHoliday.Add(item, angle);
                }
            }

            //Taken from stackoverflow! 
            var sortedDict = from entry in SuitabilityOfHoliday orderby entry.Value descending select entry;
            //No longer taken from stackoverflow

            bool breakOut = false;

            foreach (KeyValuePair<Holiday, double> kvp in SuitabilityOfHoliday)
            {
                Console.WriteLine($"{count + 1}:\n > Holiday Name = {kvp.Key.getName()}\n > Tropical Level = {kvp.Key.getTropical()}\n > Urban Level = {kvp.Key.getUrban()}\n > Adventure Level = {kvp.Key.getAdventure()}\n > Nightlife Level = {kvp.Key.getNightlife()}\n > Temperature Level = {kvp.Key.getTemperature()}\n   > £{kvp.Key.getPricePP()}pp, ");

                Console.Write($"   > Unique Holiday ID: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(kvp.Key.getHID());
                Console.ResetColor();

                count++;
                if (breakOut == false)
                {
                    Console.WriteLine($"This is your most suitable holiday! This is because for example, on a scale of 1 - 10, you gave Tropical Holidays a {userValues[0]}/10, and the holiday displayed has a value of {kvp.Key.getTropical()}");
                    Console.WriteLine($"\nWould you like to be shown the rest of your suitable holidays ({SuitableHolidays.Count} Holidays found!) ? (Y/N)");
                    char choice = char.Parse(Console.ReadLine());
                    switch (choice.ToString().ToUpper())
                    {
                        case "Y":
                            breakOut = true;
                            break;
                        case "N":
                            Console.WriteLine("(Press any key!)");
                            Console.ReadKey(true);
                            break;
                        default:
                            break;
                    }
                }
            }
            Console.ReadKey(true);
            Console.CursorVisible = false;
        }
        #endregion

        #region GenHolidays
        static List<Holiday> GenerateHolidays(List<Holiday> ListOfHolidays)
        {
            //Generates Holidays w/ Random attributes and Name and adds them to database
            //If database already has holidays, retrieve all data and create holiday objects that can be modified during the process
            SQLiteCommand sql = new SQLiteCommand($"SELECT * FROM holidays", conn);
            SQLiteDataReader check = sql.ExecuteReader();

            if (!check.HasRows)
            {
                Console.WriteLine("Generate Holidays (Press any key)");
                Console.ReadKey(true);
                sql = new SQLiteCommand($"DELETE FROM holidays", conn);
                sql.ExecuteNonQuery();
                sql = new SQLiteCommand($"UPDATE sqlite_sequence SET seq = 0 WHERE `name` = 'userInfo'", conn);
                sql.ExecuteNonQuery();

                for (int i = 0; i < 1000; i++)
                {
                    ListOfHolidays.Add(new Holiday());
                    sql = new SQLiteCommand($"INSERT INTO holidays(destination, departureDate, returnDate, transportationType, price, tropicalLevel, urbanLevel, adventureLevel, nightlifeLevel, temperatureLevel) VALUES ('{ListOfHolidays[i].getName()}','{ListOfHolidays[i].getDepartureDate()}','{ListOfHolidays[i].getReturnDate()}','{ListOfHolidays[i].getTransportType()}','{ListOfHolidays[i].getPricePP()}','{ListOfHolidays[i].getTropical()}','{ListOfHolidays[i].getUrban()}','{ListOfHolidays[i].getAdventure()}','{ListOfHolidays[i].getNightlife()}','{ListOfHolidays[i].getTemperature()}')", conn);
                    Thread.Sleep(10);
                    sql.ExecuteNonQuery();
                    Console.WriteLine(ListOfHolidays[i].getName());
                }
                Thread.Sleep(600);

                Console.WriteLine("Done! (Press any key)");
                Console.ReadKey(true);

                Console.Clear();

                return ListOfHolidays;
            }
            else
            {
                Console.WriteLine("Retrieving Holidays from Database... (Estimated Time Taken: 5 - 10 seconds)");
                double tropical, urban, adventure, nightlife, temperature, price, sumOfAttributes;
                int HID;
                string holidayName, departureDate, returnDate, transportationType;
                sql = new SQLiteCommand($"SELECT * FROM holidays", conn);
                SQLiteDataReader reader = sql.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.RecordsAffected >= -1)
                    {
                        while (reader.Read())
                        {
                            tropical = (int)reader["tropicalLevel"];
                            urban = (int)reader["urbanLevel"];
                            adventure = (int)reader["adventureLevel"];
                            nightlife = (int)reader["nightlifeLevel"];
                            temperature = (int)reader["temperatureLevel"];
                            price = (int)reader["price"];
                            sumOfAttributes = tropical + urban + adventure + nightlife + tropical;

                            holidayName = (string)reader["destination"];
                            departureDate = (string)reader["departureDate"];
                            returnDate = (string)reader["returnDate"];
                            transportationType = (string)reader["transportationType"];
                            HID = Convert.ToInt16(reader["HID"]);

                            ListOfHolidays.Add(new Holiday(HID, tropical, urban, adventure, nightlife, temperature, price, sumOfAttributes, holidayName, departureDate, returnDate, transportationType));
                        }
                    }
                }
                Console.WriteLine("Done! (Press any key)");
                Console.ReadKey(true);

                Console.Clear();
                return ListOfHolidays;
            }

        }
        #endregion

        #region AngleOfVectors
        static double getAngleOfVectors(List<double> x, List<double> y)
        {
            double dotProduct = (x[0] * y[0] + x[1] * y[1] + x[2] * y[2] + x[3] * y[3] + x[4] * y[4]);
            double magnitude1 = Math.Sqrt(Math.Pow(x[0], 2) + Math.Pow(x[1], 2) + Math.Pow(x[2], 2) + Math.Pow(x[3], 2) + Math.Pow(x[4], 2));
            double magnitude2 = Math.Sqrt(Math.Pow(y[0], 2) + Math.Pow(y[1], 2) + Math.Pow(y[2], 2) + Math.Pow(y[3], 2) + Math.Pow(y[4], 2));
            double cosTh = (dotProduct) / (magnitude1 * magnitude2);
            double angle = Math.Acos(cosTh);

            Math.Round(angle, 3);

            angle = angle * (180 / Math.PI);

            return angle;
        }
        #endregion

        static bool checkIfValidLogin(string usernameIN, string passwordIN)
        {
            int key = HashFunction(passwordIN, usernameIN);

            SQLiteCommand sql = new SQLiteCommand($"SELECT * FROM userInfo WHERE userInfo.userName = '{usernameIN}'", conn);

            SQLiteDataReader userCheck = sql.ExecuteReader();
            if (userCheck.HasRows)
            {
                if (userCheck.RecordsAffected >= -1)
                {
                    while (userCheck.Read())
                    {
                        if ((int)userCheck["passKey"] == key)
                        {
                            if ((bool)userCheck["isAgent"] == true)
                            {
                                currentUser = new AgentUser(usernameIN, passwordIN);
                                currentUser.setIsAgent(true);
                            }
                            else
                            {
                                currentUser = new CustomerUser(usernameIN, passwordIN);
                                currentUser.setIsAgent(false);
                            }
                            return true;
                        }
                        else
                        {
                            Console.CursorTop = 5;
                            Console.WriteLine("\nUsername or Password entered incorrectly.");

                            Console.WriteLine("\nPress Any Key...");

                            Console.ReadKey(true);

                            return false;
                        }
                    }
                }

            }

            Console.Clear();

            Console.WriteLine("No users found in database");
            Console.WriteLine("\nPress Any Key...");

            Console.ReadKey(true);

             return false;
        }
        static UserInfo createNewUser()
        {
            UserInfo newUser = new UserInfo("", "");

            Console.Title = "Create New Account";

            int choices = 2;

            Console.WriteLine("\n New Customer");
            Console.WriteLine(" New Agent");

            Console.CursorLeft = 0;
            Console.CursorTop = 1;
            Console.Write(">");
            bool valid = false;
            int option = 1;

            do
            {
                ConsoleKeyInfo choice = Console.ReadKey(true);
                if (choice.Key == ConsoleKey.UpArrow && option - 1 > 0)
                {
                    Console.CursorTop = option;
                    Console.CursorLeft = 0;
                    Console.WriteLine(" ");
                    option--;
                    Console.CursorTop = option;
                    Console.CursorLeft = 0;
                    Console.Write(">");
                }
                else if (choice.Key == ConsoleKey.DownArrow && option + 1 <= choices)
                {
                    Console.CursorTop = option;
                    Console.CursorLeft = 0;
                    Console.WriteLine(" ");
                    option++;
                    Console.CursorTop = option;
                    Console.CursorLeft = 0;
                    Console.Write(">");
                }
                else if (choice.Key == ConsoleKey.Enter)
                {
                    switch (option)
                    {
                        case 1:
                            newUser = new CustomerUser("", "");
                            valid = true;
                            break;
                        case 2:
                            newUser = new AgentUser("", "");
                            //File with automatically generated key gets created
                            //If I were to publish this program I would use something like JS to send an email to the administrator of the company,
                            //and then the administrator would give this key to the agent
                            valid = true;
                            break;
                    }
                }

            } while (valid != true);

            Console.Clear();
            Console.CursorVisible = true;
            if (newUser is AgentUser)
            {
                Console.WriteLine("In order to create an agent account, you must first enter your verification key sent to you by your administrator");
                Console.WriteLine("\nPlease enter your verification key: ");

                for (int i = 5; i > 0; i--)
                {
                    Console.CursorTop = 5;
                    Console.WriteLine($"You have {i} tries left.");
                    Console.CursorTop = 3;
                    string inputKey = Console.ReadLine();
                    if (newUser.getAgentKey() == inputKey)
                    {
                        Console.WriteLine("Verified Succesfully. (Press any key)");
                        Console.ReadKey(true);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("                                                                       ");
                        Console.CursorTop = 3;
                        inputKey = Console.ReadLine();
                    }
                }
            }
            //File gets deleted to reduce storage use
            File.Delete("agentKey.txt");

            Console.Clear();

            Console.Write("Set a Username ( < 20 characters ): ");
            newUser.setUserName(Console.ReadLine());

            Console.Write("Set a Password ( < 20 characters ): ");
            string password = Console.ReadLine();

            int key = HashFunction(password, newUser.getUserName());

            newUser.setPassword(password);
            newUser.setPassKey(key);

            //User is entered into the database
            if (newUser is CustomerUser)
            {
                SQLiteCommand sql = new SQLiteCommand($"INSERT INTO userInfo(userName, passKey, isAgent) VALUES ('{newUser.getUserName()}', '{newUser.getPasswordKey()}', FALSE)", conn);
                sql.ExecuteNonQuery();
            }
            else if (newUser is AgentUser)
            {
                SQLiteCommand sql = new SQLiteCommand($"INSERT INTO userInfo(userName, passKey, isAgent) VALUES ('{newUser.getUserName()}', '{newUser.getPasswordKey()}', TRUE)", conn);
                sql.ExecuteNonQuery();
            }

            Console.CursorVisible = false;

            Console.Clear();

            return newUser;
        }
        static int HashFunction(string s, string userName)
        {
            int total = 0;
            char[] c;
            c = s.ToCharArray();

            for (int i = 0; i <= c.GetUpperBound(0); i++)
            {
                total += (int)c[i];
            }

            return total % s.Length + (userName.Length + 1);
        }
        static int UIMovement(int choices)
        {
            Console.CursorLeft = 0;
            Console.CursorTop = 1;
            Console.Write(">");
            bool valid = false;
            int option = 1;
            int returnChoice = 0;

            do
            {
                ConsoleKeyInfo choice = Console.ReadKey(true);
                if (choice.Key == ConsoleKey.UpArrow && option - 1 > 0)
                {
                    Console.CursorTop = option;
                    Console.CursorLeft = 0;
                    Console.WriteLine(" ");
                    option--;
                    Console.CursorTop = option;
                    Console.CursorLeft = 0;
                    Console.Write(">");
                }
                else if (choice.Key == ConsoleKey.DownArrow && option + 1 <= choices)
                {
                    Console.CursorTop = option;
                    Console.CursorLeft = 0;
                    Console.WriteLine(" ");
                    option++;
                    Console.CursorTop = option;
                    Console.CursorLeft = 0;
                    Console.Write(">");
                }
                else if (choice.Key == ConsoleKey.Enter)
                {
                    returnChoice = option;
                    valid = true;
                }
            } while (valid != true);

            return returnChoice;
        }
        static bool LoginScreen(int choices)
        {
            string usernameIN = "", passwordIN = "";

            Console.WriteLine("Login Or Create a new Account");
            Console.WriteLine($" Username: ");
            Console.WriteLine($" Password: ");
            Console.WriteLine(" Create new account");

            Console.CursorLeft = 0;
            Console.CursorTop = 1;
            Console.Write(">");
            bool valid = false;
            bool usernameEntered = false, passwordEntered = false;
            int option = 1;

            do
            {
                ConsoleKeyInfo choice = Console.ReadKey(true);
                if (choice.Key == ConsoleKey.UpArrow && option - 1 > 0)
                {
                    Console.CursorTop = option;
                    Console.CursorLeft = 0;
                    Console.WriteLine(" ");
                    option--;
                    Console.CursorTop = option;
                    Console.CursorLeft = 0;
                    Console.Write(">");
                }
                else if (choice.Key == ConsoleKey.DownArrow && option + 1 <= choices)
                {
                    Console.CursorTop = option;
                    Console.CursorLeft = 0;
                    Console.WriteLine(" ");
                    option++;
                    Console.CursorTop = option;
                    Console.CursorLeft = 0;
                    Console.Write(">");
                }
                else if (choice.Key == ConsoleKey.Enter)
                {
                    switch (option)
                    {
                        case 1:
                            Console.CursorVisible = true;
                            Console.CursorLeft = 11;
                            Console.CursorTop = option;
                            Console.Write("                    ");
                            Console.CursorLeft = 11;
                            Console.CursorTop = option;
                            usernameIN = Console.ReadLine();
                            Console.CursorVisible = false;
                            usernameEntered = true;
                            break;
                        case 2:
                            Console.CursorVisible = true;
                            Console.CursorLeft = 11;
                            Console.CursorTop = option;
                            Console.Write("                    ");
                            Console.CursorLeft = 11;
                            Console.CursorTop = option;
                            passwordIN = Console.ReadLine();
                            Console.CursorVisible = false;
                            passwordEntered = true;
                            break;
                        case 3:
                            Console.Clear();
                            valid = true;
                            break;
                        default:
                            break;
                    }
                }
                if (passwordEntered == true && usernameEntered == true) { valid = true; }

            } while (valid != true);

            if (passwordEntered == true && usernameEntered == true)
            {
                return checkIfValidLogin(usernameIN, passwordIN);
            }
            else
            {
                currentUser = createNewUser();
                return false;
            }
        }
        static string[] contactDetailsUI()
        {
            string address, postCode, phoneNum, IDC;
            string[] contactDetails = new string[4];
            Console.CursorLeft = 0;
            Console.CursorTop = 1;
            Console.Write(">");
            bool valid = false;
            bool addressEntered = false, postCodeEntered = false, phoneNumberEntered = false, IDCEntered = false;
            int option = 1;
            do
            {
                ConsoleKeyInfo choice = Console.ReadKey(true);
                if (choice.Key == ConsoleKey.UpArrow && option - 1 > 0)
                {
                    Console.CursorTop = option;
                    Console.CursorLeft = 0;
                    Console.WriteLine(" ");
                    option--;
                    Console.CursorTop = option;
                    Console.CursorLeft = 0;
                    Console.Write(">");
                }
                else if (choice.Key == ConsoleKey.DownArrow && option + 1 <= 4)
                {
                    Console.CursorTop = option;
                    Console.CursorLeft = 0;
                    Console.WriteLine(" ");
                    option++;
                    Console.CursorTop = option;
                    Console.CursorLeft = 0;
                    Console.Write(">");
                }
                else if (choice.Key == ConsoleKey.Enter)
                {
                    switch (option)
                    {
                        case 1:
                            Console.CursorVisible = true;
                            Console.CursorLeft = 21;
                            Console.CursorTop = option;
                            Console.Write("                                      ");
                            Console.CursorLeft = 21;
                            Console.CursorTop = option;
                            address = Console.ReadLine();
                            contactDetails[0] = address;
                            Console.CursorVisible = false;
                            addressEntered = true;
                            break;
                        case 2:
                            Console.CursorVisible = true;
                            Console.CursorLeft = 23;
                            Console.CursorTop = option;
                            Console.Write("                                      ");
                            Console.CursorLeft = 23;
                            Console.CursorTop = option;
                            postCode = Console.ReadLine();
                            contactDetails[1] = postCode;
                            Console.CursorVisible = false;
                            postCodeEntered = true;
                            break;
                        case 3:
                            Console.CursorVisible = true;
                            Console.CursorLeft = 26;
                            Console.CursorTop = option;
                            Console.Write("                                      ");
                            Console.CursorLeft = 26;
                            Console.CursorTop = option;
                            phoneNum = Console.ReadLine();
                            contactDetails[2] = phoneNum;
                            Console.CursorVisible = false;
                            phoneNumberEntered = true;
                            break;
                        case 4:
                            Console.CursorVisible = true;
                            Console.CursorLeft = 41;
                            Console.CursorTop = option;
                            Console.Write("                                      ");
                            Console.CursorLeft = 41;
                            Console.CursorTop = option;
                            IDC = Console.ReadLine();
                            contactDetails[3] = IDC;
                            Console.CursorVisible = false;
                            IDCEntered = true;
                            break;
                        default:
                            break;
                    }
                }
                if (addressEntered == true && postCodeEntered == true && phoneNumberEntered == true && IDCEntered == true) { valid = true; }

            } while (valid != true);
            return contactDetails;
        }
        static void Customer()
        {
            Console.Clear();
            Console.WriteLine($"Welcome {currentUser.getUserName()}! Here are your options:");
            Console.WriteLine(" Find my perfect holiday");
            Console.WriteLine(" Show my favourite holidays");
            Console.WriteLine(" Log Out");
            int choice = UIMovement(3);

            switch (choice)
            {
                case 1:
                    Console.Clear();
                    HolidayFinder();
                    break;
                case 2:
                    Console.Clear();
                    ShowFavHolidays(currentUser.getUserName());
                    break;
                case 3:
                    Console.Clear();
                    Console.WriteLine("Goodbye!");

                    Thread.Sleep(3000);
                    Environment.Exit(0);
                    break;
            }
            Customer();
        }
        static void ShowFavHolidays(string userName)
        {
            SQLiteCommand sql;
            sql = new SQLiteCommand($"SELECT userName FROM userInfo WHERE userInfo.userName = '{userName}'", conn);
            SQLiteDataReader user = sql.ExecuteReader();
            while (user.Read())
            {
                userName = user["userName"].ToString();
            }
            int count = 1;
            Console.WriteLine($"Here are your saved/favourite holidays {userName}:\n");
            sql = new SQLiteCommand($"SELECT destination, departureDate, returnDate, transportationType, price FROM holidays, favHolidays, userInfo WHERE holidays.HID = favHolidays.HID AND userInfo.userName = favHolidays.userName AND userInfo.userName = '{userName}' ORDER BY holidays.destination DESC", conn);
            SQLiteDataReader holidays = sql.ExecuteReader();
            if (holidays.HasRows)
            {
                if (holidays.RecordsAffected >= -1)
                {
                    while (holidays.Read())
                    {
                        Console.WriteLine($"{count}) Destination: {(string)holidays["destination"]}");
                        Console.WriteLine($"  > Departure Date: {(string)holidays["departureDate"]}");
                        Console.WriteLine($"  > Return Date: {(string)holidays["returnDate"]}");
                        Console.WriteLine($"  > Transportation Type: {(string)holidays["transportationType"]}");
                        Console.WriteLine($"    > £{holidays["price"].ToString()}pp\n");
                        count++;
                    }

                }

            }
            Console.WriteLine("Press Any Key To Go Back...");
            Console.ReadKey(true);
        }
        static string validateCustomerUserName(string userName)
        {
            bool valid = false;

            SQLiteCommand sql = new SQLiteCommand($"SELECT userName FROM userInfo WHERE userName = '{userName}' ", conn);

            SQLiteDataReader userCheck = sql.ExecuteReader();
            while (valid == false)
            {
                if (userCheck.HasRows)
                {
                    if (userCheck.RecordsAffected >= -1)
                    {
                        while (userCheck.Read())
                        {
                            if (userCheck["userName"].ToString() == userName)
                            {
                                valid = true;
                            }
                        }
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Username entered incorrectly.");

                    Console.Write("\nPlease re-enter customer's Username:                                            ");
                    Console.CursorLeft = 37;
                    userName = Console.ReadLine();

                    sql = new SQLiteCommand($"SELECT userName FROM userInfo WHERE userName = '{userName}' ", conn);

                    userCheck = sql.ExecuteReader();
                }
            }
            return userName;
        }
        static void addContactDetails(string userName)
        {
            validateCustomerUserName(userName);
            Console.Clear();
            Console.WriteLine("Add a Customer's Contact details: ");
            Console.WriteLine(" Enter your address: ");
            Console.WriteLine(" Enter your post code: ");
            Console.WriteLine(" Enter your phone number: ");
            Console.WriteLine(" Enter your International Dialling Code: ");

            string[] contactDetails = contactDetailsUI();

            SQLiteCommand sql = new SQLiteCommand($"INSERT INTO contactDetails(address, postCode, phoneNumber, IDC) VALUES ('{contactDetails[0]}','{contactDetails[1]}','{contactDetails[2]}','{contactDetails[3]}')", conn);
            sql.ExecuteNonQuery();
            sql = new SQLiteCommand($"INSERT INTO residence(address, userName) VALUES ('{contactDetails[0]}','{userName}')", conn);
            sql.ExecuteNonQuery();
            Console.WriteLine("\nDone! Press any key...");
            Console.ReadKey(true);
        }
        static void ShowContactDetails(string userName)
        {
            validateCustomerUserName(userName);
            SQLiteCommand sql;
            sql = new SQLiteCommand($"SELECT userName FROM userInfo WHERE userInfo.userName = '{userName}'", conn);
            SQLiteDataReader user = sql.ExecuteReader();
            while (user.Read())
            {
                userName = user["userName"].ToString();
            }
            int count = 1;
            Console.WriteLine($"Here are {userName}'s contact details:\n");
            sql = new SQLiteCommand($"SELECT contactDetails.address, contactDetails.postCode, contactDetails.phoneNumber, contactDetails.IDC FROM contactDetails, userInfo, residence WHERE contactDetails.address = residence.address AND residence.userName = userInfo.userName", conn);
            SQLiteDataReader contactDetails = sql.ExecuteReader();
            if (contactDetails.HasRows)
            {
                if (contactDetails.RecordsAffected >= -1)
                {
                    while (contactDetails.Read())
                    {
                        string phoneNumber = (string)contactDetails["IDC"] + (string)contactDetails["phoneNumber"];
                        Console.WriteLine($"{count})");
                        Console.WriteLine($"  > Address: {(string)contactDetails["address"]}");
                        Console.WriteLine($"  > Post Code: {(string)contactDetails["postCode"]}");
                        Console.WriteLine($"  > Phone Number: {phoneNumber}");
                        count++;
                    }

                }

            }
            Console.WriteLine("\nPress Any Key To Go Back...");
            Console.ReadKey(true);
        }
        static void setFavouriteHoliday(string userName)
        {
            validateCustomerUserName(userName);
            int HID = 0;

            Console.CursorVisible = true;

            Console.Write($"Enter Unique Holiday ID to add holiday to {userName}'s saved holidays\nHID: ");
            Console.ForegroundColor = ConsoleColor.Green;
            HID = int.Parse(Console.ReadLine());
            Console.ResetColor();

            Console.CursorVisible = false;

            SQLiteCommand sql = new SQLiteCommand($"INSERT INTO favHolidays(userName, HID) VALUES ('{userName}', {HID})", conn);
            sql.ExecuteNonQuery();

            Console.WriteLine("\nPress Any Key To Go Back...");
            Console.ReadKey(true);
        }
        static void Agent()
        {
            string customername = "";
            Console.Clear();
            Console.WriteLine($"Welcome Agent {currentUser.getUserName()}! Here are your options:");
            Console.WriteLine(" Add a Customer's Contact Details");
            Console.WriteLine(" Set a Customer's favourite holiday");
            Console.WriteLine(" Show a Customer's Contact Details");
            Console.WriteLine(" Show a Customer's favourite holidays");
            Console.WriteLine(" Log Out");
            int choice = UIMovement(5);

            switch (choice)
            {
                case 1:
                    Console.Clear();
                    Console.WriteLine("Enter customer's Username: ");
                    customername = Console.ReadLine();
                    addContactDetails(customername);
                    break;
                case 2:
                    Console.Clear();
                    Console.WriteLine("Enter customer's Username: ");
                    customername = Console.ReadLine();
                    setFavouriteHoliday(customername);
                    break;
                case 3:
                    Console.Clear();
                    Console.WriteLine("Enter customer's Username: ");
                    customername = Console.ReadLine();
                    ShowContactDetails(customername);
                    break;
                case 4:
                    Console.Clear();
                    Console.WriteLine("Enter customer's Username: ");
                    customername = Console.ReadLine();
                    ShowFavHolidays(customername);
                    break;
                case 5:
                    Console.Clear();
                    Console.WriteLine("Goodbye!");

                    Thread.Sleep(3000);
                    Environment.Exit(0);
                    break;
            }
            Agent();
        }
        public static UserInfo currentUser;
        public static SQLiteConnection conn = new SQLiteConnection("Data Source=userInfo.db;Version=3;New=True;Compress=True;");
        static void Main(string[] args)
        {
            List<string> userInfo = new List<string>();
            int choices = 3;


            conn.Open();

            bool cont = false;

            Console.CursorVisible = false;

            Console.Title = "Login";
            while (cont == false)
            {
                cont = LoginScreen(choices);
                Console.Clear();
            }
            if (currentUser.getIsAgent() == true)
            {
                Agent();
            }
            else if (currentUser.getIsAgent() == false)
            {
                Customer();
            }

            Console.ReadKey(true);

        }
    }
}

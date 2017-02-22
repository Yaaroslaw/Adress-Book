using System;
using AddressBook;
using Logger;
using System.Linq;
using System.Net.Mail;
using System.Net;

namespace Program
{
    //Class for testing
    //Subscriber of AddressBook
    class Program
    {
        Logger.Logger logger;
        ConsoleLogger consoleLogger;
        FileLogger fileLogger;

        public Program(AddressBook.AddressBook addressBook)
        {
            consoleLogger = new ConsoleLogger();
            fileLogger = new FileLogger();
            logger = new Logger.Logger(consoleLogger);

            addressBook.UserAdded += HandleEvent;
            addressBook.UserRemoved += HandleEvent;
            addressBook.DebugEvent += HandleEvent;
        }

        void HandleEvent(object o, ModifiedEventArgs e)
        {
            switch(e.Type)
            {
                case EventTypes.Info:
                    {
                        logger.SetLogger(consoleLogger);
                        logger.Info(e.Message);
                        logger.SetLogger(fileLogger);
                        logger.Info(e.Message);
                        break;
                    }
                case EventTypes.Debug:
                    {
                        logger.SetLogger(consoleLogger);
                        logger.Debug(e.Message);
                        logger.SetLogger(fileLogger);
                        logger.Debug(e.Message);
                        break;
                    }
                case EventTypes.Warning:
                    {
                        logger.SetLogger(consoleLogger);
                        logger.Warning(e.Message);
                        logger.SetLogger(fileLogger);
                        logger.Warning(e.Message);
                        break;
                    }
                case EventTypes.Error:
                    {
                        logger.SetLogger(consoleLogger);
                        logger.Error(e.Message);
                        logger.SetLogger(fileLogger);
                        logger.Error(e.Message);
                        break;
                    }
            }
        }

        public static void SendMail(string smtpServer, string from, string password,
                                    string mailto, string caption, string message, string attachFile = null)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from);
                mail.To.Add(new MailAddress(mailto));
                mail.Subject = caption;
                mail.Body = message;
                SmtpClient client = new SmtpClient();
                client.Host = smtpServer;
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(from.Split('@')[0], password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
                mail.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception("Mail.Send: " + e.Message);
            }
        }

        static void Main(string[] args)
        {
            var addressBook = AddressBook.AddressBook.Book;
            Program program = new Program(addressBook);
            try
            {
                addressBook.AddUser(null, "Khlybov", Convert.ToDateTime("06/01/1996"), "Kirovograd", null,  "0507240373", "Male", "hlybov@outlook.com");
                addressBook.AddUser("Yaroslav",  "Chaban",  Convert.ToDateTime("17/08/1996"), "Nadvirna",   "Chabana 13", "0997340583", "Male", "chaban@gmail.com");
                addressBook.AddUser("Vitalyi",   "Viktorchuk", Convert.ToDateTime("18/01/1995"), "Kyiv", "Vitakika 27",  "0668440737",  "Male", "vitakik@ukr.net");
                addressBook.AddUser("Roman", "Holovatyi", Convert.ToDateTime("07/12/1995"), "Kaniv", "Holovatyi station 76",  "0508765224",  "Male", "holovatyi@mail.ru");
                addressBook.AddUser("Evgen", "Yagolnik", Convert.ToDateTime("07/02/1996"), "Bila cerkva", "Evgeshi 19", "0634277303",  "Male", "yagolnik@gmail.com");
                addressBook.AddUser("Marianna", "Onoprienko", Convert.ToDateTime("31/05/1999"), "Kyiv", "Marianna 68", "0994847303", "Female", "onoprienko@gmail.com");
                addressBook.AddUser("Veronika", "Farktukh", Convert.ToDateTime("02/07/1996"), "Kyiv", "Farktukh 5B", "0504277330", "Female", "fartukh@outlook.com");

                addressBook.RemoveUser("Alexander", null);
                addressBook.RemoveUser("0508765224");

                Console.WriteLine("\n---------------------LINQ-----------------------------\n");

                Console.WriteLine("Users that have Gmail account: ");
                var gmailUsers = addressBook.GmailUsers();
                //Adding new user that has gmail after LINQ query
                addressBook.AddUser("TestUser", "", Convert.ToDateTime("02/07/1996"), "", "", "", "", "test@gmail.com");
                //TestUser will be shown in console because of lazy evaluation
                foreach (User user in gmailUsers)
                    Console.WriteLine(user);

                Console.WriteLine("\nAdult users from Kyiv: ");
                var adults = addressBook.AdultUsersFromKiev();
                foreach (User user in adults)
                    Console.WriteLine(user);

                Console.WriteLine("\nFemale users that were added during last 10 days");
                var newFemaleUsers = addressBook.NewFemaleUsers();
                foreach (User user in newFemaleUsers)
                    Console.WriteLine(user);

                Console.WriteLine("\nUsers that were born in Jan and have address and phone number");
                var janUsers = addressBook.BornInJanWithAddress();
                foreach (User user in janUsers)
                    Console.WriteLine(user);

                Console.WriteLine("\nTwo list of women and men respectively");
                var womenMen = addressBook.GenderSortedUsers();
                Console.WriteLine("Women: ");
                foreach (User user in womenMen["woman"])
                    Console.WriteLine(user);
                Console.WriteLine("Men: ");
                foreach (User user in womenMen["man"])
                    Console.WriteLine(user);

                Console.WriteLine("\nUsers taken by given predicate: ");
                var predUsers = addressBook.TakeByPredicate(user => user.Gender == "Male", 2, 4);
                foreach (User user in predUsers)
                    Console.WriteLine(user);

                Console.WriteLine("\nPeople users city X whose birthday is today");
                var birthdayGuys = addressBook.BirthdayGuysFromCityX("Kyiv");
                foreach (User user in birthdayGuys)
                    Console.WriteLine(user);

                birthdayGuys = addressBook.BirthdayGuys();
                foreach (User user in birthdayGuys)
                    SendMail("smtp.gmail.com", "mymail@gmail.com", "password", user.Email, "Birth Day", "Congratulations!");
            }
            catch (FormatException fex)
            {
                program.logger.SetLogger(program.fileLogger);
                program.logger.Error(string.Format(fex.Message + "; " + DateTime.Now));
                program.logger.SetLogger(program.consoleLogger);
                program.logger.Error(string.Format(fex.Message + "; " + DateTime.Now));
            }
            catch (NullReferenceException nex)
            {
                program.logger.SetLogger(program.fileLogger);
                program.logger.Error(string.Format(nex.Message + "; " + DateTime.Now));
                program.logger.SetLogger(program.consoleLogger);
                program.logger.Error(string.Format(nex.Message + "; " + DateTime.Now));
            }
            catch (Exception ex)
            {
                program.logger.SetLogger(program.fileLogger);
                program.logger.Error(string.Format(ex.Message + "; " + DateTime.Now));
                program.logger.SetLogger(program.consoleLogger);
                program.logger.Error(string.Format(ex.Message + "; " + DateTime.Now));
            }
            Console.Read();
        }
    }
}

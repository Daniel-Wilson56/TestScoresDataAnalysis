using MySql.Data.MySqlClient;

namespace TestScoresData
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter password: ");
            string password = Console.ReadLine();
            DatabaseInterface db = new DatabaseInterface();
            bool success = db.ConnectToDatabase(password);
            if (success)
            {
                UserInterface ui = new(db);
                bool finished;
                do
                {
                    finished = ui.AskForOptions();
                } while (!finished);
            }
        }
    }
}
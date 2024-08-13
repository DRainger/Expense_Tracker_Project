using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Runtime.Remoting.Messaging;
using System.Data;

namespace Expense_Tracker
{
    internal class Program
    {
        //------------ func to create databace ------------
        static public void CreatDB(string server)
        {
            try
            {
                SqlConnection mySqlConnection = new SqlConnection($"server={server};Integrated Security=SSPI;");
                SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = "create database ExpenseTrackerDB;"; // command SQL
                mySqlConnection.Open();
                int n = mySqlCommand.ExecuteNonQuery();
                Console.WriteLine("creat database " + n);
                mySqlConnection.Close();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }

        }
        //------------ func to present data from tables ------------
        static public void Show(string str, string server)
        {
            try
            {
                SqlConnection mySqlConnection = new SqlConnection($"server={server};database=ExpenseTrackerDB;Integrated Security=SSPI;");
                SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = str; // command SQL
                mySqlConnection.Open();
                SqlDataReader mySqlDateReader = mySqlCommand.ExecuteReader();
                for (int i = 0; i < mySqlDateReader.FieldCount; i++) // present colomns names 
                {
                    Console.Write("{0,-20}", mySqlDateReader.GetName(i));
                }
                Console.WriteLine("\n");
                while (mySqlDateReader.Read()) // present row values 
                {
                    
                    for (int i = 0; i < mySqlDateReader.FieldCount; i++)
                    {
                        Console.Write("{0,-20}",mySqlDateReader[i].ToString());

                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                mySqlDateReader.Close();
                mySqlConnection.Close();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
        //------------ func to create tables and insert data ------------
        static public void CreatTable(string str, string server)
        {
            try
            {
                SqlConnection mySqlConnection = new SqlConnection($"server={server};database=ExpenseTrackerDB;Integrated Security=SSPI;");
                SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = str; // command SQL
                mySqlConnection.Open();
                int n = mySqlCommand.ExecuteNonQuery();
                Console.WriteLine("affected rows " + n);
                mySqlConnection.Close();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }

        }

        //------------ func to get value of query result ------------
        static public string getValue(string str, string server, string column)
        {
            string value = "";
            try
            {
                SqlConnection mySqlConnection = new SqlConnection($"server={server};database=ExpenseTrackerDB;Integrated Security=SSPI;");
                SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = str; // command SQL
                mySqlConnection.Open();
                SqlDataReader mySqlDateReader = mySqlCommand.ExecuteReader();
                DataTable dt = new DataTable();


                if (mySqlDateReader.HasRows)
                {
                    dt.Load(mySqlDateReader);
                    foreach (DataRow row in dt.Rows)
                        value = row[column].ToString();
                }
                else
                    Console.WriteLine("No rows found.");
                Console.WriteLine();
                mySqlDateReader.Close();
                mySqlConnection.Close();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            return value;
        }

        //------------ func to check if table exists or not ------------
        static public bool DbTableExists(string tableName, string server)
        {
            SqlConnection mySqlConnection = new SqlConnection($"server={server};database=ExpenseTrackerDB;Integrated Security=SSPI;");
            using (mySqlConnection)
            {
                string CheckTable = string.Format(
                    "IF OBJECT_ID('{0}','U') IS NOT NULL SELECT 'true' ELSE SELECT 'false'", tableName);
                SqlCommand cmd = new SqlCommand(CheckTable, mySqlConnection);
                cmd.CommandType = CommandType.Text;
                mySqlConnection.Open();

                return Convert.ToBoolean(cmd.ExecuteScalar());
            }
        }
        //------------ login manu ------------
        static public void loginMenu(string server)
        {
            while (true)
            {
                int choice, id = 0;
                string username = "", password = "", email;
                Console.Clear();
                Console.WriteLine("Welcome!");
                Console.WriteLine("1 - Login");
                Console.WriteLine("2 - Create User");
                Console.WriteLine("0 - Exit");
                choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Enter Username:");
                        username = Console.ReadLine();
                        Console.WriteLine("Enter password:");
                        password = Console.ReadLine();
                        mainMenu(server, username, password);
                        break;
                    case 2:
                        Console.WriteLine("Enter Username:");
                        username = Console.ReadLine();
                        Console.WriteLine("Enter password:");
                        password = Console.ReadLine();
                        Console.WriteLine("Enter I.D:");
                        id = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter email:");
                        email = Console.ReadLine();
                        CreatTable($"insert into Users values({id},'{username}','{password}','{email}');", server);
                        mainMenu(server, username, password);
                        break;
                    case 9: return;
                }
                Console.ReadKey();
            }
            Console.ReadKey();
        }
        //------------ main manu ------------
        static public void mainMenu(string server, string username, string password)
        {
            while (true)
            {
                int choice, expType = 0, amount = 0 ,expID = 0, id, cataId;
                string description = "", cataName = "";
                Console.Clear();
                Console.WriteLine("\nMain Menu\n");
                Console.WriteLine("1 - Show all my Expenses");
                Console.WriteLine("2 - Add Expens");
                Console.WriteLine("3 - Delete Expens");
                Console.WriteLine("4 - Edit Expens");
                Console.WriteLine("5 - Maximum Expens");
                Console.WriteLine("6 - Averge Expenss");
                Console.WriteLine("7 - Sum all My Expenss");
                Console.WriteLine("8 - Add Expens Category");
                Console.WriteLine("9 - Switch User");
                Console.WriteLine("0 - Exit");
                choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        Show($"SELECT Expenses.ExpenseID,Expenses.[Description], Expenses.Amount,Categories.CategoryName,Expenses.[date],Expenses.UserID FROM Expenses left join Users on Expenses.UserID = Users.UserID inner join Categories on Categories.CategoryID=Expenses.CategoryID where Users.Username = '{username}' and Users.[Password] = '{password}';", server);
                        break;
                    case 2:
                        Console.WriteLine("what kind of expense?");
                        Show("select * from Categories", server);
                        expType = int.Parse(Console.ReadLine());
                        Console.WriteLine("write a description:");
                        description = Console.ReadLine();
                        Console.WriteLine("amount of expense:");
                        amount = int.Parse(Console.ReadLine());
                        int.TryParse(getValue($"select Users.UserID from Users where Users.Username = '{username}' and Users.[Password] = '{password}';", server,"UserID"), out id);
                        CreatTable($"insert into Expenses ([Description], Amount, CategoryID, UserID) values('{description}',{amount},{expType},{id})",server);
                        break;
                    case 3:
                        Console.WriteLine("which expense?");
                        expID = int.Parse(Console.ReadLine());
                        CreatTable($"DELETE FROM Expenses WHERE ExpenseID ='{expID}';",server);
                        break;
                    case 4:
                        Console.WriteLine("which expense?");
                        expID = int.Parse(Console.ReadLine());
                        Console.WriteLine("what kind of expense?");
                        Show("select * from Categories", server);
                        expType = int.Parse(Console.ReadLine());
                        Console.WriteLine("write a description:");
                        description = Console.ReadLine();
                        Console.WriteLine("amount of expense:");
                        amount = int.Parse(Console.ReadLine());
                        CreatTable($"UPDATE Expenses SET Description = '{description}', Amount = {amount}, CategoryID = {expType} WHERE ExpenseID = '{expID}'", server);
                        break;
                    case 5:
                        Show($"SELECT MAX(Expenses.Amount) as Maximum from Expenses left join Users on Expenses.UserID = Users.UserID WHERE Users.Username = '{username}' and Users.[Password] = '{password}'", server);
                        break;
                    case 6:
                        Show($"SELECT AVG(Expenses.Amount) as Averge from Expenses left join Users on Expenses.UserID = Users.UserID WHERE Users.Username = '{username}' and Users.[Password] = '{password}'", server);
                        break;
                    case 8:
                        Console.WriteLine("Categorie name:");
                        cataName = Console.ReadLine();
                        int.TryParse(getValue("SELECT MAX(Categories.CategoryID) as CategoryID FROM Categories", server, "CategoryID"), out cataId);
                        CreatTable($"insert into Categories values('{cataId + 1}','{cataName}');", server);
                        break;
                    case 7:
                        Show($"SELECT SUM(Expenses.Amount) as Sum_of_Expenses from Expenses left join Users on Expenses.UserID = Users.UserID WHERE Users.Username = '{username}' and Users.[Password] = '{password}'", server);
                        break;
                    case 9: 
                        loginMenu(server); 
                        break;
                    case 0: return;
                }
                Console.WriteLine(" Press Any key to continue ...");
                Console.ReadKey();
            }
            Console.ReadKey();
        }
     

    static void Main(string[] args)
        {
            int Do = 1;
            // (to switch servers simply change the value of the "server" veriable)
            string server = "LocalHost\\SQLEXPRESS"; // "LocalHost\\SQLEXPRESS" // "HAWKSTATION\\SQLEXPRESS"

            /*---------------create database----------------*/
            CreatDB(server);
            /*---------------create database----------------*/

            CreatTable("drop table Users", server);
            CreatTable("drop table Expenses", server);
            CreatTable("drop table Categories", server);


            /*---------------create tables----------------*/
            if (!DbTableExists("Users",server)) // check if table exists or not 
                CreatTable("use ExpenseTrackerDB; create table Users (UserID int not null primary key ,Username nvarchar(50),Password nvarchar(20) check(Password like '%[a-zA-Z0-9]%'),Email nvarchar(255) check(email like '%__@__%.__%'));", server);
            if (!DbTableExists("Categories", server))// check if table exists or not 
                CreatTable("use ExpenseTrackerDB; create table Categories (CategoryID int not null primary key ,CategoryName nvarchar(30));", server);
            if (!DbTableExists("Expenses", server))// check if table exists or not 
                CreatTable("use ExpenseTrackerDB create table Expenses (ExpenseID int not null IDENTITY(1, 1) PRIMARY KEY ,Description nvarchar(30), Amount decimal, CategoryID int, date datetime DEFAULT GETDATE(),UserID int, FOREIGN KEY (CategoryID) REFERENCES Categories);", server);
            /*---------------create tables----------------*/

            /*---------------insert in to tables----------------*/
            if (DbTableExists("Users", server))// check if table exists or not 
                CreatTable("insert into Users values(111111111,'Adam','adamA123','adam@gmail.com'),(222222222,'Bob','bobD147','bob@gmail.com'),(333333333,'dilen','dilenS258','dilen@gmail.com'),(444444444,'Emma','emmaG789','emma@gmail.com')," +
                "(555555555,'sam','samP369','sam@gmail.com'),(666666666,'rily','rilyK753','rily@gmail.com');", server);
            if (DbTableExists("Categories", server))// check if table exists or not 
                CreatTable("insert into Categories values(1,'Groceries'),(2,'Utilities'),(3,'Dining Out'),(4,'Transportation'),(5,'Entertainment'),(6,'Health'),(7,'Shopping')", server);
            if (DbTableExists("Expenses", server))// check if table exists or not 
                CreatTable("insert into Expenses ([Description], Amount, CategoryID, [date], UserID) values('Grocery shopping',50.00,1,'2024-06-15 10:00:00',111111111),('Dinner with friends',80.00,3,'2024-06-14 19:30:00',222222222),('Gasoline',50.00,4,'2024-06-13 14:15:00',111111111)"+
               ", ('Movie night', 25.00, 5, '2024-06-12 20:00:00', 333333333), ('Pharmacy', 15.00, 6, '2024-06-11 11:45:00', 222222222), ('Book purchase', 30.00, 7, '2024-06-10 16:20:00', 111111111),"+
                "('Electricity bill', 100.00, 2, '2024-06-09 08:00:00', 333333333), ('Lunch at work', 15.00, 3, '2024-06-08 12:30:00', 111111111); ", server);
            /*---------------insert in to tables----------------*/
            
            
            loginMenu(server);
        }
    }                                                       
}

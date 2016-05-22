using System;
using System.IO;
using MySql.Data.MySqlClient;
using System.Windows;

namespace JKPlacementsDBManager
{
    public class DBConnect
    {

        public MySqlConnection conn;
        public bool connState;
        string[] settings = new string[5];
        public string server, database, uid, password, port;
        public string table1 = "company"; //Company
        public string table2 = "contacts"; //Contacts
        public string table3 = "pending"; //Pending
        public string table4 = "contactnos"; //ContactNos

        public string table1CompanyNum = "CompanyNum"; //TABLE 1 -> company
        public string table1CompanyName = "CompanyName";
        public string table1DateOfApproach = "DateOfApproach";
        public string table1Comments = "Comments";
        public string table1Response = "Response";
        public string table1Stream = "Stream";

        public string table2ContactID = "ContactID"; //TABLE 2 -> contacts
        public string table2CompanyName = "CompanyName";
        public string table2ContactPerson = "ContactPerson";
        public string table2Position = "Position";

        public string table3PendingJobID = "PendingJobID"; //TABLE 3 -> pending
        public string table3CompanyName = "CompanyName";
        public string table3PendingJob = "PendingJob";
        public string table3FinalDate = "FinalDate";
        public string table3JobStatus = "JobStatus";

        public string table4ContactNoID = "ContactNoID"; //TABLE 4 -> contactnos
        public string table4CompanyName = "CompanyName";
        public string table4ContactPerson = "ContactPerson";
        public string table4ContactNum = "ContactNum";
        public string table4ContactEmail = "ContactEmail";

        public DBConnect()
        {
            try
            {
                settings = File.ReadAllLines(@MainWindow.ServerLogLocation);
                server = settings[0];
                database = settings[1];
                uid = settings[2];
                password = settings[3];
                port = settings[4];
                File.SetAttributes(@MainWindow.ServerLogLocation, FileAttributes.ReadOnly);
            }
            catch (FileNotFoundException e)
            {
                server = "localhost";
                database = "placements";
                uid = "root";
                password = "";
                port = "3306";
                settings[0] = server;
                settings[1] = database;
                settings[2] = uid;
                settings[3] = password;
                settings[4] = port;
                try
                {
                    File.WriteAllLines(@MainWindow.ServerLogLocation, settings);
                    //MainWindow.WriteToFile(MainWindow.ServerLogLocation, "\n------SESSION START TIMESTAMP : " + DateTime.Now.ToString() + " ------\n");
                    File.SetAttributes(@MainWindow.ServerLogLocation, FileAttributes.ReadOnly);
                    File.SetAttributes(@MainWindow.ServerLogLocation, FileAttributes.Hidden);
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show("Restart Application with administrative privileges.");
                }
            }
            Initialize();
        }

        public void Initialize()
        {
            string connString = "Server=" + server + ";Port=" + port + ";Database=" + database + ";Uid=" + uid + ";Pwd=" + password + ";";
            conn = new MySqlConnection(connString);
        }

        public bool OpenConnection()
        {
            Initialize();
            try
            {
                conn.Open();
                connState = true;
                return true;
            }
            catch (Exception e)
            {
                connState = false;
                return false;
            }
        }

        public bool CloseConnection()
        {
            Initialize();
            try
            {
                conn.Close();
                connState = false;
                return true;
            }
            catch (Exception e)
            {
                connState = true;
                MessageBox.Show(e.Message);
                return false;
            }
        }
    }

}

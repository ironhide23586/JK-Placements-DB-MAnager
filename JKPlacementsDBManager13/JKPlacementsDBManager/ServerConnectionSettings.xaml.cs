using System;
using System.IO;
using System.Windows;

namespace JKPlacementsDBManager
{
    /// <summary>
    /// Interaction logic for ServerConnectionSettings.xaml
    /// </summary>
    public partial class ServerConnectionSettings : Window
    {
        public ServerConnectionSettings()
        {
            InitializeComponent();
        }

        private void ConnTestButton_Click(object sender, RoutedEventArgs e)
        {
            string[] prevVals = new string[5];
            prevVals[0] = MainWindow.connectionObj.server;
            prevVals[1] = MainWindow.connectionObj.database;
            prevVals[2] = MainWindow.connectionObj.uid;
            prevVals[3] = MainWindow.connectionObj.password;
            prevVals[4] = MainWindow.connectionObj.port;
            MainWindow.connectionObj.server = serverField.Text;
            MainWindow.connectionObj.database = databaseField.Text;
            MainWindow.connectionObj.uid = uidField.Text;
            MainWindow.connectionObj.password = passwordField.Password;
            MainWindow.connectionObj.port = portField.Text;
            if (MainWindow.connectionObj.OpenConnection())
            {
                MessageBox.Show("Server Connection Successful!  :D");
                MainWindow.connectionObj.CloseConnection();
            }
            else
            {
                MessageBox.Show("Server Connection Failure.   :(");
            }
            MainWindow.connectionObj.server = prevVals[0];
            MainWindow.connectionObj.database = prevVals[1];
            MainWindow.connectionObj.uid = prevVals[2];
            MainWindow.connectionObj.password = prevVals[3];
            MainWindow.connectionObj.port = prevVals[4];
            MainWindow.connectionObj.Initialize();
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.connectionObj.server = serverField.Text;
            MainWindow.connectionObj.database = databaseField.Text;
            MainWindow.connectionObj.uid = uidField.Text;
            MainWindow.connectionObj.password = passwordField.Password;
            MainWindow.connectionObj.port = portField.Text;
            MainWindow.connectionObj.Initialize();
            string[] prevVals = new string[5];
            prevVals[0] = MainWindow.connectionObj.server;
            prevVals[1] = MainWindow.connectionObj.database;
            prevVals[2] = MainWindow.connectionObj.uid;
            prevVals[3] = MainWindow.connectionObj.password;
            prevVals[4] = MainWindow.connectionObj.port;
            try
            {
                File.SetAttributes(@MainWindow.ServerLogLocation, FileAttributes.Normal);
                File.WriteAllLines(@MainWindow.ServerLogLocation, prevVals);
                File.SetAttributes(@MainWindow.ServerLogLocation, FileAttributes.ReadOnly);
                File.SetAttributes(@MainWindow.ServerLogLocation, FileAttributes.Hidden);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("Restart Application with administrative priviledges.");
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] data = new string[5];
            File.SetAttributes(@MainWindow.ServerLogLocation, FileAttributes.Normal);
            data = File.ReadAllLines(@MainWindow.ServerLogLocation);
            File.SetAttributes(@MainWindow.ServerLogLocation, FileAttributes.ReadOnly);
            File.SetAttributes(@MainWindow.ServerLogLocation, FileAttributes.Hidden);
            serverField.Text = data[0];
            databaseField.Text = data[1];
            uidField.Text = data[2];
            passwordField.Password = data[3];
            portField.Text = data[4];
        }
    }
}

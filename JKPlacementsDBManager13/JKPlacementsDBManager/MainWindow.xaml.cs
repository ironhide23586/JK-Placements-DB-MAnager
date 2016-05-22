using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace JKPlacementsDBManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static DataTable company = new DataTable();
        public static DataTable contacts = new DataTable();
        public static DataTable pending = new DataTable();
        public static DataTable contactnos = new DataTable();
        public static DataTable contactNosFull = new DataTable();
        public static DataTable contactsGridTable = new DataTable();
        public static DataTable contactsGridTableTemp = new DataTable();
        public static DBConnect connectionObj;

        public static string ServerLogLocation = "ServerConnSettings";

        public MainWindow()
        {
            InitializeComponent();
        }

        public static void WriteToFile(string loc, string content)
        {
            try
            {
                File.SetAttributes(@loc, FileAttributes.Normal);
                File.AppendAllText(@loc, content);
            }
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show("Restart Application with administrative privileges. :/");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Critical Error :(");
            }
            try
            {
                File.SetAttributes(@loc, FileAttributes.ReadOnly);
                File.SetAttributes(@loc, FileAttributes.Hidden);
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to set config file to Read-only/Hidden mode :(");
            }
        }

        private void ConnSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServerConnectionSettingsAuthorization connSettingsAuthorizationWindow = new ServerConnectionSettingsAuthorization();
                this.Opacity = 0.5;
                connSettingsAuthorizationWindow.ShowDialog();
                this.Opacity = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Critical Error :(\nError Details -\n" + ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                connectionObj = new DBConnect();
                WriteToFile(ServerLogLocation, "\n------SESSION START TIMESTAMP : " + DateTime.Now.ToString() + " ------\n");
                fillTableHere("SELECT * FROM " + connectionObj.table1 + ";", ref company);
                modifyDGrid(company, DGrid);
                initContactsDGridDataTable();
            }
            catch (Exception ex1)
            {
                MessageBox.Show("Database Connectivity Error :(\nError Details -\n" + ex1.ToString());
            }
        }

        public static bool fillTable(string queryString, ref DataTable dt)
        {
            dt = new DataTable();
            if (!connectionObj.OpenConnection())
            {
                MessageBox.Show("Server Connection Failure.   :(");
                return false;
            }
            MySqlCommand cmd = new MySqlCommand(queryString, connectionObj.conn);
            MySqlDataAdapter dAdapter = new MySqlDataAdapter(cmd);
            try
            {
                dAdapter.Fill(dt);
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Invalid SQL String. \nSQL String : \"" + queryString + "\"");
                return false;
            }
        }

        private void fillTableHere(string queryString, ref DataTable dt)
        {
            if (fillTable(queryString, ref dt))
            {
                if (QueryLogCheckBox.IsChecked == true)
                    WriteToFile(@ServerLogLocation, queryString + "\n");
            }
        }

        private void modifyDGrid(DataTable dt, DataGrid dg)
        {
            if (connectionObj.connState)
            {
                dg.ItemsSource = dt.DefaultView;
                connectionObj.CloseConnection();
            }
        }

        private void RefreshButton1_Click(object sender, RoutedEventArgs e)
        {
            refreshMethod();
        }

        private void refreshMethod()
        {
            try
            {
                fillTableHere("SELECT * FROM " + connectionObj.table1 + ";", ref company);
                modifyDGrid(company, DGrid);
            }
            catch (Exception e2)
            {
                MessageBox.Show("Database Connectivity Error :(\nError Details -\n" + e2.ToString());
            }
        }

        private void DBConnectionTestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (connectionObj.OpenConnection())
                {
                    MessageBox.Show("Server Connection Successful!  :D");
                    connectionObj.CloseConnection();
                }
                else
                {
                    MessageBox.Show("Server Connection Failure.   :(");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Connectivity Error :(\nError Details -\n" + ex.ToString());
            }
        }

        private void DGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            try
            {
                DGrid.Columns[0].Header = "Sr. No.";
                DGrid.Columns[1].Header = "Company Name";
                DGrid.Columns[2].Header = "Date of \nApproach";
                DGrid.Columns[4].CanUserResize = false;
                DGrid.Columns[5].CanUserResize = false;
                DGrid.Columns[2].MaxWidth = 75;
                DGrid.Columns[3].Width = 110;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Critical Error :(\nError Details -\n" + ex.ToString());
            }
        }

        private void ModifyDBButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServerConnectionSettingsAuthorization window = new ServerConnectionSettingsAuthorization();
                window.targetWindow = JKPlacementsDBManager.ServerConnectionSettingsAuthorization.targWin.DBMod;
                bool temp = false;
                if (QueryLogCheckBox.IsChecked == true)
                    temp = true;
                window.queryMaintain = temp;
                this.Opacity = 0.5;
                window.ShowDialog();
                this.Opacity = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR 0XEFF0004574ABCEF : System.Windows.MicrosoftRulz.IllegalOperationException. \nYou have caused irreversible damage to the memory write mechanism.\nGod will punish you now.\n\n" + ex.ToString());
            }
        }

        private void ClearEntriesButton_Click(object sender, RoutedEventArgs e)
        {
            SrNoField.Text = "";
            CompanyNameField.Text = "";
            DateOfApproachField.SelectedDate = null;
            ResponseBox.SelectedIndex = -1;
            StreamBox.SelectedIndex = -1;
        }

        private void searchMethod()
        {
            if (StreamBox.SelectedIndex == -1)
                StreamBox.SelectedIndex = 3;
            if (ResponseBox.SelectedIndex == -1)
                ResponseBox.SelectedIndex = 3;
            string originalString = CompanyNameField.Text;
            string cNameTemp = CompanyNameField.Text;
            DataModder.legalizeSQLString(ref cNameTemp);
            CompanyNameField.Text = cNameTemp;
            if ((SrNoField.Text == "") && (CompanyNameField.Text == "") && (DateOfApproachField.SelectedDate == null) && (ResponseBox.SelectedIndex == 3) && (StreamBox.SelectedIndex == 3))
            {
                fillTableHere("SELECT * FROM " + connectionObj.table1 + ";", ref company);
                modifyDGrid(company, DGrid);
            }
            else
            {
                string[] responseTable = new string[]
                {
                    "Yes",
                    "No",
                    "Floating",
                    ""
                };
                string[] streamTable = new string[]
                {
                    "CSE",
                    "ECE",
                    "CSE/ECE",
                    ""
                };
                string date;
                try
                {
                    date = DateTime.Parse(DateOfApproachField.SelectedDate.ToString()).ToString("yyyy-MM-dd");
                }
                catch (FormatException ex)
                {
                    date = "";
                }
                string cNum = "", cName = "", dOA = "", rspns = "", strm = "";
                if (SrNoField.Text != "")
                    cNum = connectionObj.table1CompanyNum + " = '" + SrNoField.Text + "'|";
                if (CompanyNameField.Text != "")
                {
                    if(InstaSearchCheckBox.IsChecked==false)
                        cName = connectionObj.table1CompanyName + " = '" + CompanyNameField.Text + "'|";
                    else
                        cName = connectionObj.table1CompanyName + " LIKE '" + CompanyNameField.Text + "%'|";
                }
                if (date != "")
                    dOA = connectionObj.table1DateOfApproach + " = '" + date + "'|";
                if (ResponseBox.SelectedIndex != 3)
                    rspns = connectionObj.table1Response + " = '" + responseTable[ResponseBox.SelectedIndex] + "'|";
                if (StreamBox.SelectedIndex != 3)
                    strm = connectionObj.table1Stream + " = '" + streamTable[StreamBox.SelectedIndex] + "'|";
                string query = "SELECT * FROM " + connectionObj.table1 + " WHERE " + cNum + cName + dOA + rspns + strm + ";";
                string[] querySplit = query.Split('|');
                query = "";
                for (int i = 0; i < querySplit.Length - 1; i++)
                {
                    if (i == querySplit.Length - 2)
                        query += querySplit[i] + ";";
                    else
                        query += querySplit[i] + " AND ";
                }
                fillTableHere(query, ref company);
                modifyDGrid(company, DGrid);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                searchMethod();
            }
            catch (Exception ex1)
            {
                MessageBox.Show("Database Connectivity Error :(\nError Details -\n" + ex1.ToString());
            }
        }

        private void SrNoField_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                if (SrNoField.Text != "")
                {
                    try
                    {
                        Convert.ToInt32(SrNoField.Text);
                    }
                    catch (FormatException ex)
                    {
                        MessageBox.Show("Please enter a number in the Sr. No. Field.");
                        SrNoField.Text = "";
                    }
                }
                if (InstaSearchCheckBox.IsChecked == true)
                    searchMethod();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Connectivity Error :(\nError Details -\n" + ex.ToString());
            }
        }
        
        private void CompanyNameField_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                if (InstaSearchCheckBox.IsChecked == true)
                    searchMethod();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Connectivity Error :(\nError Details -\n" + ex.ToString());
            }
        }

        private void DateOfApproachField_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (InstaSearchCheckBox.IsChecked == true)
                    searchMethod();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Connectivity Error :(\nError Details -\n" + ex.ToString());
            }
        }


        private bool var = false;
        private void ResponseBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (var)
                {
                    if (InstaSearchCheckBox.IsChecked == true)
                        searchMethod();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Connectivity Error :(\nError Details -\n" + ex.ToString());
            }
        }

        private void StreamBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (var)
                {
                    if (InstaSearchCheckBox.IsChecked == true)
                        searchMethod();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Connectivity Error :(\nError Details -\n" + ex.ToString());
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                WriteToFile(@ServerLogLocation, "------SESSION END TIMESTAMP : " + DateTime.Now.ToString() + " ------\n");
                File.SetAttributes(@ServerLogLocation, FileAttributes.ReadOnly);
                File.SetAttributes(@ServerLogLocation, FileAttributes.Hidden);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Critical Runtime Error :(\nError Details -\n" + ex.ToString());
            }
        }

        private void DGrid_AutoGeneratingColumn(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd-MM-yyyy";
        }

        private void fillContactsDataGrid()
        {
            int i = 0;
            contactsGridTable.Clear();
            foreach (DataRow dR in contacts.Rows)
            {
                contactsGridTable.Rows.Add(contactsGridTable.NewRow());
                contactsGridTable.Rows[i][0] = dR.ItemArray[0];
                contactsGridTable.Rows[i][1] = dR.ItemArray[2];
                contactsGridTable.Rows[i][4] = dR.ItemArray[3];
                foreach (DataRow dR2 in contactNosFull.Rows)
                {
                    if ((dR2.ItemArray[2].ToString() == dR.ItemArray[2].ToString()) && (dR.ItemArray[1].ToString() == dR2.ItemArray[1].ToString()))
                    {
                        contactsGridTable.Rows[i][2] += dR2.ItemArray[3] + "\n";
                        contactsGridTable.Rows[i][3] += dR2.ItemArray[4] + "\n";
                        
                    }
                }
                i++;
            }
            modifyDGrid(contactsGridTable, contactsDGrid);
        }

        //public static bool ifStringStartsWith(string beginning, string inp)
        //{
        //    string tmp = inp.Remove(beginning.Length);
        //    char[] tmpArr = tmp.ToCharArray();
        //    //Console.Write(tmpArr.Length.ToString());
        //    if (tmp.ToLower() == beginning.ToLower())
        //        return true;
        //    return false;
        //}


        //private void fillContactsDataGrid(string contactName)
        //{
        //    int i = 0;
        //    contactsGridTable.Clear();
            
        //    foreach (DataRow dR in contacts.Rows)
        //    {
        //        if (ifStringStartsWith(contactName, dR.ItemArray[2].ToString()))
        //        {
        //            contactsGridTable.Rows.Add(contactsGridTable.NewRow());
        //            contactsGridTable.Rows[i][0] = dR.ItemArray[0];
        //            contactsGridTable.Rows[i][1] = dR.ItemArray[2];
        //            contactsGridTable.Rows[i][4] = dR.ItemArray[3];
        //            foreach (DataRow dR2 in contactNosFull.Rows)
        //            {
        //                if ((dR2.ItemArray[2].ToString() == dR.ItemArray[2].ToString()) && (dR.ItemArray[1].ToString() == dR2.ItemArray[1].ToString()))
        //                {
        //                    contactsGridTable.Rows[i][2] += dR2.ItemArray[3] + "\n";
        //                    contactsGridTable.Rows[i][3] += dR2.ItemArray[4] + "\n";

        //                }
        //            }
        //            i++;
        //        }
        //    }
        //    modifyDGrid(contactsGridTable, contactsDGrid);
        //}

        private void DGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataRowView[] k = new DataRowView[1];
                e.AddedItems.CopyTo(k, 0);
                InfoBoxBlock.Text = getDetails(Convert.ToInt32(k[0].Row.ItemArray[0]));
                fillContactsDataGrid();
            }
            catch (NullReferenceException) { return; }
            catch (ArgumentException) { return; }
            catch (Exception ex)
            {
                MessageBox.Show("Critical Error :(\nError Details -\n" + ex.ToString());
            }
        }

        private string getDetails(int SelectedIndex)
        {
            string outString = "";
            string selectedCompanyName;
            string selectedDateOfApproach;
            string selectedComments;
            string selectedResponse;
            string selectedStream;
            contactNosFull.Clear();
            try
            {
                DataTable tmpDt = new DataTable();
                fillTableHere("SELECT * FROM " + connectionObj.table1 + " WHERE " + connectionObj.table1CompanyNum + " = '" + SelectedIndex + "';", ref tmpDt);  
                //selectedCompanyName = (string)company.Rows[SelectedIndex][1];
                selectedCompanyName = (string)tmpDt.Rows[0][1];
                try
                {
                    //selectedDateOfApproach = company.Rows[SelectedIndex][2].ToString().Remove(10);
                    selectedDateOfApproach = tmpDt.Rows[0][2].ToString().Remove(10);
                }
                catch (Exception ex)
                {
                    selectedDateOfApproach = "";
                }
                try
                {
                    //selectedComments = (string)company.Rows[SelectedIndex][3];
                    selectedComments = (string)tmpDt.Rows[0][3];
                }
                catch (Exception ex)
                {
                    selectedComments = "";
                }
                try
                {
                    //selectedResponse = (string)company.Rows[SelectedIndex][4];
                    selectedResponse = (string)tmpDt.Rows[0][4];
                }
                catch (Exception ex)
                {
                    selectedResponse = "";
                }
                try
                {
                    //selectedStream = (string)company.Rows[SelectedIndex][5];
                    selectedStream = (string)tmpDt.Rows[0][5];
                }
                catch (Exception ex)
                {
                    selectedStream = "";
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                return "";
            }
            fillTableHere("SELECT * FROM " + connectionObj.table2 + " WHERE " + connectionObj.table1CompanyName + " = '" + selectedCompanyName + "';", ref contacts);
            fillTableHere("SELECT * FROM " + connectionObj.table3 + " WHERE " + connectionObj.table1CompanyName + " = '" + selectedCompanyName + "';", ref pending);
            outString = "Company Name : " + selectedCompanyName + "\n-------------------------------------------------\nDate of Approach : " + selectedDateOfApproach + "\n-------------------------------------------------\nComments -\n\n" + selectedComments + "\n-------------------------------------------------\nResponse : " + selectedResponse + "\n-------------------------------------------------\nStream : " + selectedStream + "\n-------------------------------------------------\n\nCONTACTS :-\n******************************\n";
            int i = 1;
            foreach (DataRow d in contacts.Rows)
            {
                try
                {
                    outString += i++ + ".) " + (string)d.ItemArray[2] + "\nPosition : " + (string)d.ItemArray[3] + "\n\nEmail(s) -\n";
                }
                catch (InvalidCastException)
                {
                    outString += i++ + ".) " + (string)d.ItemArray[2] + "\n\nEmail(s) -\n";
                }
                fillTableHere("SELECT * FROM " + connectionObj.table4 + " WHERE " + connectionObj.table4CompanyName + " = '" + selectedCompanyName + "' AND " + connectionObj.table4ContactPerson + " = '" + (string)d.ItemArray[2] + "';", ref contactnos);
                contactNosFull.Merge(contactnos, true);
                foreach (DataRow d2 in contactnos.Rows)
                {
                    try
                    {
                        if (d2.ItemArray[4] != null)
                            outString += (string)d2.ItemArray[4] + "\n";
                    }
                    catch (InvalidCastException e3)
                    {
                    }
                }
                outString += "\nPhone No(s) -\n";
                foreach (DataRow d2 in contactnos.Rows)
                {
                    try
                    {
                        outString += (string)d2.ItemArray[3] + "\n";
                    }
                    catch (Exception ex)
                    {
                    }
                }
                outString += "******************************\n";
            }
            outString += "-------------------------------------------------\nPENDING JOBS - \n\n";
            i = 1;
            foreach (DataRow d in pending.Rows)
            {
                try
                {
                    outString += i + ".) " + (string)d.ItemArray[2] + "\nFinal Date : " + d.ItemArray[3].ToString().Remove(10) + "\nJob Status : " + d.ItemArray[4] + "\n\n";
                }
                catch (Exception ex)
                {
                    outString += i + ".) " + (string)d.ItemArray[2] + "\nFinal Date : " + "\nJob Status : " + d.ItemArray[4] + "\n\n";
                }
                i++;
            }
            return outString;
        }

        private void ExportDataButton_Click(object sender, RoutedEventArgs e)
        {
            refreshMethod();
            string[] data = new string[1];

            foreach (DataRow dR in company.Rows)
            {
                data[0] += getDetails(Convert.ToInt32(dR.ItemArray[0])) + "\n\nCompany Placement ID = " + dR.ItemArray[0].ToString() + "\n++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n------------------------------------------------------------\n++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n\n";
            }
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.Filter = "Word Document (*.docx)|*.docx|Open Document (*.odt)|*.odt";
            saveDialog.ShowDialog();
            try
            {
                File.WriteAllLines(@saveDialog.FileName, data);
            }
            catch (ArgumentException) { }
        }

        private void InstaSearchCheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            var = true;
        }

        private void initContactsDGridDataTable()
        {
            contactsGridTable.Columns.Add("Contact ID", System.Type.GetType("System.Int32"));
            contactsGridTable.Columns.Add("Contact", System.Type.GetType("System.String"));
            contactsGridTable.Columns.Add("Contact Nos", System.Type.GetType("System.String"));
            contactsGridTable.Columns.Add("Email(s)", System.Type.GetType("System.String"));
            contactsGridTable.Columns.Add("Position", System.Type.GetType("System.String"));
            contactsGridTableTemp = contactsGridTable.Clone();
        }


        private void ContactsSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            contactsGridTableTemp.Clear();
            DataRow[] dArr = contactsGridTable.Select("Contact LIKE '%" + ContactsSearchTextBox.Text + "%'");
            for (int tmp = 0; tmp < dArr.Length; tmp++)
            {
                contactsGridTableTemp.ImportRow(dArr[tmp]);
            }
            contactsDGrid.ItemsSource = contactsGridTableTemp.DefaultView;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool maximized = false;

        private void MinimizeButton_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void MaximizeButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (maximized)
            {
                this.WindowState = System.Windows.WindowState.Normal;
                maximized = false;
                return;  
            }
            this.WindowState = System.Windows.WindowState.Maximized;
            maximized = true;
        }

        private void contactsDGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            contactsDGrid.Columns[2].Width = 90;
            contactsDGrid.Columns[3].Width = 160;
        }
    }
}
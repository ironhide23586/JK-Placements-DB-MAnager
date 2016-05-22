using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace JKPlacementsDBManager
{
    /// <summary>
    /// Interaction logic for DBModifyPage.xaml
    /// </summary>
    public partial class DBModifyPage : Window
    {
        private bool logMaintain;
        private string date;
        private DataTable addedContacts = new DataTable();
        private DataTable addedPending = new DataTable();
        public static DataModder dbMod = new DataModder();
        public DBModifyPage(bool MaintainQueryLog)
        {
            InitializeComponent();
            logMaintain = MaintainQueryLog;
            dbMod.keepLog = logMaintain;
        }

        private bool fillStatus;
        private void fillTableHere2(string queryString, ref DataTable dt) //Fetches Data from server and fills into DataTable
        {
            fillStatus = MainWindow.fillTable(queryString, ref dt);
            if(fillStatus)
            {
                writeLog(queryString);
            }
            if (MainWindow.connectionObj.connState)
                MainWindow.connectionObj.CloseConnection();
        }

        private void writeLog(string queryString)
        {
            if (logMaintain)
                MainWindow.WriteToFile(MainWindow.ServerLogLocation, queryString + "\n");
        }

        private void modifyDGrid2(DataTable dt)
        {
            try
            {
                DGrid2.ItemsSource = dt.DefaultView;
            }
            catch (Exception e)
            {
            }
        }

        public void DGrid2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataRowView[] k = new DataRowView[1];
                e.AddedItems.CopyTo(k, 0);
                //InfoBoxBlock.Text = getDetails(Convert.ToInt32(k[0].Row.ItemArray[0]));

                //dbMod.selIndex = DGrid2.SelectedIndex;
                dbMod.selIndex = Convert.ToInt32(k[0].Row.ItemArray[0]);
                CompanyNameField.IsEnabled = false;
                //CompanyNameField.Text = MainWindow.company.Rows[DGrid2.SelectedIndex][1].ToString();
                MainWindow.fillTable("SELECT * FROM " + MainWindow.connectionObj.table1 + " WHERE " + MainWindow.connectionObj.table1CompanyNum + " = '" + dbMod.selIndex + "';", ref DataModder.tmpDT);
                CompanyNameField.Text = DataModder.tmpDT.Rows[0][1].ToString();
                //if (MainWindow.company.Rows[DGrid2.SelectedIndex][2].ToString() != "")
                if (DataModder.tmpDT.Rows[0][2].ToString() != "")
                    DOAField.SelectedDate = DateTime.Parse(DataModder.tmpDT.Rows[0][2].ToString());
                    //DOAField.SelectedDate = DateTime.Parse(MainWindow.company.Rows[DGrid2.SelectedIndex][2].ToString());
                else
                    DOAField.Text = "";
                //CommentField.Text = MainWindow.company.Rows[DGrid2.SelectedIndex][3].ToString();
                CommentField.Text = DataModder.tmpDT.Rows[0][3].ToString();
                //switch (MainWindow.company.Rows[DGrid2.SelectedIndex][4].ToString())
                switch (DataModder.tmpDT.Rows[0][4].ToString())
                {
                    case "Yes":
                        ResponseCBox.SelectedIndex = 0;
                        break;
                    case "No":
                        ResponseCBox.SelectedIndex = 1;
                        break;
                    case "Floating":
                        ResponseCBox.SelectedIndex = 2;
                        break;
                }
                //switch (MainWindow.company.Rows[DGrid2.SelectedIndex][5].ToString())
                switch (DataModder.tmpDT.Rows[0][5].ToString())
                {
                    case "CSE":
                        StreamCBox.SelectedIndex = 0;
                        break;
                    case "ECE":
                        StreamCBox.SelectedIndex = 1;
                        break;
                    case "CSE/ECE":
                        StreamCBox.SelectedIndex = 2;
                        break;
                }
                
                DataTable contacts = new DataTable();
                DataTable pending = new DataTable();
                DataTable contactnos = new DataTable();
                clearDataTables();
                MainWindow.fillTable("SELECT * FROM " + MainWindow.connectionObj.table2 + " WHERE " + MainWindow.connectionObj.table1CompanyName + " = '" + CompanyNameField.Text + "';", ref contacts);
                MainWindow.fillTable("SELECT * FROM " + MainWindow.connectionObj.table3 + " WHERE " + MainWindow.connectionObj.table1CompanyName + " = '" + CompanyNameField.Text + "';", ref pending);
                int i = 0;
                foreach (DataRow d in contacts.Rows)
                {
                    addedContacts.Rows[i][0] = d[2];
                    addedContacts.Rows[i][3] = d[3];
                    MainWindow.fillTable("SELECT * FROM " + MainWindow.connectionObj.table4 + " WHERE " + MainWindow.connectionObj.table4CompanyName + " = '" + CompanyNameField.Text + "' AND " + MainWindow.connectionObj.table4ContactPerson + " = '" + (string)d.ItemArray[2] + "';", ref contactnos);
                    bool flag = false;
                    foreach (DataRow d2 in contactnos.Rows) //Emails
                    {
                        if (flag)
                            addedContacts.Rows[i][1] += "; ";
                        try
                        {
                            if (d2[4].ToString().Trim() != "")
                            {
                                addedContacts.Rows[i][1] += (string)d2[4];
                                flag = true;
                            }
                            else
                                flag = false;
                        }
                        catch (InvalidCastException e3)
                        {
                        }
                    }
                    flag = false;
                    foreach (DataRow d2 in contactnos.Rows) //Phone Nos
                    {
                        if (flag)
                            addedContacts.Rows[i][2] += "; ";
                        try
                        {
                            if (d2[3].ToString().Trim() != "")
                            {
                                addedContacts.Rows[i][2] += (string)d2[3];
                                flag = true;
                            }
                            else
                                flag = false;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    i++;
                    addedContacts.Rows.Add(addedContacts.NewRow());
                }
                i = 0;
                foreach (DataRow d in pending.Rows)
                {
                    try
                    {
                        addedPending.Rows[i][0] = d[2];
                        addedPending.Rows[i][1] = DateTime.Parse(d[3].ToString().Remove(10));
                        switch (d[4].ToString())
                        {
                            case "DONE": addedPending.Rows[i][2] = true;
                                break;
                            case "PENDING": addedPending.Rows[i][2] = false;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    addedPending.Rows.Add(addedPending.NewRow());
                    i++;
                }
                ContactsModTable.ItemsSource = addedContacts.DefaultView;
                PendingModTable.ItemsSource = addedPending.DefaultView;
            }
            catch (Exception ex)
            {
            }
        }

        public void DGrid2_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd-MM-yyyy";
        }

        public void DGrid2_AutoGeneratedColumns(object sender, EventArgs e)
        {
            DGrid2.Columns[0].Header = "Sr. No.";
            DGrid2.Columns[1].Header = "Company Name";
            DGrid2.Columns[2].Header = "Date of \nApproach";
            DGrid2.Columns[4].CanUserResize = false;
            DGrid2.Columns[5].CanUserResize = false;
            DGrid2.Columns[2].MaxWidth = 75;
            DGrid2.Columns[3].Width = 200;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //fillTableHere2("SELECT * FROM " + MainWindow.connectionObj.table1 + ";", ref MainWindow.company);
            //if(fillStatus)
            //    modifyDGrid2(MainWindow.company);
            //refreshDataTables(true);
            //ContactsModTable.ItemsSource = addedContacts.DefaultView;
            //PendingModTable.ItemsSource = addedPending.DefaultView;
        }

        private string generateSQLStringFromSearchBox()
        {
            if (ModWindowCompanySearchTextBox.Text == "")
                return "SELECT * FROM " + MainWindow.connectionObj.table1 + ";";
            string cName = ModWindowCompanySearchTextBox.Text;
            DataModder.legalizeSQLString(ref cName);
            return "SELECT * FROM " + MainWindow.connectionObj.table1 + " WHERE " + MainWindow.connectionObj.table1CompanyName + " LIKE '" + ModWindowCompanySearchTextBox.Text + "%';";
        }

        private void loadDataGrid(bool val)
        {
            fillTableHere2(generateSQLStringFromSearchBox(), ref MainWindow.company);
            if (fillStatus)
                modifyDGrid2(MainWindow.company);
            refreshDataTables(val);
            ContactsModTable.ItemsSource = addedContacts.DefaultView;
            PendingModTable.ItemsSource = addedPending.DefaultView;
        }

        private void refreshDataTables(bool a)
        {
            if (a)
            {
                addedContacts.Columns.Add("Name", System.Type.GetType("System.String"));                
                addedContacts.Columns.Add("Email", System.Type.GetType("System.String"));
                addedContacts.Columns.Add("Contact No(s)", System.Type.GetType("System.String"));
                addedContacts.Columns.Add("Position", System.Type.GetType("System.String"));
                addedPending.Columns.Add("Job", System.Type.GetType("System.String"));
                addedPending.Columns.Add("Final\nDate\n(dd/mm/yyyy)", System.Type.GetType("System.DateTime"));
                addedPending.Columns.Add("Job Status", System.Type.GetType("System.Boolean"));
                addedPending.Columns[2].DefaultValue = false; 
            }
            addedPending.Rows.Add(addedPending.NewRow());
            addedContacts.Rows.Add(addedContacts.NewRow());
        }

        private string[] getDataVals()
        {
            string[] returnVals = { CompanyNameField.Text, "", CommentField.Text, ResponseCBox.SelectionBoxItem.ToString(), StreamCBox.SelectionBoxItem.ToString() };
            try
            {
                returnVals[1] = DateTime.Parse(DOAField.SelectedDate.ToString()).ToString("yyyy-MM-dd");
                //MessageBox.Show(returnVals[1]);
            }
            catch (Exception e)
            {
                returnVals[1] = "";
            }
            return returnVals;
        }

        private void AddEntryButton_Click(object sender, RoutedEventArgs e)
        {
            addedPending = ((DataView)PendingModTable.ItemsSource).ToTable();
            addedContacts = ((DataView)ContactsModTable.ItemsSource).ToTable();
            CompanyNameField.Text = CompanyNameField.Text.ToString().Trim();
            if (CompanyNameField.Text.ToString() == "")
            {
                MessageBox.Show("Please enter the Company Name!");
                return;
            }
            dbMod.insert(getDataVals(), addedContacts, addedPending, ref MainWindow.company);
            refreshDGridHere();
        }

        private void refreshDGridHere()
        {
            fillTableHere2("SELECT * FROM " + MainWindow.connectionObj.table1 + ";", ref MainWindow.company);
            modifyDGrid2(MainWindow.company);
        }

        private void ContactsModTable_AutoGeneratedColumns(object sender, EventArgs e)
        {
            ContactsModTable.Columns[0].MinWidth = 130;
            ContactsModTable.Columns[1].MinWidth = 90;
            ContactsModTable.Columns[3].MinWidth = 150;
        }

        private void AddNewContactButton_Click(object sender, RoutedEventArgs e)
        {
            addedContacts = ((DataView)ContactsModTable.ItemsSource).ToTable();
            if (addedContacts.Rows[addedContacts.Rows.Count - 1][0].ToString() == "")
                MessageBox.Show("Please add a Contact Name! :|");
            else
            {
                addedContacts.Rows.Add(addedContacts.NewRow());
                ContactsModTable.ItemsSource = addedContacts.DefaultView;
            }
        }

        private void PendingModTable_AutoGeneratedColumns(object sender, EventArgs e)
        {
            PendingModTable.Columns[0].MinWidth = 180;
            PendingModTable.Columns[1].MinWidth = 70;
            PendingModTable.Columns[1].MaxWidth = 86; 
        }

        private void AddNewPendingButton_Click(object sender, RoutedEventArgs e)
        {
            addedPending = ((DataView)PendingModTable.ItemsSource).ToTable();
            if (addedPending.Rows[addedPending.Rows.Count - 1][0].ToString() == "")
                MessageBox.Show("Please make an entry, then add a new one! >:@");
            else
            {
                addedPending.Rows.Add(addedPending.NewRow());
                PendingModTable.ItemsSource = addedPending.DefaultView;
            } 
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            File.SetAttributes(@MainWindow.ServerLogLocation, FileAttributes.ReadOnly);
            File.SetAttributes(@MainWindow.ServerLogLocation, FileAttributes.Hidden);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DeleteEntryButton_Click(object sender, RoutedEventArgs e)
        {
            if (DGrid2.SelectedIndex == -1)
            {
                MessageBox.Show("Please select something to Delete!");
                return;
            }

            //DeleteConfirmationWindow delWin = new DeleteConfirmationWindow(MainWindow.company.Rows[DGrid2.SelectedIndex][1].ToString());
            DeleteConfirmationWindow delWin = new DeleteConfirmationWindow(DataModder.tmpDT.Rows[0][1].ToString());
            this.Opacity = 0.5;
            delWin.ShowDialog();
            refreshDGridHere();
            this.Opacity = 1;
        }

        private void ModEntryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CompanyNameField.IsEnabled == false)
                {
                    dbMod.update(getDataVals(), addedContacts, addedPending, ref MainWindow.company);
                    refreshDGridHere();
                }
                else
                    MessageBox.Show("Please select an entry to be modified!", "Illegal Operation");
            }
            catch (Exception)
            {
                MessageBox.Show("Please select an entry to be modified!", "Illegal Operation");
            }
        }

        private void PendingModTable_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy";
        }

        private void DGrid2_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            deselect();
        }

        private void deselect()
        {
            DGrid2.SelectedIndex = -1;
            ContactsModTable.SelectedIndex = -1;
            PendingModTable.SelectedIndex = -1;
            CompanyNameField.IsEnabled = true;
        }

        private void DeselectButton_Click(object sender, RoutedEventArgs e)
        {
            deselect();
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            clearAll();
        }

        private void clearAll()
        {
            deselect();
            CompanyNameField.Text = "";
            DOAField.SelectedDate = null;
            CommentField.Text = "";
            ResponseCBox.SelectedIndex = 2;
            StreamCBox.SelectedIndex = 2;
            clearDataTables();
            ContactsModTable.ItemsSource = addedContacts.DefaultView;
            PendingModTable.ItemsSource = addedPending.DefaultView;
        }

        private void clearDataTables()
        {
            addedContacts.Clear();
            addedPending.Clear();
            refreshDataTables(false);
        }

        private void RemoveContactButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContactsModTable.SelectedIndex != -1)
            {
                addedContacts.Rows.RemoveAt(ContactsModTable.SelectedIndex);
                ContactsModTable.ItemsSource = addedContacts.DefaultView;
            }
            else
                MessageBox.Show("Please select the entry to be removed!", "Illegal Operation");
        }

        private void RemoveJobButton_Click(object sender, RoutedEventArgs e)
        {
            if (PendingModTable.SelectedIndex != -1)
            {
                addedPending.Rows.RemoveAt(PendingModTable.SelectedIndex);
                PendingModTable.ItemsSource = addedPending.DefaultView;
            }
            else
                MessageBox.Show("Please select the job entry to be removed!", "Illegal Operation");
        }

        private void ModWindowCompanySearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //clearAll();
            loadDataGrid(false);
            clearAll();
        }

        private void ModWindowCompanySearchTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            loadDataGrid(true);
        }

        private void ModWindowRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ModWindowCompanySearchTextBox.Text = "";
        }

        private void CloseButton2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool maximized = false;

        private void MaximizeButton2_Click(object sender, RoutedEventArgs e)
        {
            if (!maximized)
            {
                // .WindowState = System.Windows.WindowState.Maximized;
                //JKPlacementsDBManager.DBModifyPage.siz
                
                //this.w

                maximized = true;
            }
            else
            {
                //this.WindowState = System.Windows.WindowState.Normal;
                maximized = false;
            }
        }

        private void MinimizeButton2_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

    }
}
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;

namespace JKPlacementsDBManager
{
    public class DataModder
    {
        private string company = MainWindow.connectionObj.table1;
        private string contacts = MainWindow.connectionObj.table2;
        private string pending = MainWindow.connectionObj.table3;
        private string contactnos = MainWindow.connectionObj.table4;

        public static DataTable tmpDT = new DataTable();

        public int selIndex;
        public bool keepLog { get; set; }
        
        private void connInit()
        {
            if (!MainWindow.connectionObj.OpenConnection())
            {
                MessageBox.Show("Server connection Failure.  :(");
            }
        }

        private void logQuery(string s)
        {
            if (keepLog)
            {
                MainWindow.WriteToFile(MainWindow.ServerLogLocation, s);
            }
        }

        private void connDeInit()
        {
            MainWindow.connectionObj.CloseConnection();
        }

        private enum mode { INSERT, DELETE, UPDATE };

        private string queryStringGenerate(string[] args, DataTable d1, DataTable d2, mode modMode) ///////LEFT TO DESIGN
        {
            string outString = "";
            for (int i = 0; i < args.Length; i++)
            {
                legalizeSQLString(ref args[i]);
            }
            //string outString = "INSERT INTO `placements`.`company` (`CompanyName`, `DateOfApproach`, `Comments`) VALUES ('Test Entry', '1997-02-12', 'LOOLOLOLOLOLOLOLLLLLLL');";
            if (modMode == mode.INSERT)
            {
                outString += queryStringGenerateInsertTable1(args) + "\n";
                if (d1.Rows[0][0].ToString().Trim() != "")
                {
                    outString += queryStringGenerateInsertTable2(d1, args[0]);
                    outString += queryStringGenerateInsertTable4(d1, args[0]);
                }
                if (d2.Rows[0][0].ToString().Trim() != "")
                    outString += queryStringGenerateInsertTable3(d2, args[0]);
                //MessageBox.Show(outString);
                logQuery(outString);
                return outString;
            }
            else if (modMode == mode.UPDATE)
            {
                outString += queryStringGenerateUpdateTable1(args) + "\n" + queryStringGenerateDelete(args[0]) + queryStringGenerateInsertTable2(d1, args[0]) + queryStringGenerateInsertTable4(d1, args[0]) + queryStringGenerateInsertTable3(d2, args[0]);
                //MessageBox.Show(outString);
                logQuery(outString);
                return outString;
            }
            return "";
        }

        private string queryStringGenerateUpdateTable1(string[] args)
        {
            if (args[1] != "")
                return "UPDATE " + MainWindow.connectionObj.table1 + " SET `" + MainWindow.connectionObj.table1DateOfApproach + "` = '" + args[1] + "', `" + MainWindow.connectionObj.table1Comments + "` = '" + args[2] + "', `" + MainWindow.connectionObj.table1Response + "` = '" + args[3] + "', `" + MainWindow.connectionObj.table1Stream + "` = '" + args[4] + "' WHERE `" + MainWindow.connectionObj.table1CompanyName + "` = '" + args[0] + "';";

            MainWindow.fillTable("SELECT * FROM " + MainWindow.connectionObj.table1 + " WHERE " + MainWindow.connectionObj.table1CompanyNum + " = '" + selIndex + "';", ref tmpDT);
            //if (MainWindow.company.Rows[selIndex][2].ToString() == "")
            if (tmpDT.Rows[0][2].ToString() == "")
                return "UPDATE " + MainWindow.connectionObj.table1 + " SET `" + MainWindow.connectionObj.table1Comments + "` = '" + args[2] + "', `" + MainWindow.connectionObj.table1Response + "` = '" + args[3] + "', `" + MainWindow.connectionObj.table1Stream + "` = '" + args[4] + "' WHERE `" + MainWindow.connectionObj.table1CompanyName + "` = '" + args[0] + "';";

            return "UPDATE " + MainWindow.connectionObj.table1 + " SET `" + MainWindow.connectionObj.table1DateOfApproach + "` = NULL, `" + MainWindow.connectionObj.table1Comments + "` = '" + args[2] + "', `" + MainWindow.connectionObj.table1Response + "` = '" + args[3] + "', `" + MainWindow.connectionObj.table1Stream + "` = '" + args[4] + "' WHERE `" + MainWindow.connectionObj.table1CompanyName + "` = '" + args[0] + "';";
        }

        private string queryStringGenerate(string companyName, mode modMode)
        {
            string outstring = "";
            //companyName = companyName.Replace("'", "\'");
            legalizeSQLString(ref companyName);
            if (modMode == mode.DELETE)
            {
                outstring += queryStringGenerateDelete(companyName);
                outstring += "DELETE FROM " + MainWindow.connectionObj.table1 + " WHERE " + MainWindow.connectionObj.table4CompanyName + " = '" + companyName + "';\n";
            }
            logQuery(outstring);
            return outstring;
        }

        private string queryStringGenerateDelete(string companyName)
        {
            return "DELETE FROM " + MainWindow.connectionObj.table4 + " WHERE " + MainWindow.connectionObj.table4CompanyName + " = '" + companyName + "';\nDELETE FROM " + MainWindow.connectionObj.table3 + " WHERE " + MainWindow.connectionObj.table4CompanyName + " = '" + companyName + "';\nDELETE FROM " + MainWindow.connectionObj.table2 + " WHERE " + MainWindow.connectionObj.table4CompanyName + " = '" + companyName + "';\n";
        }

        public static void legalizeSQLString(ref string input)
        {
            input = input.Replace("'", "");
        }

        private string queryStringGenerateInsertTable1(string[] args)
        {
            string outString1 = "INSERT INTO " + MainWindow.connectionObj.table1 + " (`" + MainWindow.connectionObj.table1CompanyName + "`, `" + MainWindow.connectionObj.table1Comments + "`, `" + MainWindow.connectionObj.table1Response + "`, `" + MainWindow.connectionObj.table1Stream + "`";
            if (args[1] != "")
                outString1 += ", `" + MainWindow.connectionObj.table1DateOfApproach + "`";
            outString1 += ") VALUES ('" + args[0] + "', '" + args[2] + "', '" + args[3] + "', '" + args[4] + "'";
            if (args[1] != "")
                outString1 += ", '" + args[1] + "'";
            outString1 += ");";
            //MessageBox.Show(outString1);
            return outString1;
        }

        private string queryStringGenerateInsertTable2(DataTable contactsFormTable, string companyName)
        {
            string outString2 = "";
            foreach (DataRow d in contactsFormTable.Rows)
            {
                if (d[0].ToString() != "")
                {
                    outString2 += "INSERT INTO " + MainWindow.connectionObj.table2 + " (`" + MainWindow.connectionObj.table2CompanyName + "`, `" + MainWindow.connectionObj.table2ContactPerson + "`, `" + MainWindow.connectionObj.table2Position +"`) VALUES ('" + companyName + "', '" + d[0].ToString() + "', '" + d[3].ToString() + "');\n";
                }
            }
            //MessageBox.Show(outString2);
            return outString2;
        }

        private string queryStringGenerateInsertTable3(DataTable pendingFormTable, string companyName)
        {
            string outString3 = "";
            foreach (DataRow d in pendingFormTable.Rows)
            {
                if (d[0].ToString() != "")
                {
                    string jobStatus = "PENDING";
                    outString3 += "INSERT INTO " + MainWindow.connectionObj.table3 + " (`" + MainWindow.connectionObj.table3CompanyName + "`, `" + MainWindow.connectionObj.table3PendingJob + "`, `" + MainWindow.connectionObj.table3JobStatus + "`";
                    if (d[1].ToString() != "")
                        outString3 += ", `" + MainWindow.connectionObj.table3FinalDate + "`";
                    if (d[2].Equals(true))
                        jobStatus = "DONE";
                    outString3 += ") VALUES ('" + companyName + "', '" + d[0].ToString() + "', '" + jobStatus + "'";
                    if (d[1].ToString() != "")
                        outString3 += ", '" + DateTime.Parse(d[1].ToString()).ToString("yyyy-MM-dd") + "'";
                    outString3 += ");\n";
                }
            }
            //MessageBox.Show(outString3);
            return outString3;
        }

        private string queryStringGenerateInsertTable4(DataTable contactsFormTable, string companyname)
        {
            string outstring4 = "";
            foreach (DataRow d in contactsFormTable.Rows)
            {
                if (d[0].ToString() != "")
                {
                    string[] contactnos = d[2].ToString().Split(';');
                    string[] contactEmails = d[1].ToString().Split(';');
                    int i = 0;
                    foreach (string s in contactnos)
                    {
                        contactnos[i++] = s.Trim();
                    }
                    i = 0;
                    foreach (string s in contactEmails)
                    {
                        contactEmails[i++] = s.Trim();
                    }
                    i = 0;
                    //MessageBox.Show("email : " + contactEmails.Length + "\nos : " + contactnos.Length);
                    if ((contactnos.Length != 1) && (contactEmails.Length != 1))
                    {
                        for (int smaller = System.Math.Min(contactEmails.Length, contactnos.Length); i < smaller; i++)
                        {
                            outstring4 += "INSERT INTO " + MainWindow.connectionObj.table4 + " (`" + MainWindow.connectionObj.table4CompanyName + "`, `" + MainWindow.connectionObj.table4ContactPerson + "`, `" + MainWindow.connectionObj.table4ContactNum + "`, `" + MainWindow.connectionObj.table4ContactEmail + "`) VALUES ('" + companyname + "', '" + d[0].ToString() + "', '" + contactnos[i] + "', '" + contactEmails[i] + "');\n";
                        }
                    }
                    else
                    {
                        outstring4 += "INSERT INTO " + MainWindow.connectionObj.table4 + " (`" + MainWindow.connectionObj.table4CompanyName + "`, `" + MainWindow.connectionObj.table4ContactPerson + "`, `" + MainWindow.connectionObj.table4ContactNum + "`, `" + MainWindow.connectionObj.table4ContactEmail + "`) VALUES ('" + companyname + "', '" + d[0].ToString() + "', '" + contactnos[i] + "', '" + contactEmails[i] + "');\n";
                        i++;
                    }
                    if (contactEmails.Length > contactnos.Length)
                    {
                        for (; i < contactEmails.Length; i++)
                        {
                            outstring4 += "INSERT INTO " + MainWindow.connectionObj.table4 + " (`" + MainWindow.connectionObj.table4CompanyName + "`, `" + MainWindow.connectionObj.table4ContactPerson + "`, `" + MainWindow.connectionObj.table4ContactEmail + "`) VALUES ('" + companyname + "', '" + d[0].ToString() + "', '" + contactEmails[i] + "');\n";
                        }
                    }
                    else if (contactEmails.Length < contactnos.Length)
                    {
                        for (; i < contactnos.Length; i++)
                        {
                            outstring4 += "INSERT INTO " + MainWindow.connectionObj.table4 + " (`" + MainWindow.connectionObj.table4CompanyName + "`, `" + MainWindow.connectionObj.table4ContactPerson + "`, `" + MainWindow.connectionObj.table4ContactNum + "`) VALUES ('" + companyname + "', '" + d[0].ToString() + "', '" + contactnos[i] + "');\n";
                        }
                    }
                }
            }
            return outstring4;
        }

        public void insert(string[] args, DataTable d1, DataTable d2, ref DataTable dt)
        {
            executeQuery(queryStringGenerate(args, d1, d2, mode.INSERT), ref dt);
        }

        public void update(string[] args, DataTable d1, DataTable d2, ref DataTable dt)
        {
            executeQuery(queryStringGenerate(args, d1, d2, mode.UPDATE), ref dt);
        }

        public void delete(string companyName, ref DataTable dt)
        {
            executeQuery(queryStringGenerate(companyName, mode.DELETE), ref dt);
        }

        private void executeQuery(string queryString, ref DataTable dt)
        {
            connInit();
            MySqlCommand cmd = new MySqlCommand(queryString, MainWindow.connectionObj.conn);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            fillDataTable(da, ref dt);
            connDeInit();
        }

        private void fillDataTable(MySqlDataAdapter da, ref DataTable dt)
        {
            try
            {
                da.Fill(dt);
            }
            catch
            {
            }
        }
    }
}

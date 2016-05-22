using System.Windows;

namespace JKPlacementsDBManager
{
    /// <summary>
    /// Interaction logic for DeleteConfirmationWindow.xaml
    /// </summary>
    public partial class DeleteConfirmationWindow : Window
    {
        private string companyName;
        public DeleteConfirmationWindow(string cName)
        {
            InitializeComponent();
            companyName = cName;
        }

        private void DeleteYesButton_Click(object sender, RoutedEventArgs e)
        {
            DBModifyPage.dbMod.delete(companyName, ref MainWindow.company);
            this.Close();
        }

        private void DeleteNoButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

using System.Windows;

namespace JKPlacementsDBManager
{
    /// <summary>
    /// Interaction logic for ServerConnectionSettingsAuthorization.xaml
    /// </summary>
    public partial class ServerConnectionSettingsAuthorization : Window
    {
        public enum targWin { ConnSettings, DBMod };
        public targWin targetWindow = targWin.ConnSettings;
        public bool queryMaintain = false;
        public ServerConnectionSettingsAuthorization()
        {
            InitializeComponent();
        }

        private void AuthorizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordField.Password != "123placement")
                WrongPasswordLabel.Content = "Wrong Password!";
            else
            {
                this.Opacity = 0.01;
                if (targetWindow == targWin.ConnSettings)
                {
                    ServerConnectionSettings serverSettingWindow = new ServerConnectionSettings();
                    serverSettingWindow.ShowDialog();
                    this.Close();
                }
                else if (targetWindow == targWin.DBMod)
                {
                    DBModifyPage DBModder = new DBModifyPage(queryMaintain);
                    DBModder.ShowDialog();
                    this.Close();
                }
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

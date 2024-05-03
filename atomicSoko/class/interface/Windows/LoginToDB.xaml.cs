using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace atomicSoko
{
    /// <summary>
    /// Interaction logic for LoginToDB.xaml
    /// </summary>
    public partial class LoginToDB : Window
    {
        public event EventHandler? LogedIn;
        public LoginToDB()
        {
            InitializeComponent();
        }

        private async void BtnCreateConnectAccount_Click(object sender, RoutedEventArgs e)
        {
            UserModel user = new UserModel { UserName = txtAccountName.Text, Password = txtPassword.Text };
            if(UCSwitchBtn.LogInEnable)
            {
                user = await Repository.Instance.GetUser(user)!;
                if(user != null)
                {
                    LogedIn?.Invoke(user, EventArgs.Empty);
                    this.Close();
                }
                
            }
            else if(UCSwitchBtn.SignUpEnable)
            {
                await Repository.Instance.PostUser(user)!;
                LogedIn?.Invoke(user, EventArgs.Empty);
                this.Close();
            }
        }
    }
}

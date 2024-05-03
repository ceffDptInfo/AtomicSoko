using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for Shop.xaml
    /// </summary>
    public partial class Shop : Window
    {
        public User User { get; set; }

        public HubConnection Connection { get; set; }
        public Shop(User user, HubConnection connection)
        {
            InitializeComponent();
            User = user;
            Connection = connection;
            ListOfConnection();
            DisplayMoney();
        }

        public void ListOfConnection()
        {
            Connection.On<string>("ReceiveSkin", (StringForDisplay) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DropLbl.Content = StringForDisplay;
                    User.Cash -= 200;
                    DisplayMoney();
                });
            });
        }

        public async Task UpdateMoney()
        {
            User.Cash = await Repository.Instance.UpdateMoney(User);
        }

        private void RarityBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string stringSelected = RarityBox.SelectedItem.ToString()!;
            SkinView skinView = new SkinView(Rarity.GetList(stringSelected.Remove(0,38))!);
            skinView.Show();
        }

        private void RecCrate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Connection.InvokeAsync("BuyCrate", User.UserName);
        }

        private async void DisplayMoney()
        {
            await UpdateMoney();
            LblMoney.Content = $"Money : {User.Cash}";
        }
    }
}

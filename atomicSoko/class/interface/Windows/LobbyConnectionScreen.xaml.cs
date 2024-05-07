using atomicSoko;
using AtomicSokoLibrary;
using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace atomicSoko
{
    /// <summary>
    /// Interaction logic for LobbyConnectionScreen.xaml
    /// </summary>
    public partial class LobbyConnectionScreen : Window
    {
        public string? PlayerName { get; set; }
        public event EventHandler? ConnectionButtonClicked;
        public double R { get; set; } = 0;
        public double G { get; set; } = 0;
        public double B { get; set; } = 0;


        public List<string>? skins { get; set; } = new List<string>();

        public User NewUser { get; set; }

        public Color MediaColor { get; set; }

        LoginToDB? loginToDB;
        ListOfSkin? listOfSkin;


        public LobbyConnectionScreen(bool error, User user)
        {
            InitializeComponent();
            NewUser = user;
            btnConnection.IsEnabled = false;
            if (error)
            {
                lblError.Content = "Name is already used choose a other Name";
            }
            
            if (File.Exists("assets/data/Cache.txt"))
            {
                var file = new StreamReader("assets/data/Cache.txt");
                string line = file.ReadLine()!;
                if (line != null)
                {
                    lblConnect.Text = line;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PlayerName = lblName.Text;
            SaveInCache();
            ConnectionButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void lblName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (lblName.Text != "")
            {
                btnConnection.IsEnabled = true;
            }
            else
            {
                btnConnection.IsEnabled = false;
            }
        }

        private void RSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            R = e.NewValue;
            ChangeEllColor();
        }

        private void GSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            G = e.NewValue;
            ChangeEllColor();
        }

        private void BSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            B = e.NewValue;
            ChangeEllColor();
        }

        public void ChangeEllColor()
        {
            MediaColor = Color.FromRgb(byte.Parse($"{Math.Round(R)}"), byte.Parse($"{Math.Round(G)}"), byte.Parse($"{Math.Round(B)}"));
            EllipseColorPreview.Fill = new SolidColorBrush(MediaColor);
        }

        private void BtnLogIn_Click(object sender, RoutedEventArgs e)
        {
            loginToDB = new LoginToDB();
            loginToDB.LogedIn += LoginToDB_LogedIn;
            loginToDB.ShowDialog();
        }

        private void LoginToDB_LogedIn(object? sender, EventArgs e)
        {
            UserModel userModel = (UserModel)sender!;
            PlayerName = userModel.UserName;
            lblName.Text = PlayerName;
            NewUser.Cash = userModel.Cash;
            if(userModel.Skins != null)
            {
                foreach (var skin in userModel.Skins!.Values)
                {
                    skins?.Add(skin);
                }
            }
            NewUser.Admin = userModel.Admin;
        }

        private void EllipseColorPreview_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(skins != null)
            {
                listOfSkin = new ListOfSkin(skins);
                listOfSkin.Show();
                listOfSkin.selected += ListOfSkin_selected;
            }
        }

        private void ListOfSkin_selected(object? sender, EventArgs e)
        {
            string skinName = (string)sender!;
            if(skinName != null)
            {
                NewUser.SetSkin(skinName);
                recSkin.Fill = new ImageBrush(NewUser.Skin);
            }
            listOfSkin!.Close();
        }

        private void SaveInCache()
        {
            if (!Directory.Exists("assets/data"))
            {
                Directory.CreateDirectory("assets/data");
            }
            using (StreamWriter sw = new StreamWriter("assets/data/Cache.txt"))
            {
                sw.WriteLine(lblConnect.Text);
            }
        }
    }
}

using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNetCore.SignalR.Client;
using AtomicSokoLibrary;
using Drawing = System.Drawing;
using System.Net.Http;

namespace atomicSoko
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection? connection;

        public User User { get; set; }

        public List<User> users = new List<User>();
        UCAtom[,] UCGrid { get; set; }

        int width = 8;
        int height = 8;

        Model? model;

        bool firstMessages = false;
        bool isReady = false;

        List<string> chatText = new List<string>();
        

        public MainWindow()
        {
            InitializeComponent();
            User = new User();
            UCGrid = new UCAtom[width, height];
            ShowLobbyConnectionScreen();
        }

        //public

        public async void ConnectToHub()
        {
            try
            {
                connection = new HubConnectionBuilder().WithUrl("http://PC-BD52-14:5222/AtomicSokoHub").Build();
                await connection.StartAsync();
                Confirmation();
                ListOfConnection();
                connection!.Closed += MainWindow_Closed;

            }catch (Exception e)
            {
                e.ToString();
            }
            
        }

        //private

        //connection Interact

        private Task MainWindow_Closed(Exception? arg)
        {
            Environment.Exit(0);
            return Task.CompletedTask; //ne returnera jamais
        }
        private async void Confirmation()
        {
            UserData userData = new UserData();
            userData.Name = User.UserName!;
            userData.Color = User.Color.ToString();
            userData.Cash = User.Cash;
            if(User.SkinName != null)
            {
                userData.Skin = User.SkinName;
            }
            if (User.Admin)
            {
                userData.Admin = true;
            }
            await connection!.InvokeAsync("sendConnectionConfirmation", userData);
        }

        private void ListOfConnection()
        {

            connection!.On<string>("ConfirmPlayerJoinedGame", (id) =>
            {
                try
                {
                    Debug.WriteLine(id);
                    User.Id = id;
                }
                catch (Exception e)
                {
                    e.ToString();
                }
            });
            connection!.On<List<UserData>>("SendUserNameList", (UserDataList) =>
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ParseUserDataList(UserDataList);
                        UpdateLobby();
                    });
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"{e.Message}", e);
                }
            });
            connection!.On("GameInisialized", () =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    BuildGridOfUC();
                    btnStart.IsEnabled = false;
                });

            });
            connection!.On<string>("Refresh", (modelFromHub) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    model = new Model(modelFromHub, width, height);
                    RefreshUI();
                });

            });
            connection!.On<List<string>>("GetNewListOfMessages", (listString) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    chatText = listString;
                    updateChat();
                });

            });
            connection!.On("EndGame", () =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    btnStart.Content = "Ready";
                    isReady = false;
                    ResetGrid();
                });

            });
            connection!.On<string>("NameIsOk", (name) =>
            {
                User.UserName = name;
            });
            connection!.On("ConnectionDenieded", () =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    connection!.StopAsync();
                    ShowLobbyConnectionScreen(true);
                });
            });
            connection!.On<UserData>("SendUserTurn", (user) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    lblWhoHasTheTurn.Content = $"Turn of {user.Name}";
                    lblWhoHasTheTurn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(user.Color));
                });
                
            });
            connection!.On("Close", () =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Environment.Exit(0);
                });
            });
        }

        private void ParseUserDataList(List<UserData> userDatas)
        {
            users.Clear();
            foreach (UserData userData in userDatas)
            {
                User u = new User();
                u.UserName = userData.Name;
                u.Color = (Color)ColorConverter.ConvertFromString(userData.Color);
                u.Id = userData.Id;
                if(userData.Skin != null)
                {
                    u.SetSkin(userData.Skin);
                }
                users.Add(u);
            }
        }

        private void ShowLobbyConnectionScreen(bool error = false)
        {
            LobbyConnectionScreen lcs = new LobbyConnectionScreen(error, User);
            lcs.ConnectionButtonClicked += Lcs_ConnectionButtonClicked;
            lcs.ShowDialog();
        }

        private void UpdateLobby()
        {
            List<string> list = new List<string>();
            lblNbUser.Content = $"Lobby : {users.Count}";
            foreach(User u in users)
            {
                list.Add(u.UserName!);
            }
            playerListBox.ItemsSource = list;
            if (list.Count >= 2)
            {
                btnStart.IsEnabled = true;
            }
            else
            {
                btnStart.IsEnabled = false;
            }
        }

        private void BuildGridOfUC()
        {
            for (int y = 0; y < height; y++)
            {
                GrdDisplay.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int x = 0; x < width; x++)
            {
                GrdDisplay.RowDefinitions.Add(new RowDefinition());
            }
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    UCAtom atom = new UCAtom();
                    Grid.SetColumn(atom, y);
                    Grid.SetRow(atom, x);
                    UCGrid[x, y] = atom;
                    atom.MouseLeftButtonDown += Atom_MouseLeftButtonDown;
                    GrdDisplay.Children.Add(atom);
                }
            }
        }

        private void RefreshUI()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    UCGrid[x, y].DisplayEllips(model!.BackGrid[x, y], users);
                }
            }
        }

        private void updateChat()
        {
            chat.ItemsSource = chatText;
            chat.Items.MoveCurrentToLast();
            chat.ScrollIntoView(chat.Items.CurrentItem);
        }

        private void ResetGrid()
        {
            GrdDisplay.Children.Clear();
            GrdDisplay.RowDefinitions.Clear();
            GrdDisplay.ColumnDefinitions.Clear();
        }

        //interface interaction

        private void Lcs_ConnectionButtonClicked(object? sender, EventArgs e)
        {
            LobbyConnectionScreen lcs = (LobbyConnectionScreen)sender!;
            ConnectToHub();
            this.User.UserName = lcs.PlayerName;
            this.User.Color = lcs.MediaColor;
            if(User.Cash != 0)
            {
                BtnShop.IsEnabled = true;
            }
            lcs.Close();
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            isReady = !isReady;
            if (isReady) btnStart.Content = "Cancel";
            else btnStart.Content = "Ready";
            await connection!.InvokeAsync("StartGame", User.Id);
        }

        private async void Atom_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UCAtom atom = (UCAtom)sender;
            int x = Grid.GetRow(atom);
            int y = Grid.GetColumn(atom);
            await connection!.InvokeAsync("SelectAtom", x, y, User.Id);
        }

        private void btnSubmitToChat_Click(object sender, RoutedEventArgs e)
        {

            string Text = TxtInChat.Text;
            string compressedString = $"{Text}";

            connection!.InvokeAsync("SendMessageInChat", compressedString, User.UserName);
            TxtInChat.Text = string.Empty;
        }

        private void TxtInChat_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!firstMessages)
            {
                TxtInChat.Text = string.Empty;
            }
            
            firstMessages = true;
        }

        private void TxtInChat_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                btnSubmitToChat_Click(sender, e);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if(connection != null)
            {
                connection!.InvokeAsync("Disconnect", User.Id);
                connection!.StopAsync();
            }
        }

        private void BtnShop_Click(object sender, RoutedEventArgs e)
        {
            Shop shop = new Shop(User, connection!);
            shop.Show();
        }
    }
}
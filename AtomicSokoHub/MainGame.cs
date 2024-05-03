using AtomicSokoHub.Class;
using AtomicSokoHub.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Text;
using AtomicSokoLibrary;

namespace AtomicSokoHub
{
    public class MainGame
    {

        public int Width { get; set; } = 8;
        public int Height { get; set; } = 8;
        public bool GameIsRunning { get; set; } = false;
        public IHubCallerClients? Clients { get; set; }

        public static MainGame Instance = new MainGame();
        public Model? model;
        public ChatDB chatDB = new();
        public Dictionary<string, User> users = new Dictionary<string, User>();

        private string currentPlayerId = "p0";
        private AtomicConsole console = new();
        private Random rnd = new Random();

        private MainGame()
        {
            chatDB.MsgAdded += UpdateChat;
            console.Kick += ConsoleKick;
            console.SetTurnCmd += ConsoleGetTurn;
            console.Tell += ConsoleTell;
            console.ChangeUserState += ConsoleChangeUserState; 
        }

        public void Disconnect(string? id)
        {
            
            if(id != null)
            {
                if (users.ContainsKey(id))
                {
                    chatDB.PlayerLeftMsg(users[id].UserName!);
                    users.Remove(id);
                    UpdateChat(null, EventArgs.Empty);
                    EndGame(null, EventArgs.Empty);
                    UpdateUsersList();
                }
            }
            
        }
        public void AddPlayerToDictionary(UserData userData, ISingleClientProxy proxy)
        {
            User user = new();

            ChangePlayerId();

            user.Id = $"p{users.Count + 1}";
            user.UserName = userData.Name;
            user.Color = userData.Color;
            user.Skin = userData.Skin;
            user.Proxy = proxy;
            user.Cash = userData.Cash;
            user.Admin = userData.Admin;

            users.Add(user.Id, user);

            foreach(User u in users.Values)
            {
                u.Proxy!.SendAsync("ConfirmPlayerJoinedGame", u.Id);
            }

            chatDB.PlayerJoinedMsg(userData.Name);
        }

        public void UpdateUsersList()
        {
            List<UserData> userDatas = new List<UserData>();

            foreach (User user in users.Values)
            {
                UserData userData = new UserData();
                userData.Id = user.Id!;
                userData.Name = user.UserName!;
                userData.Color = user.Color!;
                userData.Skin = user.Skin!;
                userDatas.Add(userData);
            }
            foreach (User user in users.Values)
            {
                user.Proxy!.SendAsync("SendUserNameList", userDatas);

            }
        }

        public void StartGame(string id)
        {
            if (!GameIsRunning)
            {
                users[id].IsReady = !users[id].IsReady;
                int playersReady = 0;

                foreach (User user in users.Values)
                {
                    if (user.IsReady)
                    {
                        playersReady++;
                    }
                }

                if (playersReady == users.Count)
                {

                    foreach (User user in users.Values)
                    {
                        user.State = UserState.InLife;
                    }

                    LaunchGame();
                }
            }
        }

        public void SelectAndCalculateAtom(int x, int y, string id)
        {

            if (id == currentPlayerId && GameIsRunning && users[id].State == UserState.InLife)
            {
                if (model!.CheckIfCellBelongsToPlayer(x, y, currentPlayerId!))
                {
                    model!.SelectAtom(x, y, currentPlayerId!);
                }
            }
            
        }

        public void UpdateChat(object? sender, EventArgs e)
        {
            foreach(User user in users.Values)
            {
                user.Proxy!.SendAsync("GetNewListOfMessages", chatDB.Chats);
            }
        }

        public void NewChatMsg(string msg, string? userName = null)
        {
            if (userName != null && msg.StartsWith('/') && GetIdWithName(userName) != null && users[GetIdWithName(userName)!].Admin)
            {
                console.CommandReconizer(msg);
            }
            else
            {
                if(userName != null)
                {
                    msg = $"{userName} :   {msg}";
                }
                chatDB.NewMessage(msg);
            }
        }

        public void ConfirmeConnection(UserData userData, ISingleClientProxy proxy)
        {
            bool nameIsOk = true;

            foreach(User user in users.Values)
            {
                if(user.UserName == userData.Name)
                {
                    nameIsOk = false;
                }
            }

            if(nameIsOk)
            {
                proxy.SendAsync("NameIsOk", userData.Name);
                AddPlayerToDictionary(userData, proxy);
                UpdateChat(null, EventArgs.Empty);
                UpdateUsersList();
            }
            else
            {
                proxy.SendAsync("ConnectionDenieded");
            }

        }

        public async void TestAndBuyCrate(string userName)
        {
            
            if (users[GetIdWithName(userName)!] != null)
            {
                UserModel? userModel = await Repository.Instance.GetUser(userName);

                if (userModel != null && userModel.Cash >= 200)
                {
                    users[GetIdWithName(userName)!].Cash -= 200;
                    Repository.Instance.UpdateUserCash(userName, -200);

                    List<int> chances = new List<int>() { 1, 10, 200, 600, };

                    int rndChance = rnd.Next(1000);

                    int rarity = 0;
                    foreach (int chance in chances)
                    {
                        if (chance > rndChance)
                        {
                            rarity = chance;
                            break;
                        }
                    }

                    List<Skin> selections = new List<Skin>();

                    foreach (Skin skin in Skins.ListOfSkins)
                    {
                        if (skin.Rarity == rarity)
                        {
                            selections.Add(skin);
                        }
                    }
                    Skin? selectedSkin;
                    if (selections.Count == 0)
                    {
                        selectedSkin = null;
                    }
                    else
                    {
                        selectedSkin = selections[rnd.Next(selections.Count)];
                    }

                    string strRarity = "";

                    switch (selectedSkin?.Rarity)
                    {
                        case 1: strRarity = "Unic"; break;
                        case 10: strRarity = "Legendary"; break;
                        case 200: strRarity = "Rare"; break;
                        case 600: strRarity = "Common"; break;
                    }

                    if (selectedSkin != null)
                    {
                        Repository.Instance.UpdateUserSkins(userName, selectedSkin.Name);
                        await users[GetIdWithName(userName)!].Proxy!.SendAsync("ReceiveSkin", $"{strRarity}:{selectedSkin.Name}");
                    }
                    else
                    {
                        await users[GetIdWithName(userName)!].Proxy!.SendAsync("ReceiveSkin", $"None");
                    }

                    UpdateUsersList();
                }
            }
        }

        //Private Functions

        private void ChangePlayerTurn()
        {
            int playerTurnAsInt = 0;
            foreach (char c in currentPlayerId)
            {
                int.TryParse(c.ToString(), out playerTurnAsInt);
            }
            playerTurnAsInt++;
            if (playerTurnAsInt <= users.Count)
            {
                currentPlayerId = $"p{playerTurnAsInt}";
            }
            else
            {
                currentPlayerId = "p1";
            }

            if (users[currentPlayerId].State == UserState.Dead)
            {
                ChangePlayerTurn();
            }
        }

        private void LaunchGame()
        {
            model = new Model(Width, Height);
            model!.Atomsetted += SetAtoms;
            model!.AtomExploded += RefreshAtoms;
            model!.AtomsDestroyed += EndGame;


            GameIsRunning = true;
            currentPlayerId = "p0";

            Clients!.All.SendAsync("GameInisialized");
            RefreshAtoms(null, EventArgs.Empty);
            ChangePlayerTurn();
            SendUserTurn();
        }

        private void SendUserTurn()
        {
            UserData userData = new UserData();
            userData.Id = users[currentPlayerId].Id!;
            userData.Name = users[currentPlayerId].UserName!;
            userData.Color = users[currentPlayerId].Color!;
            Clients!.All.SendAsync("SendUserTurn", userData);
        }

        private void RefreshAtoms(object? sender, EventArgs e)
        {
            Clients!.All.SendAsync("Refresh", CompressTableToString(model!.Cells));
        }

        private void SetAtoms(object? sender, EventArgs e)
        {
            Clients!.All.SendAsync("Refresh", CompressTableToString(model!.Cells));
            ChangePlayerTurn();
            SendUserTurn();
        }

        private string CompressTableToString(Cell[,] table)
        {
            StringBuilder finalString = new StringBuilder();

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    string str = "";

                    if (table[j, i].isLastSelected)
                    {
                        str = $"{table[j, i].Player},{table[j, i].Value},_;";
                    }
                    else
                    {
                        str = $"{table[j, i].Player},{table[j, i].Value};";
                    }

                    finalString.Append(str);
                }
                finalString.Append('\n');
            }

            return finalString.ToString();
        }

        private void WinnerReward(string playerId)
        {
            int nbPlayer = 0;
            foreach(User user in users.Values)
            {
                if(user.State == UserState.Dead || user.State == UserState.InLife)
                {
                    nbPlayer++;
                }
            }

            Int64 cash = rnd.Next((nbPlayer * 10) / 2, nbPlayer * 10);

            users[playerId].Cash = cash;
            Repository.Instance.UpdateUserCash(users[playerId].UserName!, users[playerId].Cash);
        }

        private void EndGame(object? sender, EventArgs e)
        {
            string? player = sender as string;
            if (player != null)
            {
                WinnerReward(player);

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("-----------------------------------------\n");
                stringBuilder.Append($"{users[player].UserName} Won the GAME!!\n");
                stringBuilder.Append("-----------------------------------------\n");

                NewChatMsg($"{stringBuilder}");
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("-----------------------------------------\n");
                stringBuilder.Append($"Game has been Canceled on Twitté (X)\n");
                stringBuilder.Append("-----------------------------------------\n");

                NewChatMsg($"{stringBuilder}");
            }

            GameIsRunning = false;
            foreach (User user in users.Values)
            {
                user.IsReady = false;
                user.State = UserState.Spectator;
            }

            UpdateUsersList();
            Clients!.All.SendAsync("EndGame");
        }

        private void ChangePlayerId()
        {
            Dictionary<string, User> newUserList = new Dictionary<string, User>();

            int count = 1;
            foreach (KeyValuePair<string, User> entry in users)
            {
                newUserList[$"p{count}"] = entry.Value;
                entry.Value.Id = $"p{count}";
                count++;
            }
            users = newUserList;
        }

        private string? GetIdWithName(string userName)
        {
            foreach(User user in users.Values)
            {
                if(user.UserName == userName)
                {
                    return user.Id;
                }
            }

            return null;
        }

        //Console Methodes

        private void ConsoleKick(object? sender, EventArgs e)
        {
            string[]? cmd = sender as string[];
            if(cmd != null && cmd.Length == 2)
            {
                string? id = GetIdWithName(cmd[1]);
                if(id != null)
                {
                    users[id].Proxy!.SendAsync("Close");
                    Disconnect(users[id].Id);
                }                
            }
        }

        private void ConsoleGetTurn(object? sender, EventArgs e)
        {
            string[]? cmd = sender as string[];
            
            if(cmd != null && cmd.Length == 2)
            {
                string? id = GetIdWithName(cmd[1]);
                if(id != null)
                {
                    currentPlayerId = id;
                    SendUserTurn();
                    NewChatMsg("Console : You Have Been ADMINED !!!");
                }
            }
        }

        private void ConsoleTell(object? sender, EventArgs e)
        {
            string[]? cmd = sender as string[];

            if(cmd != null && cmd.Length == 2)
            {
                bool isInit = true;
                string msg = "";
                foreach (string str in cmd)
                {
                    if (!isInit)
                    {
                        msg += " ";
                        msg += str;
                    }
                    isInit = false;
                }

                NewChatMsg(msg, "Server Announcement");
                UpdateChat(null, EventArgs.Empty);

            }
        }

        private void ConsoleChangeUserState(object? sender, EventArgs e)
        {
            string[]? cmd = sender as string[];

            if(cmd != null && cmd.Length == 3)
            {
                foreach (User user in users.Values)
                {
                    if (cmd[1] == user.UserName)
                    {
                        if (cmd[2] == "Spectator")
                        {
                            user.State = UserState.Spectator;
                        }
                        else if (cmd[2] == "Dead")
                        {
                            user.State |= UserState.Dead;
                        }
                        else if (cmd[2] == "InLife")
                        {
                            user.State = UserState.InLife;
                        }

                        if(user.Id == currentPlayerId)
                        {
                            ChangePlayerTurn();
                            SendUserTurn();
                        }
                    }
                }
            }
        }
    }
}

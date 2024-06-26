﻿using Microsoft.AspNetCore.SignalR;
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
        public int Round { get; set; } = 1;

        public static MainGame Instance = new MainGame();
        public Model? model;
        public ChatDB chatDB = new();
        public Dictionary<string, User> users = new Dictionary<string, User>();
        public string currentPlayerId = "p0";

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
                    users[id].EmptyUser();
                    UpdateChat(null, EventArgs.Empty);
                    UpdateUsersList();
                    if (id == currentPlayerId)
                    {
                        ChangePlayerTurn();
                        SendUserTurn();
                    }
                    int nbConnected = 0;
                    foreach(User user in users.Values)
                    {
                        if(user.IsConnected)
                        {
                            nbConnected++;
                        }
                    }
                    if (nbConnected <= 1)
                    {
                        EndGame(null, EventArgs.Empty);
                    }
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
                userData.PowerUp = user.PowerUp;
                userDatas.Add(userData);
            }
            foreach (User user in users.Values)
            {
                if (user.IsConnected)
                {
                    user.Proxy!.SendAsync("SendUserNameList", userDatas);
                }
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
                        user.IsReady = false;
                        user.CashMultipliers = 1;
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
                if (user.IsConnected)
                {
                    user.Proxy!.SendAsync("GetNewListOfMessages", chatDB.Chats);
                }
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

        public void ControlAndUsePowerUp(int x, int y, string id)
        {
            if(id == currentPlayerId && Round > 1)
            {
                switch (users[id].PowerUp)
                {
                    case PowerUps.CellThief: model!.CellThief(x, y, id); break;
                    case PowerUps.WallDestroyer: model!.WallDestroyer(x, y); break;
                    case PowerUps.CashDoubler: CashDoubler(id); break;
                    case PowerUps.NeutralNuke: model!.PlantNeutralNuke(x, y); break;
                    case PowerUps.Angelica: model!.SetAngelica(x, y, Round); break;
                    case PowerUps.ElJutos: model!.SetElJutos(x, y, id); break;
                    case PowerUps.Topico: model!.SetTopico(id); break;
                }
            }
        }

        //Private Functions

        private void CashDoubler(string id)
        {
            users[id].CashMultipliers++;
            PowerUpUsed("CashDoubler", EventArgs.Empty);
            SetAtoms(null, EventArgs.Empty);
        }

        private void RemoveDisconnectedUsers()
        {
            List<string> usersToRemove = new List<string>();
            foreach(User user in users.Values)
            {
                if(!user.IsConnected)
                {
                    usersToRemove.Add(user.Id!);
                }
            }

            foreach(string user in usersToRemove)
            {
                users.Remove(user);
            }
        }

        private void ChangePlayerTurn()
        {
            if(GameIsRunning && TestIfUserInLifeLeft())
            {

                List<string> keys = users.Keys.ToList();

                int playerTurnAsInt = 0;
                int.TryParse(currentPlayerId.Remove(0, 1), out playerTurnAsInt);



                if (playerTurnAsInt >= keys.Count)
                {
                    playerTurnAsInt = 0;
                    NewRoundProcess();
                }

                currentPlayerId = keys[playerTurnAsInt];


                if (!users.TryGetValue(currentPlayerId, out User? user) || users[currentPlayerId].State != UserState.InLife || !users[currentPlayerId].IsConnected)
                {
                    ChangePlayerTurn();
                }
            }
        }

        private void NewRoundProcess()
        {
            Round++;
            if (Round % 10 == 0)
            {
                SetPowerUp();
            }
            model!.AngelicaRoundTester(Round);
            RefreshAtoms(null, EventArgs.Empty);
            UpdateRound();
        }

        private void UpdateRound()
        {
            foreach (User user in users.Values)
            {
                if (user.IsConnected)
                {
                    user.Proxy!.SendAsync("UpdateRound", Round);
                }
            }
        }

        private void LaunchGame()
        {
            model = new Model(Width, Height);
            model!.Atomsetted += SetAtoms;
            model!.AtomExploded += RefreshAtoms;
            model!.AtomsDestroyed += EndGame;
            model!.PowerUpUsed += PowerUpUsed;

            GameIsRunning = true;
            Round = 1;
            currentPlayerId = "p0";

            SetPowerUp();

            Clients!.All.SendAsync("GameInisialized");
            RefreshAtoms(null, EventArgs.Empty);
            UpdateRound();
            ChangePlayerTurn();
            UpdateUsersList();
            SendUserTurn();
        }

        private void PowerUpUsed(object? sender, EventArgs e)
        {
            string? powerUp = sender as string;
            if(powerUp != null)
            {
                users[currentPlayerId].PowerUp = PowerUps.None;
                UpdateUsersList();
                chatDB.PowerUpMsg(users[currentPlayerId].UserName!, powerUp);
                UpdateChat(null, EventArgs.Empty);
            }
        }

        private void SetPowerUp()
        {
            List<PowerUps> powerUps = Enum.GetValues(typeof(PowerUps)).Cast<PowerUps>().ToList();
            
            foreach(User user in users.Values)
            {
                PowerUps p = powerUps[rnd.Next(powerUps.Count - 1)];
                user.PowerUp = p;
            }
            UpdateUsersList();
        }

        private bool TestIfUserInLifeLeft()
        {
            bool usersLeft = false;
            foreach(User user in users.Values)
            {
                if(user.State == UserState.InLife)
                {
                    usersLeft = true;
                }
            }
            return usersLeft;
        }

        private void SendUserTurn()
        {
            if(users.TryGetValue(currentPlayerId, out User? user) && user != null)
            {
                UserData userData = new UserData();
                userData.Id = users[currentPlayerId].Id!;
                userData.Name = users[currentPlayerId].UserName!;
                userData.Color = users[currentPlayerId].Color!;

                foreach(User u in users.Values)
                {
                    if (u.IsConnected)
                    {
                        u.Proxy!.SendAsync("SendUserTurn", userData);
                    }
                }
            }
        }

        private void RefreshAtoms(object? sender, EventArgs e)
        {
            Clients!.All.SendAsync("Refresh", CompressTableToString(model!.Cells));
        }

        private void SetAtoms(object? sender, EventArgs e)
        {
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

                    if (table[j, i].Buff != ' ')
                    {
                        str = $"{table[j, i].Player},{table[j, i].Value},{table[j, i].Buff};";
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

            users[playerId].Cash = cash * users[playerId].CashMultipliers;
            
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
                user.State = UserState.Spectator;
            }

            RemoveDisconnectedUsers();
            UpdateUsersList();
            foreach (User u in users.Values)
            {
                if (u.IsConnected)
                {
                    u.Proxy!.SendAsync("EndGame");
                }
            }
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

            if(cmd != null && cmd.Length >= 2)
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

using AtomicSokoLibrary;
using Microsoft.AspNetCore.SignalR;

namespace AtomicSokoHub
{
    public class GameHub : Hub
    {

        MainGame mainGame = MainGame.Instance;


        public void SendConnectionConfirmation(UserData userData)
        {
            Console.WriteLine($"Player {userData.Name} is connected");
            mainGame.Clients = Clients;
            mainGame.ConfirmeConnection(userData, Clients.Caller);
        }

        public void StartGame(string id)
        {
            mainGame.Clients = Clients;
            mainGame.StartGame(id);
        }

        public void SelectAtom(int x, int y, string id)
        {
            mainGame.Clients = Clients;
            mainGame.SelectAndCalculateAtom(x, y, id);
        }

        public void SendMessageInChat(string chat, string userName)
        {
            mainGame.Clients = Clients;
            mainGame.NewChatMsg(chat, userName);
        }

        public void Disconnect(string id)
        {
            mainGame.Disconnect(id);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public void BuyCrate(string userName)
        {
            mainGame.Clients = Clients;
            mainGame.TestAndBuyCrate(userName);
        }
    }
}

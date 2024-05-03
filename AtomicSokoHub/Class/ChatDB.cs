namespace AtomicSokoHub.Class
{
    public class ChatDB
    {
        public List<string> Chats { get; set; }

        public event EventHandler? MsgAdded;

        public ChatDB()
        {
            Chats = new List<string>();
        }

        public void NewMessage(string msg)
        {
            if(Chats.Count() > 30)
            {
                Chats.Remove(Chats.First());
            }
            Chats.Add(msg);
            MsgAdded?.Invoke(Chats, new EventArgs());
        }

        public void PlayerJoinedMsg(string userName)
        {
            NewMessage($"{userName} has joined the Game!");
        }

        public void PlayerLeftMsg(string userName)
        {
            NewMessage($"{userName} has left the Game!");
        }
    }
}

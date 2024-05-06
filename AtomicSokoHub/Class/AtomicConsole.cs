namespace AtomicSokoHub
{
    public class AtomicConsole
    {

        public event EventHandler? Kick;
        public event EventHandler? SetTurnCmd;
        public event EventHandler? Tell;
        public event EventHandler? ChangeUserState;

        public void CommandReconizer(string msg)
        {
            msg = msg.Remove(0,1);

            string[] cmd = msg.Split(' ');

            switch (cmd[0])
            {
                case "kick": case "k":
                    Kick?.Invoke(cmd, EventArgs.Empty);
                    break;
                case "getturn": case "gt":
                    SetTurnCmd?.Invoke(cmd, EventArgs.Empty);
                    break;
                case "tell": case "t":
                    Tell?.Invoke(cmd, EventArgs.Empty);
                    break;
                case "state": case "s":
                    ChangeUserState?.Invoke(cmd, EventArgs.Empty);
                    break;
            }
        }
    }
}

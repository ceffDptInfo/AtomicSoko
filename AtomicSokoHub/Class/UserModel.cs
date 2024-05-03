namespace AtomicSokoHub.Class
{
    public class UserModel
    {
        public string UserName { get; set; } = string.Empty;
        public Int64 Cash { get; set; } = 0;

        public Dictionary<string, string>? Skins { get; set; }

    }
}

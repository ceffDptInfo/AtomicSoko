namespace AtomicSokoHub
{
    public class Skin
    {
        public string Name { get; set; } = "";
        public int Rarity { get; set; } = 0;

        public Skin(string name, int rarity)
        {
            Name = name;
            Rarity = rarity;
        }
    }
}

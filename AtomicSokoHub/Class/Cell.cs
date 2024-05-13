namespace AtomicSokoHub
{
    public class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Value { get; set; }
        public char Player { get; set; } = ' ';
        public char Buff { get; set; } = ' ';
        public int Round { get; set; } = 0;
    }
}

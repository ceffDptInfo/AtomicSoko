namespace AtomicSokoHub.Class
{
    public class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Value { get; set; }
        public char Player { get; set; } = ' ';
        public bool isLastSelected { get; set; } = false;
    }
}

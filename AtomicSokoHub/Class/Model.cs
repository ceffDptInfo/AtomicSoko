using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;

namespace AtomicSokoHub
{
    //ABC abc " "
    public class Model
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Cell[,] Cells {  get; set; }

        public event EventHandler? Atomsetted;
        public event EventHandler? AtomExploded;
        public event EventHandler? AtomsDestroyed;
        public event EventHandler? PowerUpUsed;

        private bool enemyCellsLeft = true;
        private Random rnd = new Random();
        private List<Cell> angelicaCells = new List<Cell>();
        public Model(int width, int height)
        {

            Cells = new Cell[width, height];
            Width = width;
            Height = height;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Cell cell = new Cell();
                    cell.X = i;
                    cell.Y = j;
                    cell.Value = ' ';
                    Cells[i, j] = cell;
                }
            }

            GenerateSpecialCells();

        }

        public void SelectAtom(int x, int y, string currentPlayer)
        {
            if (!CellValueIsUpper(x, y) && Cells[x, y].Buff != '#')
            {
                AddAtomSetColor(x, y, currentPlayer);
                ResetSelection();
                Cells[x, y].Buff = '-';
                AtomicAlgorithm(currentPlayer);
                Atomsetted?.Invoke(this, EventArgs.Empty);
            }
        }

        public void CellThief(int x, int y, string id)
        {
            Cell cell = Cells[x, y];
            if(!CheckIfCellBelongsToPlayer(x, y, id) && char.IsLower(cell.Value) && cell.Value != ' ' && CellThiefEnougnAtomsAnalyser(cell))
            {
                cell.Player = char.Parse(id.Remove(0, 1));
                PowerUpUsed?.Invoke(this, EventArgs.Empty);
                Atomsetted?.Invoke(this, EventArgs.Empty);
                AtomExploded?.Invoke(this, EventArgs.Empty);
            }
        }

        public void WallDestroyer(int x, int y)
        {
            Cell cell = Cells[x, y];
            if (cell.Value == 'B')
            {
                cell.Value = ' ';
                PowerUpUsed?.Invoke(this, EventArgs.Empty);
                Atomsetted?.Invoke(this, EventArgs.Empty);
                AtomExploded?.Invoke(this, EventArgs.Empty);
            }
        }

        public void PlantNeutralNuke(int x, int y)
        {
            Cell cell = Cells[x, y];
            if(cell.Value == ' ')
            {
                cell.Value = (char)('a' + (GetCriticalMass(x, y) - 2));
                cell.Player = '0';
                PowerUpUsed?.Invoke(this, EventArgs.Empty);
                Atomsetted?.Invoke(this, EventArgs.Empty);
                AtomExploded?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetAngelica(int x, int y, int round)
        {
            Cell cell = Cells[x, y];

            if(cell.Value != ' ' && !CellValueIsUpper(x, y))
            {
                cell.Buff = '#';
                cell.Round = round;
                angelicaCells.Add(cell);
                PowerUpUsed?.Invoke(this, EventArgs.Empty);
                Atomsetted?.Invoke(this, EventArgs.Empty);
                AtomExploded?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetElJutos(int x, int y, string id)
        {
            Cell cell = Cells[x, y];

            if (cell.Value == 'A')
            {
                List<Cell> neighbors = GetNeighbors(x, y);
                foreach(Cell neighbor in neighbors)
                {
                    AddAtomSetColor(neighbor.X, neighbor.Y, id);
                    AtomicAlgorithm(id);
                }
                PowerUpUsed?.Invoke(this, EventArgs.Empty);
                Atomsetted?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetTopico(string id)
        {
            foreach(Cell cell in Cells)
            {
                int x = cell.X;
                int y = cell.Y;
                if(x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                {
                    cell.Value = ' ';
                    cell.Player = ' ';
                    AtomExploded?.Invoke(this, EventArgs.Empty);
                }
            }
            TestIfAtomsleft();
            PowerUpUsed?.Invoke(this, EventArgs.Empty);
            Atomsetted?.Invoke(this, EventArgs.Empty);
        }

        public bool CheckIfCellBelongsToPlayer(int x, int y, string currentPlayer)
        {
            bool isBelongingToPlayer = false;

            if (currentPlayer == $"p{Cells[x, y].Player}" || Cells[x, y].Value == ' ' || Cells[x, y].Player == '0')
            {
                isBelongingToPlayer = true;
            }

            return isBelongingToPlayer;
        }

        public void AngelicaRoundTester(int round)
        {
            List<Cell> delete = new List<Cell>();
            foreach (Cell cell in angelicaCells)
            {
                if (round - cell.Round == 3)
                {
                    cell.Round = 0;
                    cell.Buff = ' ';
                    delete.Add(cell);
                }
            }
            if (delete.Count > 0)
            {
                foreach (Cell cell in delete)
                {
                    angelicaCells.Remove(cell);
                }
            }
        }

        private void GenerateSpecialCells()
        {
            double totalCells = Width * Height;

            for (int i = 0; i < Math.Round(totalCells / 10 - 0.5); i++)
            {
                int specialCell = rnd.Next(3);

                PlaceSpecialCell(specialCell);
            }
            CheckSpecialCellPlacement();
        }

        private void PlaceSpecialCell(int specialCell)
        {
            int x = rnd.Next(1, Width - 1);
            int y = rnd.Next(1, Height- 1);

            if(specialCell == 0)
            {
                Cells[x, y].Value = 'A';
            }
            else
            {
                Cells[x, y].Value = 'B';
            }
        }

        private bool CellThiefEnougnAtomsAnalyser(Cell cell)
        {
            int atomsCount = 0;
            foreach(Cell c in Cells)
            {
                if(c.Player == cell.Player)
                {
                    atomsCount++;
                }
            }

            return atomsCount > 1;
        }

        private void CheckSpecialCellPlacement()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {

                    int nbWalls = 0;

                    for(int i = -1; i <= 1; i++)
                    {
                        for(int j = -1; j <= 1; j++)
                        {
                            int dx = j + x;
                            int dy = i + y;

                            if(dx >= 0 && dy >= 0 && dx < Width && dy < Height)
                            {
                                if (Cells[dx, dy].Value == 'B')
                                {
                                    nbWalls++;
                                }
                            }
                        }
                    }

                    if(nbWalls > 2)
                    {
                        Cells[x, y].Value = ' ';
                    }
                }
            }
        }

        private bool CellValueIsUpper(int x, int y)
        {
            bool isUpper = false;

            if (char.IsUpper(Cells[x, y].Value) && Cells[x, y].Value != ' ')
            {
                isUpper = true;
            }

            return isUpper;
        }

        private void ResetSelection()
        {
            foreach (var cell in Cells)
            {
                if(cell.Buff == '-')
                {
                    cell.Buff = ' ';
                }
            }
        }

        private void AddAtomSetColor(int x, int y, string currentPlayer)
        {

            if(!CellValueIsUpper(x, y))
            {
                if (Cells[x, y].Value == ' ')
                {
                    Cells[x, y].Value = 'a';
                }
                else
                {
                    Cells[x, y].Value = (char)(Cells[x, y].Value + 1);
                }

                int playerNumber = 0;

                foreach (char c in currentPlayer)
                {
                    int.TryParse(c.ToString(), out playerNumber);
                }
                Cells[x, y].Player = char.Parse($"{playerNumber}");
            }
        }

        private void AtomicAlgorithm(string currentPlayer)
        {
            bool explosionHappend = false;
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    explosionHappend = AtomicAlgorithmCalculation(x, y, currentPlayer, explosionHappend);
                }
            }

            if (explosionHappend)
            {
                AtomicAlgorithm(currentPlayer);
            }
        }

        private bool AtomicAlgorithmCalculation(int x, int y, string currentPlayer, bool explosionState)
        {
            bool explosion = explosionState;
            if (GetAtomMass(Cells[x, y].Value) >= GetCriticalMass(x, y))
            {
                List<Cell> neighbors = GetNeighbors(x, y);

                explosion = true;

                if (!CellValueIsUpper(x, y))
                {
                    Cells[x, y].Value = ' ';
                }
                foreach (Cell neighbor in neighbors)
                {
                    if(neighbor.Buff != '#')
                    {
                        AddAtomSetColor(neighbor.X, neighbor.Y, currentPlayer);
                        AtomExploded?.Invoke(this, EventArgs.Empty);
                    }
                }
                if (enemyCellsLeft)
                {
                    TestIfAtomsleft();
                }
            }
            else
            {
                AtomExploded?.Invoke(null, EventArgs.Empty);
            }

            return explosion;
        }

        private int GetAtomMass(char arg)
        {
            int value = 0;

            switch (arg)
            {
                case 'a': value = 1; break;
                case 'b': value = 2; break;
                case 'c': value = 3; break;
                case 'd': value = 4; break;
                case 'e': value = 5; break;
                case 'f': value = 6; break;
                case 'g': value = 7; break;
            }

            return value;
        }

        private int GetCriticalMass(int x, int y)
        {
            return GetNeighbors(x, y).Count();
        }

        private List<Cell> GetNeighbors(int x, int y)
        {
            List<Cell> neighbors = new List<Cell>();
            int[,] neighborsPos = { {x, y - 1}, {x - 1, y}, {x + 1, y}, {x, y + 1} };

            for (int i = 0; i < 4; i++)
            {
                int neighborsX = neighborsPos[i, 0];
                int neighborsY = neighborsPos[i, 1];
                if(neighborsX >= 0 && neighborsY >= 0 && neighborsX < Width && neighborsY < Height)
                {
                    if (Cells[neighborsX, neighborsY].Value != 'B')
                    {
                        neighbors.Add(Cells[neighborsPos[i, 0], neighborsPos[i, 1]]);
                    }
                }
            }

            return neighbors;
        }

        private void TestIfAtomsleft()
        {
            List<string> deadUsers = MainGame.Instance.users.Keys.ToList<string>();
            List<string> usersInLife = new List<string>();
            foreach (Cell cell in Cells)
            {
                if(cell.Value != ' ')
                {
                    foreach(string user in deadUsers)
                    {
                        if ($"p{cell.Player}" == user)
                        {
                            usersInLife.Add(user);
                            deadUsers.Remove($"p{cell.Player}");
                            break;
                        }
                    }
                }
            }

            foreach(string user in deadUsers)
            {
                MainGame.Instance.users[user].State = UserState.Dead;
            }

            if (usersInLife.Count == 1)
            {
                AtomsDestroyed?.Invoke(usersInLife.First<string>(), EventArgs.Empty);
                enemyCellsLeft = false;
            }
        }
    }
}

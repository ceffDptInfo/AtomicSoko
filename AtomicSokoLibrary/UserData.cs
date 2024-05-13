using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtomicSokoLibrary
{

    public enum PowerUps
    {
        CellThief,
        WallDestroyer,
        CashDoubler,
        NeutralNuke,
        Topico,
        ElJutos,
        Angelica,
        None,
    }

    public class UserData
    {
        public string Color { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Id { get; set; } = string.Empty;

        public string? Skin { get; set; }

        public bool Admin { get; set; } = false;

        public Int64 Cash { get; set; } = 0;

        public PowerUps PowerUp { get; set; } = PowerUps.None;

    }
}

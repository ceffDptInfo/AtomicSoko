using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace atomicSoko
{
    public class UserModel
    {
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public Int64 Cash { get; set; } = 0;

        public Dictionary<string, string>? Skins { get; set; }

        public bool Admin { get; set; } = false;
    }
}

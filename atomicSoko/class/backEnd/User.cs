using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace atomicSoko 
{
    public class User
    {
        public string? UserName { get; set; }

        public string? Id { get; set; }

        public Color Color { get; set; }

        public Int64 Cash { get; set; } = 0;

        public BitmapImage? Skin { get; set; }

        public string? SkinName { get; set; }

        public bool Admin { get; set; } = false;

        public User() { }
        public User(UserModel model)
        {
            UserName = model.UserName;
        }

        public void SetSkin(string skinName)
        {
            Skin = new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/{skinName}.png"));
            SkinName = skinName;
        }
    }
}

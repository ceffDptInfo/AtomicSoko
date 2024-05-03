using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace atomicSoko
{
    static class Rarity
    {
        static ImageBrush Amogus = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Amogus.png")));
        static ImageBrush Athena = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Athena.png")));
        static ImageBrush Atomic = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Atomic.png")));
        static ImageBrush Beer = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Beer.png")));
        static ImageBrush Damed = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Damed.png")));
        static ImageBrush DiamondPickaxe = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/DiamondPickaxe.png")));
        static ImageBrush Dragon = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Dragon.png")));
        static ImageBrush HeartToAmerica = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/HeartToAmerica.png")));
        static ImageBrush HeartToGermany = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/HeartToGermany.png")));
        static ImageBrush HeartToSwitzerland = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/HeartToSwitzerland.png")));
        static ImageBrush Phoenix = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Phoenix.png")));
        static ImageBrush Portal = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Portal.png")));
        static ImageBrush Reaper = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Reaper.png")));
        static ImageBrush Scooter = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Scooter.png")));
        static ImageBrush Skull = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Skull.png")));
        static ImageBrush Viking = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Viking.png")));
        static ImageBrush Visqueuse = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Visqueuse.png")));

        static List<ImageBrush> Commons = new List<ImageBrush>() {HeartToAmerica, HeartToGermany, HeartToSwitzerland, Skull, Scooter};
        static List<ImageBrush> Rares = new List<ImageBrush>() {Amogus, Beer, Viking, Atomic, DiamondPickaxe};
        static List<ImageBrush> Legendarys = new List<ImageBrush>() {Dragon, Reaper, Visqueuse, Athena};
        static List<ImageBrush> Unics = new List<ImageBrush>() {Portal, Phoenix, Damed};

        static public List<ImageBrush>? GetList(string type)
        {
            switch(type)
            {
                case "Common":
                    return Commons;
                case "Rare":
                    return Rares;
                case "Legendary":
                    return Legendarys;
                case "Unic":
                    return Unics;
            }
            return null;
        }
    }
}

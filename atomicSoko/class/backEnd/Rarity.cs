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
        static ImageBrush DarkVador = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/DarkVador.png")));
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
        static ImageBrush Discord = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Discord.png")));
        static ImageBrush EyeOfCthulhu = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/EyeOfCthulhu.png")));
        static ImageBrush Godzilla = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Godzilla.png")));
        static ImageBrush Golum = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Golum.png")));
        static ImageBrush KawaiiPikachu = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/KawaiiPikachu.png")));
        static ImageBrush KennyMCWhale = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/KennyMCWhale.png")));
        static ImageBrush LetMeSoloHer = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/LetMeSoloHer.png")));
        static ImageBrush Marshadow = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Marshadow.png")));
        static ImageBrush MewTwo = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/MewTwo.png")));
        static ImageBrush Pay2Win = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Pay2Win.png")));
        static ImageBrush Pizza = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Pizza.png")));
        static ImageBrush Reyna = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Reyna.png")));
        static ImageBrush Ronaldo = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/skins/Ronaldo.png")));

        static List<ImageBrush> Commons = new List<ImageBrush>() {HeartToAmerica, HeartToGermany, HeartToSwitzerland, Skull, Scooter, Discord, Pizza , Ronaldo };
        static List<ImageBrush> Rares = new List<ImageBrush>() {Amogus, Beer, Viking, Atomic, DiamondPickaxe, KawaiiPikachu, KennyMCWhale , Pay2Win };
        static List<ImageBrush> Legendarys = new List<ImageBrush>() {Dragon, Reaper, Visqueuse, Athena, EyeOfCthulhu, Godzilla, Golum, Marshadow , MewTwo };
        static List<ImageBrush> Unics = new List<ImageBrush>() {Phoenix, Portal, Damed, DarkVador, LetMeSoloHer, Reyna };

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

using AtomicSokoLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace atomicSoko
{
    /// <summary>
    /// Interaction logic for PowerUpDescription.xaml
    /// </summary>
    public partial class PowerUpDescription : Window
    {
        Dictionary<PowerUps, string> Description = new Dictionary<PowerUps, string>();
        public PowerUpDescription(PowerUps p)
        {
            InitializeComponent();
            InitDictionary();
            LblDescription.Content = Description[p];
        }

        private void InitDictionary()
        {
            Description.Add(PowerUps.CellThief, "Vole la cellule d'un adversaire sans rajouter de cellule");
            Description.Add(PowerUps.WallDestroyer, "Casse un mur");
            Description.Add(PowerUps.CashDoubler, "Chaque fois utiliser durant la partie vous augmentera votre multiplicateur pour la fin de la partie si vous gagnez");
            Description.Add(PowerUps.NeutralNuke, "Vous faite apparaître des cellules neutres que tout le monde peut faire exploser --> les cellules sont a une cellule d'exploser");
            Description.Add(PowerUps.Angelica, "Protège votre ou la cellule cliqué pour 3 tour auquel rien ne peut la touché ni même le joueur");
            Description.Add(PowerUps.ElJutos, "Vous pouvez cliquer sur un trou noir qui fera apparaître vos cellules autour");
            Description.Add(PowerUps.Topico, "Vide toute les cases qui sont sur la bordure de la partie");
            Description.Add(PowerUps.None, "Les informations sur les PowerUps s'afficheront ici, pour utiliser un powerUp il faut faire clique droit");
        }
    }
}

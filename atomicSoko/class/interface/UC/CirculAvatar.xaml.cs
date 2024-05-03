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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace atomicSoko
{
    /// <summary>
    /// Interaction logic for CirculAvatar.xaml
    /// </summary>
    public partial class CirculAvatar : UserControl
    {
        public Brush? Color { get; set; }

        public ImageBrush? Image { get; set; }
        public CirculAvatar()
        {
            InitializeComponent();
        }
        public void ChangeColor()
        {
            Ell.Fill = Color;
            recImg.Fill = Image;
        }
    }
}

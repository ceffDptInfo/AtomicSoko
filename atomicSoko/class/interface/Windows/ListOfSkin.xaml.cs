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
    /// Interaction logic for ListOfSkin.xaml
    /// </summary>
    public partial class ListOfSkin : Window
    {

        public event EventHandler? selected;
        public ListOfSkin(List<string> skin)
        {
            InitializeComponent();
            UpdateList(skin);
            
        }
        private void UpdateList(List<string> skin)
        {
            skins.ItemsSource = skin;
        }

        private void skins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected?.Invoke(skins.SelectedItem, EventArgs.Empty);
        }
    }
}

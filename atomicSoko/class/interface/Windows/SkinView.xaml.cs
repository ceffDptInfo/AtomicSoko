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
    /// Interaction logic for SkinView.xaml
    /// </summary>
    public partial class SkinView : Window
    {
        public SkinView(List<ImageBrush> images)
        {
            InitializeComponent();
            BuildScreen(images);
        }

        private void BuildScreen(List<ImageBrush> images)
        {
            for(int i = 0; i < images.Count; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }
            for(int i = 0;i < images.Count; i++)
            {
                Rectangle r = new Rectangle();
                r.Fill = images[i];
                Grid.SetRow(r, i);
                grid.Children.Add(r);
            }
            grid.Height = images.Count * 128;
        }
    }
}

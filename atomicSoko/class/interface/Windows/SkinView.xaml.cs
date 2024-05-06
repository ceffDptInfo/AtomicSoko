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
            for(int i = 0; i < 5; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                
            }
            for(int i = 0; i < Math.Round((double)images.Count / 4); i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            int Column = 0;
            int Row = 0;
            for(int i = 0;i < images.Count; i++)
            {
                Rectangle r = new Rectangle();
                r.Fill = images[i];
                Grid.SetColumn(r, Column);
                Grid.SetRow(r, Row);
                grid.Children.Add(r);
                
                if (Row == 4)
                {
                    Column++;
                    Row = 0;
                }
                else
                {
                    Row++;
                }
            }
            grid.Height = 5 * 128;
            if(images.Count > 5)
            {
                grid.Width = (Column + 1) * 128;
            }
            else
            {
                grid.Width = 128;
            }
            
        }
    }
}

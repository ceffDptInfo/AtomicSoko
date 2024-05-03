using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace atomicSoko
{
    /// <summary>
    /// Interaction logic for UCAtom.xaml
    /// </summary>
    public partial class UCAtom : UserControl
    {
        private Brush? color { get; set; }

        List<CirculAvatar> ellipse { get; set; }

        public BitmapImage? Image { get; set; }

        public UCAtom()
        {
            InitializeComponent();
            ellipse = new List<CirculAvatar> { Ell1, Ell2, Ell3 };
            imgBlackHole.Visibility = Visibility.Hidden;
        }

        //public

        public void DisplayEllips(string cell, List<User> users)
        {
            string[] chars = cell.Split(',');
            HidAll();
            User u = new User();
            foreach (User user in users)
            {
                if ($"p{cell[0]}" == user.Id)
                {
                    color = new SolidColorBrush(user.Color);
                    Image = user.Skin;
                }
            }
            switch (chars[1])
            {
                case "a":
                    SetEllipse(1);
                    break;
                case "b":
                    SetEllipse(2);
                    break;
                case "c":
                    SetEllipse(3);
                    break;
                case " ":
                    RemoveEllipse();
                    break;
                case "A":
                    imgBlackHole.Visibility = Visibility.Visible;
                    break;
                case "B":
                    RecWall.Visibility = Visibility.Visible;
                    break;
            }
            if (chars.Length == 3)
            {
                LinearGradientBrush gradientBrush = new LinearGradientBrush();
                gradientBrush.StartPoint = new Point(0, 0);
                gradientBrush.EndPoint = new Point(1, 1);
                gradientBrush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.0));
                gradientBrush.GradientStops.Add(new GradientStop(Colors.Red, 1.1));
                UCRectangle.Stroke = gradientBrush;
            }
            else
            {
                UCRectangle.Stroke = new SolidColorBrush(Colors.Black);
            }

        }
        
        //private
        private void SetEllipse(int turn)
        {
            for(int i = 0; i < turn; i++)
            {
                CirculAvatar e = ellipse[i];
                e.Color = color;
                e.Image = new ImageBrush(Image);
                e.Visibility = Visibility.Visible;
                e.ChangeColor();
            }
        }
        private void RemoveEllipse()
        {
            foreach(var ele in ellipse)
            {
                ele.Visibility = Visibility.Hidden;
            }
        }
        private void HidAll()
        {
            imgBlackHole.Visibility = Visibility.Hidden;
            RecWall.Visibility = Visibility.Hidden;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
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
    /// Interaction logic for UCSwitchButton.xaml
    /// </summary>
    public partial class UCSwitchButton : UserControl
    {
        public bool LogInEnable { get; set; } = true;

        public bool SignUpEnable { get; set; } = false;

        public event EventHandler? OnSwitched;

        private Color selectedColor = Colors.Blue;

        private Color notSelectedColor = Color.FromRgb(0, 214, 255);

        public UCSwitchButton()
        {
            InitializeComponent();
            GoodColor();
        }

        private void RecLogIn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LogInEnable = true;
            SignUpEnable = false;
            GoodColor();
            OnSwitched?.Invoke(null, EventArgs.Empty);
        }

        private void RecSignUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LogInEnable = false;
            SignUpEnable = true;
            GoodColor();
            OnSwitched?.Invoke(null, EventArgs.Empty);
        }
        private void GoodColor()
        {
            if(LogInEnable)
            {
                RecLogIn.Fill = new SolidColorBrush(selectedColor);
                RecSignUp.Fill = new SolidColorBrush(notSelectedColor);
            }
            if(SignUpEnable)
            {
                RecLogIn.Fill = new SolidColorBrush(notSelectedColor);
                RecSignUp.Fill = new SolidColorBrush(selectedColor);
            }
        }
    }
}

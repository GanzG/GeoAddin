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

namespace GeoAddin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RoomGenWindow : Window
    {
        public double upoffset;
        public double botoffset;
        public bool clickedon;
        public bool clickedoff;
        public RoomGenWindow()
        {
            InitializeComponent();
            clickedon = false;
            clickedoff = false;
        }

        private void But_Click_On(object sender, RoutedEventArgs e)
        {
            clickedon = true;
            upoffset = Convert.ToDouble(Upoffset.Text);
            botoffset = Convert.ToDouble(Botoffset.Text);
            Close();
        }

        private void But_Click_Off(object sender, RoutedEventArgs e)
        {
            clickedoff = true;
            Close();
        }
    }
}

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
    public partial class ApartmentgraphyWindow : Window
    {
        public double balconycoef;
        public double loggiacoef;
        public int roundNum;
        public bool clickedon;
        public bool clickedoff;
        public ApartmentgraphyWindow()
        {
            InitializeComponent();
            clickedon = false;
            clickedoff = false;
        }

        private void But_Click_On(object sender, RoutedEventArgs e)
        {
            clickedon = true;
            balconycoef = Convert.ToDouble(Balcony.Text);
            loggiacoef = Convert.ToDouble(Loggia.Text);
            roundNum = int.Parse(RoundNum.Text);

            Close();
        }

        private void But_Click_Off(object sender, RoutedEventArgs e)
        {
            clickedoff = true;
            Close();
        }

        private void RoundNum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}

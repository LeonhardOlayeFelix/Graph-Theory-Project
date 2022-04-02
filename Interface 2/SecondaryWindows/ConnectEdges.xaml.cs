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
using System.Text.RegularExpressions;
namespace Interface_2
{
    /// <summary>
    /// Interaction logic for ConnectEdges.xaml
    /// </summary>
    
    public partial class ConnectEdges : Window
    {
        public ConnectEdges()
        {
            InitializeComponent();
        }

        private void buttonConfirmWeight_Click(object sender, RoutedEventArgs e)
        {
            if (txWeight.Text.Length != 0)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void txWeight_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txWeight_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

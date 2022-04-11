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

        private void buttonRandomiseWeight_Click(object sender, RoutedEventArgs e)
        {
            if (txMaximum.Text.Length != 0 && txMinimum.Text.Length != 0)
            {
                int min = Convert.ToInt32(txMinimum.Text);
                int max = Convert.ToInt32(txMaximum.Text);
                Random random = new Random();
                if (min < max)
                {
                    int weight = random.Next(min, max + 1);
                    txWeight.Text = weight.ToString();
                }
                else if (min == max)
                {
                    txWeight.Text = txMinimum.Text;
                }
                else //if less than
                {
                    txWeight.Text = "0";
                }
                this.DialogResult = true;
                this.Close();
            }
            
        }
    }
}

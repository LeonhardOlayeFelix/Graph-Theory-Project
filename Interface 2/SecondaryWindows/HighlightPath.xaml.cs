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
    /// Interaction logic for HighlightPath.xaml
    /// </summary>
    public partial class HighlightPath : Window
    {
        List<int> path = new List<int>();
        public HighlightPath()
        {
            InitializeComponent();
        }

        private void txPathEntry_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnAddToPath_Click(object sender, RoutedEventArgs e)
        {
            if (txPathEntry.Text.Length != 0)
            {
                int nextVertex = Convert.ToInt32(txPathEntry.Text);
                txPathEntry.Clear();
                path.Add(nextVertex);
            }
            
        }

        private void btnHighlightPath_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        
        public List<int> getPath()
        {
            return path;
        }
    }
}

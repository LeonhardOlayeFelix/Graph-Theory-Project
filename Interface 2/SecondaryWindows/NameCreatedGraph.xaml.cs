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

namespace Interface_2
{
    /// <summary>
    /// Interaction logic for NameCreatedGraph.xaml
    /// </summary>
    public partial class NameCreatedGraph : Window
    {
        string graphName = "";
        public NameCreatedGraph()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            graphName = txBoxGraphName.Text;
            if (graphName.Length >= 2 && graphName.Length <= 15)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void txBoxGraphName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

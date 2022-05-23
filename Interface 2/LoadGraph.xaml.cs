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
using System.Data.OleDb;
using System.Data;

namespace Interface_2
{
    /// <summary>
    /// Interaction logic for LoadGraph.xaml
    /// </summary>
    public partial class LoadGraph : Window
    {
        public string graphToLoad = "";
        public LoadGraph()
        {
            InitializeComponent();
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            OleDbCommand cmd = new OleDbCommand();
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT GraphName FROM Graph";
            DataTable datatable = new DataTable();
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(cmd);
            dataAdapter.Fill(datatable);
            cbGraphName.ItemsSource = datatable.DefaultView;
            cbGraphName.DisplayMemberPath = "GraphName";
            cbGraphName.SelectedValuePath = "GraphName";
            conn.Close();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                graphToLoad = cbGraphName.SelectedValue.ToString();
            }
            catch
            {
                graphToLoad = "fail";
            }
            DialogResult = true;
            this.Close();
        }
    }
}

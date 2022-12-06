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
        public string usertype = "";
        Database database = MainWindow.database;
        public LoadGraph()
        {
            InitializeComponent();
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            OleDbCommand cmd = new OleDbCommand();
            conn.Open();
            cmd.Connection = conn;
            //we only want to display the graphs that the user is supposed to have access to.
            if (database.StudentIsLoggedIn(MainWindow.loggedStudent))
            {
                usertype = "s";
                string ID = MainWindow.loggedStudent.ID;
                cmd.CommandText = $"SELECT GraphName FROM StudentGraph WHERE StudentID = '{ID}'";
            } 
            else if (database.TeacherIsLoggedIn(MainWindow.loggedTeacher))
            {
                usertype = "t";
                string ID = MainWindow.loggedTeacher.ID;
                cmd.CommandText = $"SELECT GraphName FROM TeacherGraph WHERE TeacherID = '{ID}'";
            }
            else
            {
                usertype = "g";
                cmd.CommandText = $"SELECT GraphName FROM GuestGraph";
            }
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
                graphToLoad = cbGraphName.SelectedValue.ToString(); //create the correct graph name
                if (usertype == "s") { graphToLoad = "StudentGraphs/" + graphToLoad + MainWindow.loggedStudent.ID; }
                else if (usertype == "t") { graphToLoad = "TeacherGraphs/" + graphToLoad + MainWindow.loggedTeacher.ID; }
                else if (usertype == "g") { graphToLoad = "GuestGraphs/" + graphToLoad; }
            }
            catch
            {
                graphToLoad = "fail";
            }
            DialogResult = true;
            this.Close();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        
    }
}

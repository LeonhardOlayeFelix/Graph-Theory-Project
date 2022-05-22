using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Media;
using System.Data.OleDb;
using System.IO;
namespace Interface_2
{
    public partial class MainWindow : Window
    {

        //for the algorithms
        Ellipse lastSelectedVertex;
        Ellipse vertexToConnectTo;
        int buttonSelectionCount = 0;
        int dijkstraSelectionCount = 0;
        int rInspSelectionCount = 0;
        int rInspStart = 0;
        int startVertex = 0;
        
        static List<string> normalAlphabet = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        static List<string> alphabet = new List<string>(); //the id that the nodes could be assigned
        
        private bool graphCreated = false;//for loading, creating, and deleting files
        private Button currentButton = null; //the button that is currently activated
        private Color btnActivatedColour = Color.FromRgb(190, 230, 253);//colour to highlight activated buttons with

        List<TextBlock> valencyList = null; //a list of all the valency textblocks
        
        List<int> livePath = new List<int>(); //for the highlight path button

        List<Ellipse> vertexList = null; //list containing all of the ellipses
        List<TextBlock> vertexTxBoxList = null; //list containging all of the ellipses' labels
        HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>> edgeList = null; //hashset of tuples which represents a whole edge: Item1(Line) Item2(first end) Item3(other end) item4(weight)

        int Zindex = 2;//will let the shapes overlap each other in an ordered fashion
        int buttonId = 0; //gives each vertex a unique Id
        string valencyState; //Hidden or Visible

        Ellipse ellipseToDrop = null; //for the drag and drop function

        SolidColorBrush HighlightColour = null;
        
        private Network Graph = null; //initialise a Network Class
        public MainWindow()
        {
            InitializeComponent();
            DisableAllActionButtons();
            DisableTbCtrl();
            foreach(string letter in normalAlphabet)
            {
                alphabet.Add(letter); //first populate with letter
            }
            for (int i = 0; i < normalAlphabet.Count(); ++i)
            {
                for (int j = 0; j < normalAlphabet.Count(); ++j)
                {
                    string newId = normalAlphabet[i] + normalAlphabet[j];
                    alphabet.Add(newId);
                }
            }
            //now alphabet is populated with "A", "B", "C", "D" ... "AA", "AB", "AC" ... "BA, BB, BC" ... all the way to "ZA, ZB, ZC" so there are many unique ways to
            //represent nodes with letters now

            CreateDatabase();
        }
        public const string ConStr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=Networking.accdb";

        public void CreateDatabase()
        {
            if (!File.Exists("Networking.accdb"))
            {
                ADOX.Catalog cat = new ADOX.Catalog();
                cat.Create(ConStr);
                OleDbConnection conn = new OleDbConnection(ConStr);
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                string SQL = "";
                SQL += "CREATE TABLE Graph(GraphID INTEGER, GraphName VARCHAR(15), NumberOfVertices INTEGER, NumberOfEdges INTEGER)";
                cmd.CommandText = SQL;
                cmd.ExecuteNonQuery();
            }
        }
        
    }
}

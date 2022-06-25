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
        public static Teacher loggedTeacher = null; //teacher that is logged in
        public static Student loggedStudent = null; //student that is logged in
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
        
        private Graph Graph = null; //initialise a Network Class
        public MainWindow()
        {
            InitializeComponent();
            DisableAllActionButtons();
            DisableTbCtrl();
            btnSaveGraph.IsEnabled = false;
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
            LogOutProcess();
        }
        public const string ConStr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=NetworkDB.accdb";
        public static void CreateDatabase()
        {
            if (!File.Exists("NetworkDB.accdb")) //if a file doesnt already exist for the database
            {
                //establish the connection and then create database 
                ADOX.Catalog cat = new ADOX.Catalog();
                cat.Create(ConStr);
                OleDbConnection conn = new OleDbConnection(ConStr);
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = "CREATE TABLE Student(StudentID VARCHAR(5), FirstName VARCHAR(30), LastName VARCHAR(30), DateOfBirth DATE, Email VARCHAR(100), SPassword VARCHAR(30), NoAssignmentsSubmitted INTEGER, NoDijkstras INTEGER, NoRInsp INTEGER, NoBFS INTEGER, NoDFS INTEGER, NoPrims INTEGER, NoGraph INTEGER, DateCreated DATE, PRIMARY KEY(StudentID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE Teacher(TeacherID VARCHAR(5), FirstName VARCHAR(30), LastName VARCHAR(30), Email VARCHAR(100), TPassword VARCHAR(30), Title VARCHAR(7), PRIMARY KEY(TeacherID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE StudentGraph(Filename VARCHAR(30), StudentID VARCHAR(5), GraphName VARCHAR(25), DateCreated DATE, NoVertices INTEGER, NoEdges INTEGER, CreatedBy CHAR(1), PRIMARY KEY(Filename), FOREIGN KEY (StudentID) REFERENCES Student(StudentID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE TeacherGraph(Filename VARCHAR(30), TeacherID VARCHAR(5), GraphName VARCHAR(25), DateCreated DATE, NoVertices INTEGER, NoEdges INTEGER, CreatedBy CHAR(1), PRIMARY KEY(Filename), FOREIGN KEY (TeacherID) REFERENCES Teacher(TeacherID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE GuestGraph(Filename VARCHAR(30), GraphName VARCHAR(25), CreatedBy CHAR(1), PRIMARY KEY(Filename))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE Assignment(AssignmentID VARCHAR(5), StudentID VARCHAR(5), Filename VARCHAR(30), SetBy VARCHAR(5), GraphName VARCHAR(25), DateSet DATE, DateDue DATE, isLate CHAR(1), isCompleted CHAR(1), PRIMARY KEY(AssignmentID), FOREIGN KEY (StudentID) REFERENCES Student(StudentID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE Class(ClassID VARCHAR(5), TeacherID VARCHAR(5), ClassName VARCHAR(30), PRIMARY KEY(ClassID), FOREIGN KEY (TeacherID) REFERENCES Teacher(TeacherID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE ClassEnrollment(ClassID VARCHAR(5), StudentID VARCHAR(5), FirstName VARCHAR(30), LastName VARCHAR(30), EnrollDate DATE, FOREIGN KEY (ClassID) REFERENCES Class(ClassID), FOREIGN KEY (StudentID) REFERENCES Student(StudentID))";
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }//creates a database

        private void btnRevertOnePositions_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
        }

        private void optionTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        
    }
}

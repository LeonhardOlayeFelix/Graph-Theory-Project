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
        List<Line> linesToDelete = new List<Line>();
        public static Teacher loggedTeacher = null;
        public static Student loggedStudent = null;
        Ellipse lastSelectedVertex;
        int buttonSelectionCount = 0;
        int dijkstraSelectionCount = 0;
        int rInspSelectionCount = 0;
        int rInspStart = 0;
        int startVertex = 0;
        public static List<string> alphabet = new List<string>();
        public bool graphCreated = false;
        private Button currentButton = null;
        Action leftClickCanvasOperation;
        Action<Ellipse> leftClickVertexOperation;
        Action<Line> leftClickLineOperation;
        Action leftMouseButtonUpOperation;
        private Color btnActivatedColour = Color.FromRgb(190, 230, 253);
        List<TextBlock> valencyList = null;
        List<int> livePath = new List<int>();
        public List<Ellipse> vertexList = null;
        public List<TextBlock> vertexTxBoxList = null;
        List<Tuple<Line, Ellipse, Ellipse, TextBlock>> edgeList = null;
        public int Zindex = 2;
        public int buttonId = 0;
        string valencyState;
        Ellipse ellipseToDrop = null;
        SolidColorBrush HighlightColour = null;
        bool mouseDown = false;
        Point mouseDownPos;
        List<Ellipse> selectedVertices = new List<Ellipse>();
        List<string> selectedLinesNames = new List<string>();
        public Graph graph = null;
        public MainWindow()
        {
            List<string> normalAlphabet = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            InitializeComponent();
            DisableAllActionButtons();
            DisableTabControl();
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
        /// <summary>
        /// Creates the database if it hasn't already been created
        /// </summary>
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
        }

        
    }
}

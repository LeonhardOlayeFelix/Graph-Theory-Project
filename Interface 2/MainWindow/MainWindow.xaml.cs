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
        bool assignmentOpen = false;
        string openAssignmentPath = "";
        Ellipse lastSelectedVertex;
        int buttonSelectionCount = 0;
        int dijkstraSelectionCount = 0;
        int rInspSelectionCount = 0;
        int rInspStart = 0;
        int startVertex = 0;
        public static List<string> ModifiedAlphabet = new List<string>();
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
        Ellipse ellipseHovered = null;
        SolidColorBrush HighlightColour = null;
        bool mouseDown = false;
        Point mouseDownPos;
        List<Ellipse> selectedVertices = new List<Ellipse>();
        List<string> selectedLinesNames = new List<string>();
        public Graph graph = null;
        public static Database database = new Database();

        public MainWindow()
        {
            InitializeComponent();
            PopulateIDs();
            DisableAllActionButtons();
            DisableTabControl();
            ClearAllOperations();
            showNextID();
            loadComboBoxes();
            btnSaveGraph.IsEnabled = false;
            LogOutProcess();
        }
        private void PopulateIDs()
        {
            List<string> alphabet = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            foreach (string letter in alphabet)
            {
                ModifiedAlphabet.Add(letter); //first populate with letter
            }
            for (int i = 0; i < alphabet.Count(); ++i)
            {
                for (int j = 0; j < alphabet.Count(); ++j)
                {
                    string newId = alphabet[i] + alphabet[j];
                    ModifiedAlphabet.Add(newId);
                }
            }
        }
        public const string ConStr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=NetworkDB.accdb";
    }
}

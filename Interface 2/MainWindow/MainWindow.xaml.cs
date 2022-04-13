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

        private Network Graph = null; //initialise a Network Class
        public MainWindow()
        {
            InitializeComponent();
            DisableAllActionButtons();
            DisableTbCtrl();
        }
        
        
    }
}

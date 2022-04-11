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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //for the connection process
        Ellipse lastSelectedVertex;
        Ellipse vertexToConnectTo;
        int buttonSelectionCount = 0;
        int dijkstraSelectionCount = 0;
        int rInspSelectionCount = 0;
        int rInspStart = 0;
        int startVertex = 0;
        //for loading, creating, and deleting files
        private bool graphCreated = false;

        //colour to highlight activated buttons with
        private Color btnActivatedColour = Color.FromRgb(190, 230, 253);

        //the button that is currently activated
        private Button currentButton = null;

        //to know whether to show the valencies or not
        string valencyState;
        List<TextBlock> valencyList = null; //a list of all the valency textblocks
        //Id to assigns buttons with
        int buttonId = 0;

        //for the highlight path button
        List<int> livePath = new List<int>();

        //intialise all of the structures
        List<Ellipse> vertexList = null;
        List<TextBlock> vertexTxBoxList = null;
        HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>> edgeList = null; //hashset of tuples containing in order the Line of the edge,
                                                                           //the smallest vertex, the largest vertex, the weight
        //for the drag feature                                                              //will be assigned to the ellipse that will be dragged then dropped
        Ellipse ellipseToDrop = null;
        //the actual graph class
        private AdjacencySetGraph Graph = null;
        public MainWindow()
        {
            InitializeComponent();
            DisableAllActionButtons();
            DisableTbCtrl();
        }

        

        
    }
}

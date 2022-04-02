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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

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

        //intialise all of the structures
        List<Ellipse> vertexList = null;
        List<TextBlock> vertexTxBoxList = null;
        HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>> edgeList = null; //hashset of tuples containing in order the Line of the edge,
                                                                           //the smallest vertex, the largest vertex, the weight
        //will be assigned to the ellipse that will be dragged then dropped
        Ellipse ellipseToDrop = null;
        
        //the instanciation graph class
        private AdjacencySetGraph Graph = null;
        public MainWindow()
        {
            InitializeComponent();
            DisableAllActionButtons();
            DisableTbCtrl();
        }
        private void DeactivateButton() //'deactivates' button
        {
            if (currentButton != null)
                currentButton.Background = new SolidColorBrush(Color.FromRgb(221, 221, 221));
        }
        private void btnLoadNewGraph_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
        }

        private void btnLoadSavedGraph_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
        }

        private void btnSaveGraph_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnClearOtherAlgoLogs_Click(object sender, RoutedEventArgs e)
        {
            labelExtraInfo.Content = "Logs Cleared.";
            txLogsOtherAlgorithms.Clear();
        }

        private void btnClearPathFindingLogs_Click(object sender, RoutedEventArgs e)
        {
            labelExtraInfo.Content = "Logs Cleared.";
            txLogsPathFinding.Clear();
        }
        private void btnClearActionLogs_Click(object sender, RoutedEventArgs e)
        {
            labelExtraInfo.Content = "Logs Cleared.";
            txLogsActions.Clear();
        }

        private void btnAddVertex_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click anywhere on the canvas to place a vertex";
            ActivateButton(sender);
        }

        private void btnAddConnection_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click two vertices you want to connect and provide the weight";
            ActivateButton(sender);
        }

        private void btnDeleteVertex_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click a vertex to delete it from the canvas";
            ActivateButton(sender);
        }

        private void btnDeleteConnection_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click an edge to Delete it from the canvas";
            ActivateButton(sender);
        }

        private void btnTakeScreenshot_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Screenshot Taken";
            ActivateButton(sender);
        }
        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            labelExtraInfo.Content = "Click freely around the Canvas";
            ActivateButton(sender);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {

        }
        private void mainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) //if the canvas is pressed
        {

            if (e.OriginalSource is Ellipse) //if they press the Vertex 
            {
                //check if they are joining vertices together or deleting vertices (make sure deleting is pressed)
                if (currentButton == btnDeleteVertex)
                {
                    Ellipse activeVertex = (Ellipse)e.OriginalSource; //give the button a variable to refer to
                    HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>> listOfEdgesToRemove = GetListOfEdgesFromVertex(activeVertex);//gets list of edges we need to remove with the vertex
                    Graph.RemoveVertex(Convert.ToInt32(activeVertex.Name.Substring(3))); //update the class
                    txLogsActions.AppendText(Graph.Name + ".RemoveVertex(" + Convert.ToInt32(activeVertex.Name.Substring(3)).ToString() + ");\n");//updates the logs
                    //loop through lines and delete any lines that come out of it
                    foreach (Tuple<Line, Ellipse, Ellipse, TextBlock> edge in listOfEdgesToRemove)
                    {
                        DeleteEdge(edge); //calls function to delete the edge from cavas
                    }
                    mainCanvas.Children.Remove(activeVertex); //then delete the vertex
                    vertexList.Remove(activeVertex);//delete it from the list
                    foreach (TextBlock vertexLabel in vertexTxBoxList)
                    {
                        if (vertexLabel.Text == activeVertex.Name.Substring(3))
                        {
                            mainCanvas.Children.Remove(vertexLabel); //delete the label too
                            vertexTxBoxList.Remove(vertexLabel); //remove the label from the list
                            break;
                        }
                    }

                }
                else if (currentButton == btnAddConnection) //if they want to add a vertex
                {
                    buttonSelectionCount += 1;

                    if (buttonSelectionCount % 2 == 0 && buttonSelectionCount != 0) //if even its the second vertex they want to connect to
                    {
                        vertexToConnectTo = (Ellipse)e.OriginalSource; //this is the vertex they are connecting to
                        ConnectEdges connectEdges = new ConnectEdges();
                        if (lastSelectedVertex == vertexToConnectTo) //if they are connecting it to itself, do nothing
                        {
                            EnableAllActionButtons();
                            EnableTbCtrl();
                            btnLoadSavedGraph.IsEnabled = false;
                            btnLoadSavedGraph.IsEnabled = false;
                            labelExtraInfo.Content = "";
                        }
                        else if (connectEdges.ShowDialog() == true) // otherwise, open a new form and get the weight
                        {
                            int weight = Convert.ToInt32(connectEdges.txWeight.Text); //get weight from text box
                            ConnectVertices(lastSelectedVertex, vertexToConnectTo, weight); //add the edge
                            labelExtraInfo.Content = "";
                            EnableTbCtrl();
                            EnableAllActionButtons();
                            btnLoadSavedGraph.IsEnabled = false;
                            btnLoadSavedGraph.IsEnabled = false;
                        }
                        else
                        {
                            buttonSelectionCount -= 1; //if they closed the form without providing weight, decrement it so they can press another vertex
                        }
                    }
                    else if (buttonSelectionCount % 2 == 1) //if odd, its the first vertex they pressed to connect to, so set it to lastSelectedVertex
                    {
                        lastSelectedVertex = (Ellipse)e.OriginalSource;
                        labelExtraInfo.Content = "From vertex " + lastSelectedVertex.Name.Substring(3) + " to.....";
                        DisableTbCtrl();
                        DisableAllActionButtons();
                    }
                }
                else if (currentButton == btnDijkstras)
                {
                    dijkstraSelectionCount += 1;
                    if (dijkstraSelectionCount % 2 == 0)
                    {
                        Ellipse v = (Ellipse)e.OriginalSource;
                        int vId = Convert.ToInt32(v.Name.Substring(3));
                        if (startVertex == vId) //if they are connecting it to itself, do nothing
                        {
                            EnableAllActionButtons();
                            EnableTbCtrl();
                            btnLoadSavedGraph.IsEnabled = false;
                            btnLoadSavedGraph.IsEnabled = false;
                            labelExtraInfo.Content = "";
                        }
                        else
                        {
                            List<int> path = Graph.DijkstrasAlgorithm(startVertex, vId);
                            EnableTbCtrl();
                            EnableAllAlgoButtons();
                            if (path != null)
                            {
                                DijkstraHighlightPath(path);
                                labelExtraInfo.Content = "";
                            }
                            else
                            {
                                MessageBox.Show("There is no shortest path from these two points");
                            }
                        }
                    }
                    else if (dijkstraSelectionCount % 2 == 1)
                    {
                        DisableTbCtrl();
                        DisableAllAlgoButtons();
                        Ellipse v = (Ellipse)e.OriginalSource;
                        startVertex = Convert.ToInt32(v.Name.Substring(3));
                        labelExtraInfo.Content = "Shortest Path from " + startVertex + " to...";
                    }
                }
            }
            else if (e.OriginalSource is Line) //if they click on a line in the canvas
            {
                //check if they are deleting edges (make sure deleting is pressed)
                if (currentButton == btnDeleteConnection)
                {
                    //give the line a variable to refer to
                    Line activeEdge = (Line)e.OriginalSource;
                    foreach (Tuple<Line, Ellipse, Ellipse, TextBlock> edge in edgeList)
                    {
                        if (edge.Item1 == activeEdge) //find the correct edge,
                        {
                            DeleteEdge(edge); //the delete the edge
                            Graph.RemoveEdge(Convert.ToInt32(edge.Item2.Name.Substring(3)), Convert.ToInt32(edge.Item3.Name.Substring(3))); //update the class graph
                            break;
                        }
                    }
                }
            }
            else
            {
                if (currentButton == btnAddVertex) //this is where the button will be added to the canvas
                {
                    //update class
                    Graph.AddVertex();

                    //create the vertex that will be added
                    Ellipse vertexToAdd = new Ellipse() { StrokeThickness = 2 };

                    //binding the stroke of the vertices to the color picker
                    Binding bindingStroke = new Binding("SelectedBrush")
                    {
                        Source = colourPickerVertexStroke,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.TwoWay
                    };
                    vertexToAdd.SetBinding(Ellipse.StrokeProperty, bindingStroke);

                    //binding the fill colour of the vertices to the color picker
                    Binding bindingFill = new Binding("SelectedBrush")
                    {
                        Source = colourPickerVertex,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.TwoWay
                    };
                    vertexToAdd.SetBinding(Ellipse.FillProperty, bindingFill);

                    //binding the diameter of the vertices to the slider
                    Binding bindingDiameter = new Binding("Value")
                    {
                        Source = vertexDiameterSlider,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    };
                    vertexToAdd.SetBinding(Ellipse.HeightProperty, bindingDiameter);
                    vertexToAdd.SetBinding(Ellipse.WidthProperty, bindingDiameter);

                    //positioning the vertex in the canvas.
                    double vertexCenterXMousePos = Mouse.GetPosition(mainCanvas).X;
                    double vertexCenterYMousePos = Mouse.GetPosition(mainCanvas).Y;
                    vertexToAdd.Margin = new Thickness(-100000); //margin of 100000 so that it resizes around the center.
                    Canvas.SetLeft(vertexToAdd, vertexCenterXMousePos);
                    Canvas.SetTop(vertexToAdd, vertexCenterYMousePos);
                    Canvas.SetZIndex(vertexToAdd, 3);

                    //give the string a Name in the form btn(vertexId)
                    string vertexId = buttonId.ToString();
                    vertexToAdd.Name = "btn" + vertexId;

                    buttonId += 1; //increment button Id for unique buttons
                    vertexList.Add(vertexToAdd);//add the vertex to the list
                    vertexToAdd.MouseMove += mouseMove;//give the buttons drag and drop event handlers


                    //this is the label that describes what ID the vertex holds
                    TextBlock vertexLabel = new TextBlock() { Text = vertexId, FontSize = 15, Foreground = new SolidColorBrush(Colors.Black) };

                    //set its position ontop of the vertex, and at its center
                    Canvas.SetZIndex(vertexLabel, 4);
                    Canvas.SetTop(vertexLabel, Canvas.GetTop(vertexToAdd) - 9);
                    Canvas.SetLeft(vertexLabel, Canvas.GetLeft(vertexToAdd) - 4);

                    vertexTxBoxList.Add(vertexLabel);//add it to the vertex list

                    mainCanvas.Children.Add(vertexToAdd);//add the vertex to the canvas
                    mainCanvas.Children.Add(vertexLabel); //add the label to the canvas
                    txLogsActions.AppendText(Graph.Name + ".AddVertex()\n");//update logs
                }
            }
            if (graphCreated == true)
            {
                GenerateAdjList();
            }
        }
        private void ConnectVertices(Ellipse v1, Ellipse v2, int weight)
        {
            //gets the smaller and larger vertex
            Ellipse smallerEllipse = GetMinEllipse(v1, v2);
            Ellipse largerEllipse = GetMaxEllipse(v1, v2);

            //creates the lines soon to be name
            string lineName = "line" + smallerEllipse.Name.Substring(3).ToString() + "to" + largerEllipse.Name.Substring(3).ToString(); //have the name of the line in the form atob where a < b
            foreach (Tuple<Line, Ellipse, Ellipse, TextBlock> edge in edgeList)//but first check if a line with name exists
            {
                if (edge.Item1.Name == lineName)//if it does....
                {
                    DeleteEdge(edge);//delete line and its edge
                    break;//break to stop unecessary looping
                }
            }
            Graph.AddEdge(Convert.ToInt32(v1.Name.Substring(3)), Convert.ToInt32(v2.Name.Substring(3)), weight); //update the class

            //creates the line which will be connected
            Line temp = new Line()
            {
                StrokeThickness = 4,
                Name = lineName,
                Stroke = new SolidColorBrush(Colors.Black)
            };
            Canvas.SetZIndex(temp, 0); //make sure its underneath everything

            //bind the lines x1 to the smaller vertex's x1
            Binding bindingV1X = new Binding
            {
                Source = smallerEllipse,
                Path = new PropertyPath(Canvas.LeftProperty),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            temp.SetBinding(Line.X1Property, bindingV1X);

            //bind the lines y1 to the samller vertex's y1
            Binding bindingV1Y = new Binding
            {
                Source = smallerEllipse,
                Path = new PropertyPath(Canvas.TopProperty),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            temp.SetBinding(Line.Y1Property, bindingV1Y);

            //bind the buttons x2 to the larger vertex's x2
            Binding bindingV2X = new Binding
            {
                Source = largerEllipse,
                Path = new PropertyPath(Canvas.LeftProperty),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            temp.SetBinding(Line.X2Property, bindingV2X);

            //bind the buttons y2 to the larger vertex's y2
            Binding bindingV2Y = new Binding
            {
                Source = largerEllipse,
                Path = new PropertyPath(Canvas.TopProperty),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            temp.SetBinding(Line.Y2Property, bindingV2Y);

            //the textblock which representes the edge weight
            TextBlock weightLabel = new TextBlock()
            {
                Text = weight.ToString(),
                Name = "labelFor" + lineName,
                FontSize = 15,
                Foreground = new SolidColorBrush(Colors.White)
            };

            //binding the back colour of the weight to the colour 
            Binding bindingWeightBackcolor = new Binding("SelectedBrush")
            {
                Source = colourPickerWeight,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            weightLabel.SetBinding(TextBlock.BackgroundProperty, bindingWeightBackcolor);

            //set the initial position of the weight to the midpoint of the line it is bound to
            double MidPointX = (Canvas.GetLeft(smallerEllipse) + Canvas.GetLeft(largerEllipse)) / 2;
            double MidPointY = (Canvas.GetTop(smallerEllipse) + Canvas.GetTop(largerEllipse)) / 2;
            Canvas.SetLeft(weightLabel, MidPointX - 4);
            Canvas.SetTop(weightLabel, MidPointY - 9);
            Canvas.SetZIndex(weightLabel, 5); //needs to be visible above all else

            //add a new edge tuple to the list
            edgeList.Add(Tuple.Create(temp, smallerEllipse, largerEllipse, weightLabel));
            //update logs
            txLogsActions.AppendText("AddEdge(" + smallerEllipse.Name.Substring(3) + "," + largerEllipse.Name.Substring(3) + ")\n");

            //if the weight is 0, show it as an unweighted graph
            mainCanvas.Children.Add(temp);
            if (weight != 0)
            {
                mainCanvas.Children.Add(weightLabel);
            }
            GenerateAdjList();
        }
        private Ellipse GetMinEllipse(Ellipse vertex1, Ellipse vertex2) //returns vertex with smallest ID
        {
            return (Convert.ToInt32(vertex1.Name.Substring(3)) < Convert.ToInt32(vertex2.Name.Substring(3))) ? vertex1 : vertex2;
        }
        private Ellipse GetMaxEllipse(Ellipse vertex1, Ellipse vertex2) //return vertex with largest ID
        {
            return (Convert.ToInt32(vertex1.Name.Substring(3)) > Convert.ToInt32(vertex2.Name.Substring(3))) ? vertex1 : vertex2;
        }
        private void DeleteEdge(Tuple<Line, Ellipse, Ellipse, TextBlock> edge) //deletes an edge
        {
            mainCanvas.Children.Remove(edge.Item1); //remove the line which is the first item
            mainCanvas.Children.Remove(edge.Item4);//remove the label which is the fourth element
            txLogsActions.AppendText("RemoveEdge(" + edge.Item2.Name.Substring(3) + "," + edge.Item3.Name.Substring(3) + ")\n"); //update logs
            edgeList.Remove(edge);//remove it from the graph
            GenerateAdjList();
        }
        private void btnResetComponentShape_Click(object sender, RoutedEventArgs e)
        {
            //resets the sliders back to their original form
            edgeThicknessSlider.Value = edgeThicknessSlider.Minimum;
            vertexDiameterSlider.Value = vertexDiameterSlider.Minimum;
            weightAndLabelFontSizeSlider.Value = weightAndLabelFontSizeSlider.Minimum;
        }
        public HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>> GetListOfEdgesFromVertex(Ellipse activeVertex) //gets all of the edges coming out of a vertex
        {
            HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>> listOfEdges = new HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>>(); //thing to return
            foreach (Ellipse vertex in vertexList) //loop through vertex list
            {
                if (vertex != activeVertex)//dont check for an edge from itself to itself
                {
                    Ellipse largerEllipse = GetMaxEllipse(vertex, activeVertex);
                    Ellipse smallerEllipse = GetMinEllipse(vertex, activeVertex); //orders the ellipses
                    string lineNameToFind = "line" + smallerEllipse.Name.Substring(3).ToString() + "to" + largerEllipse.Name.Substring(3).ToString(); //name is in the form we expect it to be
                    foreach (Tuple<Line, Ellipse, Ellipse, TextBlock> edge in edgeList)
                    {
                        if (edge.Item1.Name == lineNameToFind) //when found...
                        {
                            listOfEdges.Add(edge);//add it to the list of edges
                        }
                    }
                }
            }
            return listOfEdges; //return the list of edges
        }
        private void btnCreateNewGraph_Click(object sender, RoutedEventArgs e)
        {
            string name = "";
            NameCreatedGraph nameGraphWindow = new NameCreatedGraph();
            nameGraphWindow.ShowDialog();
            if (nameGraphWindow.DialogResult == true)
            {
                name = nameGraphWindow.txBoxGraphName.Text;
                txLogsActions.AppendText("CreateNewGraph(" + name + ")\n");
                DeleteGraph();
                CreateNewGraph(name);
                btnDeleteGraph.IsEnabled = true;
                graphCreated = true;
                txAdjset.Clear();
            }
        }
        public void CreateNewGraph(string graphName) //creates a new graph
        {
            //re-intiliaise all of the attributes declared/defined above
            edgeList = new HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>>();
            Graph = new AdjacencySetGraph();
            valencyState = "Hidden";
            valencyList = new List<TextBlock>();
            Graph.Name = graphName; //the name that they provided, length = 2 to 15
            vertexTxBoxList = new List<TextBlock>();
            vertexList = new List<Ellipse>();
            graphCreated = true;
            labelGraphName.Content = graphName;
            EnableAllActionButtons(); //can only navigate buttons when a graph is created
            EnableTbCtrl();
        }
        private void btnDeleteGraph_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            txAdjset.Clear();
            DeleteGraph();
            DisableAllActionButtons();
        }
        public void DeleteGraph()
        {
            mainCanvas.Children.Clear();
            btnDeleteGraph.IsEnabled = false;
            labelGraphName.Content = "";
            Graph = new AdjacencySetGraph();
            HideValencies();
            buttonId = 0;
            Graph = null;

            lastSelectedVertex = null;
            vertexToConnectTo = null;
            vertexTxBoxList = new List<TextBlock>();
            vertexList = new List<Ellipse>();
            edgeList = new HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>>();
            graphCreated = false;
        }
        private void mouseMove(object sender, MouseEventArgs e)
        {
            //monitors the mouse as it hovers over a vertex
            if (e.LeftButton == MouseButtonState.Pressed && currentButton == btnDragAndDrop)//if, whilst hovering, they press the vertex
            {
                ellipseToDrop = sender as Ellipse; //update teh
                DragDrop.DoDragDrop(sender as Ellipse, sender as Ellipse, DragDropEffects.Move); //start the drag function on this vertex
            }
        }
        private void mainCanvas_DragOver(object sender, DragEventArgs e)
        {
            Point dropPosition = e.GetPosition(mainCanvas); //current position of the place its being dragged
            Canvas.SetLeft(ellipseToDrop, dropPosition.X);//updates the x coordinate every time its dragged
            Canvas.SetTop(ellipseToDrop, dropPosition.Y);//updates the y coordinate ever time its dragged
            foreach (TextBlock label in vertexTxBoxList) //look for the label that matches too the ellipse
            {
                if (label.Text == ellipseToDrop.Name.Substring(3))
                {
                    Canvas.SetLeft(label, dropPosition.X - 4); //update that label too
                    Canvas.SetTop(label, dropPosition.Y - 9);
                }
            }
            foreach (Tuple<Line, Ellipse, Ellipse, TextBlock> edge in edgeList)
            {
                if (edge.Item2 == ellipseToDrop || edge.Item3 == ellipseToDrop) //look for the weight that matches to vertexes
                {
                    double MidPointX = (Canvas.GetLeft(edge.Item2) + Canvas.GetLeft(edge.Item3)) / 2;
                    double MidPointY = (Canvas.GetTop(edge.Item2) + Canvas.GetTop(edge.Item3)) / 2; //update it to the midpoint of the line as it moves each time
                    Canvas.SetLeft(edge.Item4, MidPointX - 4);
                    Canvas.SetTop(edge.Item4, MidPointY - 9);
                }

            }
        }
        private void btnDragAndDrop_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click and Hold vertices to drag them around the canvas";
            ActivateButton(sender);
        }
        private void mainCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) //if mouse is scrolled up, increase slider value
            {
                vertexDiameterSlider.Value += 2; //this increases the vertex diameter (bound)
            }
            else if(e.Delta < 0)//if mouse is scrolled down, increase slider value
            {
                vertexDiameterSlider.Value -= 2;//this decreases the vertex diameter (bound)
            }
        }
        
        private void btnDijkstras_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click a vertex to find the shortest route to the next clicked vertex";
            ActivateButton(sender);
        }
        public void DijkstraHighlightPath(List<int> path) //a path of vertexIds, in the order they want to be traversed
        {
            int total = 0;
            if (path.Count() > 1)
            {
                List<Line> highlightedLines = new List<Line>(); //gets the list of lines to highlight at the end
                for (int i = 0; i < path.Count() - 1; ++i)
                {
                    int Vertex1 = path.ElementAt(i);
                    int Vertex2 = path.ElementAt(i + 1);
                    int smallerId = GetMin(Vertex1, Vertex2);
                    int largerId = GetMax(Vertex1, Vertex2);
                    string lineName = "line" + smallerId.ToString() + "to" + largerId.ToString(); //uses this to check if theres a path
                    foreach (Tuple<Line, Ellipse, Ellipse, TextBlock> edge in edgeList)
                    {
                        if (edge.Item1.Name == lineName)//detetcs if theres a path because theres a matching name
                        {
                            total += Graph.GetEdgeWeight(smallerId, largerId);
                            highlightedLines.Add(edge.Item1); //adds it to the list of edges
                        }
                    }
                }
                if (highlightedLines.Count() == path.Count() - 1)//if the path is found, then the size of the lines is always 1 less that the n. of vertices passed in
                {
                    foreach (Line line in highlightedLines)
                    {
                        line.Stroke = new SolidColorBrush(Colors.Red);
                    }
                    MessageBox.Show("Press ok to clear \nTotal Cost: " + total);
                    foreach (Line line in highlightedLines)
                    {
                        line.Stroke = new SolidColorBrush(Colors.Black);
                    }
                }
                else //if this isnt true, then a valid path was not passed in.
                {
                    MessageBox.Show("No Route/Path Exists");
                }
            }
            else
            { //if this is true, then they didnt enter a path at all
                MessageBox.Show("No Route/Path Was Entered");
            }
            
        }
        private void btnHighlightPaths_Click(object sender, RoutedEventArgs e)
        {
            labelExtraInfo.Content = "Enter a Route that you want to highlight";
            List<int> pathToHighlight = new List<int>(); //keeps the path in a list
            HighlightPath highlightPath = new HighlightPath(); //opens the highlight path window
            if (highlightPath.ShowDialog() == true) //if the window was closed how we wanted it to...
            {
                pathToHighlight = highlightPath.getPath(); //get path is a function in the other form that returns the entered path
                DijkstraHighlightPath(pathToHighlight);//runs the function on this list of IDs
            }
        }
        public void GenerateAdjList()
        {
            txAdjset.Text = Graph.coutAdjList();
        }
        public List<List<int>> GenerateAdjMat()
        {
            //populates 2d list with adjacency matrix
            List<List<int>> adjMat = Graph.GetAdjacencyMatrix();
            if (adjMat.Count() != 0)
            {
                int size = adjMat.ElementAt(0).Count();
                for (int i = 0; i < size; ++i)
                {
                    adjMat.ElementAt(i).Insert(0, i);
                }
                List<int> headers = new List<int>();
                for (int i = -1; i < size; ++i)
                {
                    if (i != -1)
                    {
                        headers.Add(i);
                    }
                    else
                    {
                        headers.Add(-1);
                    }
                }
                adjMat.Insert(0, headers);
            }
            //show the list in the form of a table
            lst.ItemsSource = adjMat;
            return adjMat;
            //return incase its necessary for future use
        }
        private void btnGenerateMatrix_Click(object sender, RoutedEventArgs e)
        {
            labelExtraInfo.Content = "Updated Adjacency Matrix.";
            GenerateAdjMat(); //function generates the matrix
        }
        private void btnToggleValencies_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
            if (valencyState == "Hidden")
            {
                ShowValencies(); //if hidden when pressed, we want to show
            }
            else if (valencyState == "Shown") 
            {
                HideValencies();//if shown when pressed we want to hide
            }
            else
            {
                throw new Exception("Invalid valencyState"); //anything else is invalid
            }
        }
        public void ShowValencies()
        {
            int sumValency = 0; //represents the total valency
            foreach (Ellipse vertex in vertexList)
            {
                TextBlock valency = new TextBlock()
                {
                    FontSize = 20,
                    Name = "Val" + vertex.Name.Substring(3), //set the name to Val(buttonId)
                    Foreground = new SolidColorBrush(Colors.Red), //set design
                    Text = Graph.GetValency(Convert.ToInt32(vertex.Name.Substring(3))).ToString() //calls the graph class, which gets the valency of a vertex 
                };
                sumValency += Convert.ToInt32(valency.Text); //adds the valency we just evaluated to the total
                double vertexX = Canvas.GetLeft(vertex) - 5; //holds x coord
                double vertexY = Canvas.GetTop(vertex) -40;//holds y coord
                Canvas.SetLeft(valency, vertexX);//setting x position
                Canvas.SetTop(valency, vertexY);//setting y position
                Canvas.SetZIndex(valency, 6);//setting z index
                mainCanvas.Children.Add(valency);//show on the canvas
                valencyList.Add(valency);//add it to the list of valency Textblocks
            }
            valencyState = "Shown"; //update the state
            labelExtraInfo.Content = "Sum of the Valencies: " + sumValency; //tell the user
        }
        public void HideValencies()
        {
            if (valencyList != null) //make sure we arent looping through a null list
            {
                foreach (TextBlock valency in valencyList)
                {
                    mainCanvas.Children.Remove(valency); //remove each of the valency textblocks
                }
                valencyList.Clear(); //Update the valency List
                valencyState = "Hidden"; //update the state
                labelExtraInfo.Content = "";
            }
        }
        public int GetMax(int a, int b)
        {
            return (a > b) ? a : b;
        }
        public int GetMin(int a, int b)
        {
            return (a < b) ? a : b;
        }

        public void DisableAllAlgoButtons()
        {
            btnDijkstras.IsEnabled = false;
            btnHighlightPaths.IsEnabled = false;
        }
        public void EnableAllAlgoButtons()
        {
            btnDijkstras.IsEnabled = true;
            btnHighlightPaths.IsEnabled = true;
        }
        public void DisableTbCtrl()
        {
            //disable all the tab controls apart from action and adjset
            tabControlAlgorithms.IsEnabled = false;
            tabControlDisplay.IsEnabled = false;
            tabControlLogs.IsEnabled = false;
            tabControlActions.IsEnabled = false;
        }
        public void EnableTbCtrl()
        {
            //enable all the tab controls
            tabControlAlgorithms.IsEnabled = true;
            tabControlDisplay.IsEnabled = true;
            tabControlLogs.IsEnabled = true;
            tabControlActions.IsEnabled = true;
        }
        public void DisableAllActionButtons()
        {
            //disables all of the action buttons but the file ones
            btnAddVertex.IsEnabled = false;
            btnAddConnection.IsEnabled = false;
            btnDeleteConnection.IsEnabled = false;
            btnDeleteVertex.IsEnabled = false;
            btnTakeScreenshot.IsEnabled = false;
            btnDragAndDrop.IsEnabled = false;
            btnDefault.IsEnabled = false;
            btnGenerateMatrix.IsEnabled = false;
            btnHighlightPaths.IsEnabled = false;
        }
        public void EnableAllActionButtons()
        {
            //enables all of the action buttons 
            btnAddVertex.IsEnabled = true;
            btnAddConnection.IsEnabled = true;
            btnDeleteConnection.IsEnabled = true;
            btnDeleteVertex.IsEnabled = true;
            btnTakeScreenshot.IsEnabled = true;
            btnDragAndDrop.IsEnabled = true;
            btnDefault.IsEnabled = true;
            btnGenerateMatrix.IsEnabled = true;
            btnHighlightPaths.IsEnabled = true;
        }
        private void btnResetColour_Click(object sender, RoutedEventArgs e)
        {
            //resets the colours of all the colour pickers
            colourPickerBackground.SelectedBrush = new SolidColorBrush(Color.FromRgb(64, 61, 61));
            colourPickerVertex.SelectedBrush = new SolidColorBrush(Colors.DodgerBlue);
            colourPickerWeight.SelectedBrush = new SolidColorBrush(Colors.White);
            colourPickerLabel.SelectedBrush = new SolidColorBrush(Colors.Black);
            colourPickerVertexStroke.SelectedBrush = new SolidColorBrush(Colors.Black);
        }
        private void ActivateButton(object btnSender) //highlight a button when its pressed
        {
            if (btnSender != null) //make sure that the button isnt null
            {
                if (currentButton != (Button)btnSender) //if the same button is not pressed
                {
                    DeactivateButton(); //'deactivate' the previous button
                    currentButton = (Button)btnSender;
                    currentButton.Background = new SolidColorBrush(btnActivatedColour); //'activate' the current button
                }
            }
        }
    }
}

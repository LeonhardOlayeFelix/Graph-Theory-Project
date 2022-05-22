using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Data;
using System.Data.OleDb;
using System.Text.RegularExpressions;

namespace Interface_2
{
    public partial class MainWindow : Window
    {
        private void ConnectVertices(Ellipse v1, Ellipse v2, int weight, bool rendering = false) //connects two vertices together 
        {
            //gets the smaller and larger vertex
            Ellipse smallerEllipse = GetMinEllipse(v1, v2);
            Ellipse largerEllipse = GetMaxEllipse(v1, v2);
            //creates the lines soon to be name as "line5to6 for example 
            string lineName = "line" + smallerEllipse.Name.Substring(3).ToString() + "to" + largerEllipse.Name.Substring(3).ToString(); 
            foreach (Tuple<Line, Ellipse, Ellipse, TextBlock> edge in edgeList)//before connecting, check if a line already exists
            {
                if (edge.Item1.Name == lineName)//if it does....
                {
                    DeleteEdge(edge);//delete the edge
                    break;
                }
            }
            if (!rendering)
            {
                Graph.AddEdge(Convert.ToInt32(v1.Name.Substring(3)), Convert.ToInt32(v2.Name.Substring(3)), weight); //update the object
            }

            //below creates the line which will be connected
            Line temp = new Line()//set properties
            {
                StrokeThickness = 4,
                Name = lineName,
                Stroke = new SolidColorBrush(Colors.Black)
            };
            Canvas.SetZIndex(temp, 0); //make sure the line is underneath everything

            Binding bindingStroke = new Binding("SelectedBrush") //this will bind the stroke of the line to the colour picker
            {
                Source = colourPickerLine,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            temp.SetBinding(Line.StrokeProperty, bindingStroke); 

            Binding bindingV1X = new Binding //this will bind the X1 coordinate of the line to the smaller ellipse
            {
                Source = smallerEllipse,
                Path = new PropertyPath(Canvas.LeftProperty),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            temp.SetBinding(Line.X1Property, bindingV1X);

            Binding bindingV1Y = new Binding //this will bind the Y1 coordinates of the line to the smaller ellipse
            {
                Source = smallerEllipse,
                Path = new PropertyPath(Canvas.TopProperty),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            temp.SetBinding(Line.Y1Property, bindingV1Y);

            Binding bindingV2X = new Binding //this will bind the X2 coordinate of the line to the larger ellipse
            {
                Source = largerEllipse,
                Path = new PropertyPath(Canvas.LeftProperty),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            temp.SetBinding(Line.X2Property, bindingV2X);

            Binding bindingV2Y = new Binding //this will bind the Y2 coordinate of the line to the larger ellipse
            {
                Source = largerEllipse,
                Path = new PropertyPath(Canvas.TopProperty),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            temp.SetBinding(Line.Y2Property, bindingV2Y);
            
            //below creates the label which represents the weight of the line
            TextBlock weightLabel = new TextBlock() //set its properties
            {
                Text = weight.ToString(),
                Name = "labelFor" + lineName,
                FontSize = 15,
            };

            Binding bindingFG = new Binding("SelectedBrush") //binds the foreground of the label to the colour picker
            {
                Source = colourPickerLabel,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            weightLabel.SetBinding(TextBlock.ForegroundProperty, bindingFG);

            Binding bindingWeightBackcolor = new Binding("SelectedBrush")//binding the back colour of the weight to the colour 
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
            Canvas.SetZIndex(weightLabel, 1); //needs to be visible above the line

            //add a new edge tuple to the list
            edgeList.Add(Tuple.Create(temp, smallerEllipse, largerEllipse, weightLabel));
            //if the weight is 0, show it as an unweighted edge
            mainCanvas.Children.Add(temp);
            if (weight != 0)
                mainCanvas.Children.Add(weightLabel);
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
        private void DeleteEdge(Tuple<Line, Ellipse, Ellipse, TextBlock> edge) //deletes an edge, and the things connected to
        {
            mainCanvas.Children.Remove(edge.Item1); //remove the line which is the first item
            mainCanvas.Children.Remove(edge.Item4);//remove the label which is the fourth element
            edgeList.Remove(edge);//remove it from the graph
            GenerateAdjList();
        }
        public Ellipse FindEllipse(int vertexId) //returns the ellipse that matches to an Id
        {
            foreach (var ctrl in mainCanvas.Children)
            {
                try //incase the ctrl is not an ellipse
                {
                    Ellipse currentEllipse = (Ellipse)ctrl;
                    if (currentEllipse.Name.Substring(3) == vertexId.ToString()) //when youve found a match
                    {
                        return currentEllipse; 
                    }
                }
                catch { }
            }
            return null;
        }
        public TextBlock FindLabel(int vertexId)
        {
            foreach (var ctrl in mainCanvas.Children)
            {
                try
                {
                    TextBlock currentLabel = (TextBlock)ctrl;
                    if (currentLabel.Name.Substring(8) == vertexId.ToString())
                    {
                        return currentLabel;
                    }
                }
                catch { }
            }
            return null;
        }
        private void btnResetComponentShape_Click(object sender, RoutedEventArgs e)
        {
            //resets the sliders back to their original form
            ActivateButton(sender);
            vertexDiameterSlider.Value = 40;
            weightAndLabelFontSizeSlider.Value = weightAndLabelFontSizeSlider.Minimum;
        }
        public HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>> GetListOfEdgesFromVertex(Ellipse activeVertex) //gets all of the edges coming out of a vertex
        {
            HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>> listOfEdges = new HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>>(); //data to return
            foreach (Ellipse vertex in vertexList) //loop through vertex list
            {
                if (vertex != activeVertex)//don't check for an edge from itself to itself
                {
                    Ellipse largerEllipse = GetMaxEllipse(vertex, activeVertex);
                    Ellipse smallerEllipse = GetMinEllipse(vertex, activeVertex); 
                    string lineNameToFind = "line" + smallerEllipse.Name.Substring(3).ToString() + "to" + largerEllipse.Name.Substring(3).ToString(); //the name of the line that we are expecting to find
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
        private void btnRouteInspStartAndEnd_Click(object sender, RoutedEventArgs e)
        {
            ShowValencies();
            labelExtraInfo.Content = "Choose a START vertex with an ODD valency";
            ActivateButton(sender);
        }
        private void btnRouteInspStartAtEnd_Click(object sender, RoutedEventArgs e)
        {
            if (vertexList.Count() != 0)
            {
                ActivateButton(sender);
                Tuple<List<Tuple<int, int>>, int> result = Graph.RInspStartAtEnd();//returns the edges to repeated (1) and the cost of repitition (2)
                if (!Graph.IsConnected()) //have to make sure that the graph is connected first
                {
                    MessageBox.Show("The graph is not connected");
                }
                else if (Graph.IsEulerian()) //if the graph is already eulerian then it will be traversable
                {
                    MessageBox.Show("This graph is traversable");
                }
                else if (result == null)
                {
                    MessageBox.Show("Appropriate graph was not entered"); //in this case, there was an unexpected error

                }
                else
                {
                    List<Tuple<int, int>> edgesToRepeat = result.Item1; //first item of the tuple
                    int cost = result.Item2;//second item of the tuple
                    RouteInspHighlightPath(edgesToRepeat, cost); //highlights the edges to be repeated and presents the cost
                }
            }
            HideValencies();
        }
        private void btnCreateNewGraph_Click(object sender, RoutedEventArgs e) //creates a new graph
        {
            string name = "";
            NameCreatedGraph nameGraphWindow = new NameCreatedGraph(); //create an instance of the new window
            nameGraphWindow.ShowDialog(); //opens a new window
            if (nameGraphWindow.DialogResult == true) //if they pressed ok rather than the exit button
            {
                name = nameGraphWindow.txBoxGraphName.Text; //re-initialise everything:
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
            Graph = new Network();
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
            DisableTbCtrl();
        }
        public void ResetSelectionCounts()
        {
            buttonSelectionCount = 0;
            dijkstraSelectionCount = 0;
            rInspSelectionCount = 0;
        }
        public void DeleteGraph()
        {
            //set all of the variables to null
            mainCanvas.Children.Clear();
            btnDeleteGraph.IsEnabled = false;
            labelGraphName.Content = "";
            Graph = new Network();
            ResetSelectionCounts();
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
        private void btnRevertPositions_Click(object sender, RoutedEventArgs e)
        {
            labelExtraInfo.Content = "";
            foreach (var ctrl in mainCanvas.Children)
            {
                try
                {
                    Ellipse currentEllipse = (Ellipse)ctrl;
                    int vertexID = Convert.ToInt32(currentEllipse.Name.Substring(3));
                    double originalX = Graph.GetVertex(vertexID).Position.originalX; //get original position
                    double originalY = Graph.GetVertex(vertexID).Position.originalY; //get original position
                    Graph.GetVertex(vertexID).Position.SetPosition(originalX, originalY); // update their positions in the class
                    Canvas.SetLeft(currentEllipse, originalX);
                    Canvas.SetTop(currentEllipse, originalY);
                    TextBlock label = FindLabel(vertexID);
                    Canvas.SetLeft(label, originalX - 4); //update that label too
                    Canvas.SetTop(label, originalY - 9);
                    foreach (Tuple<Line, Ellipse, Ellipse, TextBlock> edge in edgeList)
                    {
                        if (edge.Item2 == currentEllipse || edge.Item3 == currentEllipse) //look for the weight that matches to vertexes
                        {
                            double MidPointX = (Canvas.GetLeft(edge.Item2) + Canvas.GetLeft(edge.Item3)) / 2;
                            double MidPointY = (Canvas.GetTop(edge.Item2) + Canvas.GetTop(edge.Item3)) / 2; //update it to the midpoint of the line as it moves each time
                            Canvas.SetLeft(edge.Item4, MidPointX - 4);
                            Canvas.SetTop(edge.Item4, MidPointY - 9);
                        }

                    }
                }
                catch
                {

                }
            }
        }
        private void mouseMove(object sender, MouseEventArgs e)
        {
            //monitors the mouse as it hovers over a vertex
            if (e.LeftButton == MouseButtonState.Pressed && currentButton == btnDragAndDrop)//if, whilst hovering, they press the vertex
            {
                ellipseToDrop = sender as Ellipse;
                ellipseToDrop.Fill = new SolidColorBrush(Colors.Red);
                DragDrop.DoDragDrop(sender as Ellipse, sender as Ellipse, DragDropEffects.Move); //start the drag function on this vertex
                RevertEllipseColour();
            }
        }
        private void mainCanvas_DragOver(object sender, DragEventArgs e)
        {

            //if the mouse the vertex is being dragged
            int ellipseToDropID = Convert.ToInt32(ellipseToDrop.Name.Substring(3));
            Point dropPosition = e.GetPosition(mainCanvas); //current position of the place its being dragged
            Canvas.SetLeft(ellipseToDrop, dropPosition.X);//updates the x coordinate every time its dragged
            Graph.GetVertex(ellipseToDropID).Position.X = dropPosition.X; //udpate its position in the class too
            Canvas.SetTop(ellipseToDrop, dropPosition.Y);//updates the y coordinate ever time its dragged
            Graph.GetVertex(ellipseToDropID).Position.Y = dropPosition.Y; //update its position in the class too
            labelExtraInfo.Content = "Drag position: " + Graph.GetVertex(ellipseToDropID).Position.GetPositionTuple();
            TextBlock label = FindLabel(Convert.ToInt32(ellipseToDrop.Name.Substring(3)));
            Canvas.SetLeft(label, dropPosition.X - 4); //update that label too
            Canvas.SetTop(label, dropPosition.Y - 9);
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
        private void mainCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e) //event to change the size of the vertices
        {
            if (e.Delta > 0) //if mouse is scrolled up, increase slider value
            {
                vertexDiameterSlider.Value += 2; //this increases the vertex diameter (bound)
            }
            else if (e.Delta < 0)//if mouse is scrolled down, increase slider value
            {
                vertexDiameterSlider.Value -= 2;//this decreases the vertex diameter (bound)
            }
        }
        public void GenerateAdjList()
        {
            txAdjset.Text = Graph.PrintAdjList();
        }
        public List<List<int>> GenerateAdjMat() //makes the adjacenct list appear in the provided area
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
        private void btnPrims_Click(object sender, RoutedEventArgs e)
        {
            labelExtraInfo.Content = "Choose a start Vertex";
            ActivateButton(sender);
        }
        private void btnKruskals_Click(object sender, RoutedEventArgs e) //if the user wants to run kruskals algorithm
        {
            ActivateButton(sender); 
            HideValencies();
            if (!Graph.IsConnected())
            {
                MessageBox.Show("The graph is not connected");
            }
            else
            {
                List<Tuple<int, int, int>> mst = Graph.Kruskals();
                mstHighlightPath(mst); //highlight the MST
            }
        }
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {

        }
        private void cbAutoGenEdges_Checked(object sender, RoutedEventArgs e) //When a connection is made, auto generates a random weight
        {
            cbAutoGenEdgesValue.IsChecked = false; //only one check box can be selected at a time
        }
        private void cbAutoGenEdgesValue_Checked(object sender, RoutedEventArgs e) //when a connection is made, auto generates the entered weight
        {
            cbAutoGenEdges.IsChecked = false;//ditto
        }
        private void cbAlphabet_Checked(object sender, RoutedEventArgs e)
        {
            int maxNumber = alphabet.Count(); //the highest number of uniquely representable nodes using the alphabet
            if (Graph.GetMaxNodeID() >= maxNumber)
            {
                MessageBox.Show("Not enough Letters in the alphabet to represent each vertex"); 
                cbAlphabet.IsChecked = false;
            }
            else
            {
                for (int i = 0; i < Graph.GetMaxNodeID() + 1; ++i) //loop through all the nodes
                {
                    if (FindEllipse(i) != null) //check in advanced that this operation wont return null
                    {
                        TextBlock label = FindLabel(Convert.ToInt32(FindEllipse(i).Name.Substring(3))); //find the label of each vertex
                        if (label != null) //incase the vertex and label were deleted
                        {
                            label.Text = alphabet.ElementAt(i); //change the current label to the alphabet
                        }
                    }
                }
            }
        }
        private void cbAlphabet_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Graph.GetMaxNodeID() + 1; ++i) //loop through all the nodes
            {
                if (FindEllipse(i) != null)//check in advanced that this operation wont return null
                {
                    TextBlock label = FindLabel(Convert.ToInt32(FindEllipse(i).Name.Substring(3)));//find the label of each vertex
                    if (label != null)
                    {
                        label.Text = i.ToString(); //change the current label to the number
                    }
                }
            }
        }
        private void txAutoWeight_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text); //controls the input allowed in the textbox
        }
        private void colourPickerHighlight_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HighlightColour = (SolidColorBrush)colourPickerHighlight.SelectedBrush;
        }
        public int GetMax(int a, int b)
        {
            return (a > b) ? a : b;
        }
        public int GetMin(int a, int b)
        {
            return (a < b) ? a : b;
        }
    }
}

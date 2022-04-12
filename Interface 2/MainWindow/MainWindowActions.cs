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

namespace Interface_2
{
    public partial class MainWindow : Window
    {
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
            Binding bindingStroke = new Binding("SelectedBrush")
            {
                Source = colourPickerLine,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            temp.SetBinding(Line.StrokeProperty, bindingStroke);
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
            };
            Binding bindingBG = new Binding("SelectedBrush")
            {
                Source = colourPickerLabel,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            weightLabel.SetBinding(TextBlock.ForegroundProperty, bindingBG);

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
            Canvas.SetZIndex(weightLabel, 1); //needs to be visible above all else

            //add a new edge tuple to the list
            edgeList.Add(Tuple.Create(temp, smallerEllipse, largerEllipse, weightLabel));
            //update logs

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
            edgeList.Remove(edge);//remove it from the graph
            GenerateAdjList();
        }
        public Ellipse FindElipse(int vertexId)
        {
            foreach (var ctrl in mainCanvas.Children)
            {
                try
                {
                    Ellipse currentEllipse = (Ellipse)ctrl;
                    if (currentEllipse.Name.Substring(3) == vertexId.ToString())
                    {
                        return currentEllipse;
                    }
                }
                catch
                {

                }
            }
            return null;
        }
        private void btnResetComponentShape_Click(object sender, RoutedEventArgs e)
        {
            //resets the sliders back to their original form
            edgeThicknessSlider.Value = edgeThicknessSlider.Minimum;
            vertexDiameterSlider.Value = vertexDiameterSlider.Minimum;
            weightAndLabelFontSizeSlider.Value = weightAndLabelFontSizeSlider.Minimum;
            ActivateButton(sender);
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
        private void btnCreateNewGraph_Click(object sender, RoutedEventArgs e)
        {
            string name = "";
            NameCreatedGraph nameGraphWindow = new NameCreatedGraph();
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
            else if (e.Delta < 0)//if mouse is scrolled down, increase slider value
            {
                vertexDiameterSlider.Value -= 2;//this decreases the vertex diameter (bound)
            }
        }
        public void GenerateAdjList()
        {
            txAdjset.Text = Graph.PrintAdjList();
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
        private void btnPrims_Click(object sender, RoutedEventArgs e)
        {
            labelExtraInfo.Content = "Choose a start Vertex";
            ActivateButton(sender);
        }
        private void btnKruskals_Click(object sender, RoutedEventArgs e)
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
                mstHighlightPath(mst);
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Timers;

namespace Interface_2
{
    public partial class MainWindow : Window
    {
        public void AddConnectionEven(Ellipse vertexToConnectTo)
        {
            vertexToConnectTo.Fill = HighlightColour;
            ConnectEdges connectEdges = new ConnectEdges();
            if (lastSelectedVertex == vertexToConnectTo) //if they are connecting it to itself, do nothing
            {
                EnableAllActionButtons();
                EnableTabControl();
                labelExtraInfo.Content = "";
            }
            else if ((bool)cbAutoGenEdges.IsChecked) //if the auto generate weight randomly button is checked
            {
                Random random = new Random();
                int weight = random.Next(1, 26);
                ConnectVertices(lastSelectedVertex, vertexToConnectTo, weight);
                labelExtraInfo.Content = "";
                EnableTabControl();
                EnableAllActionButtons();
            }
            else if ((bool)cbAutoGenEdgesValue.IsChecked) //if the auto generate weight to 0 button is checked
            {
                ConnectVertices(lastSelectedVertex, vertexToConnectTo, (txAutoWeight.Text.Length == 0) ? 0 : Convert.ToInt32(txAutoWeight.Text));//conditional operator
                labelExtraInfo.Content = "";
                EnableTabControl();
                EnableAllActionButtons();
            }
            else if (connectEdges.ShowDialog() == true) // otherwise, open a new form and get the weight
            {
                int weight = Convert.ToInt32(connectEdges.txWeight.Text); //get weight from text box
                ConnectVertices(lastSelectedVertex, vertexToConnectTo, weight); //add the edge
                labelExtraInfo.Content = "";
                EnableTabControl();
                EnableAllActionButtons();
            }
            else
            {
                buttonSelectionCount -= 2; //if they closed the form without providing weight, decrement it so they can press another vertex
                labelExtraInfo.Content = "";
                EnableTabControl();
                EnableAllActionButtons();

            }
            RevertEllipseColour();
            btnDeleteGraph.IsEnabled = true;
            btnSaveGraph.IsEnabled = true;
            btnLoadGraph.IsEnabled = true;
        }
        public void AddConnectionOdd(Ellipse vertexToConnectFrom)
        {
            vertexToConnectFrom.Fill = HighlightColour;
            lastSelectedVertex = vertexToConnectFrom;
            labelExtraInfo.Content = "From vertex " + FindLabel(Convert.ToInt32(lastSelectedVertex.Name.Substring(3))).Text + " to.....";
            DisableTabControl();
            DisableAllActionButtons();
            btnSaveGraph.IsEnabled = false;
            btnLoadGraph.IsEnabled = false;
        }
        public void BreadthFirst(Ellipse startVertex)
        {
            RevertLineColour();
            RevertEllipseColour();
            startVertex.Fill = HighlightColour;
            int startVertexId = Convert.ToInt32(startVertex.Name.Substring(3)); //id of the start vertex
            if (Graph.GetAdjVertices(startVertexId).Count() != 0) //make sure the node has atleast one edge
            {
                Tuple<List<Tuple<int, int>>, List<int>> result = Graph.BreadthFirst(startVertexId);
                List<Tuple<int, int>> edges = result.Item1;
                TraversalHighlightPath(edges); //highlight the traversal order
                List<int> traversalOrder = result.Item2;
                string outputString = "";
                for (int i = 0; i < traversalOrder.Count(); ++i)
                {
                    outputString += FindLabel(traversalOrder[i]).Text;
                    if (i != traversalOrder.Count() - 1) { outputString += "=>"; }//create a string containing the traversal order
                }
                txExtraInfo2.Text = "Traversal Order: " + outputString;
            }
        }
        public void DeleteVertex(Ellipse activeVertex)
        {
            HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>> listOfEdgesToRemove = GetListOfEdgesFromVertex(activeVertex);//gets list of edges we need to remove with the vertex
            Graph.RemoveVertex(Convert.ToInt32(activeVertex.Name.Substring(3))); //update the class
                                                                                 //loop through lines and delete any lines that come out of it
            foreach (Tuple<Line, Ellipse, Ellipse, TextBlock> edge in listOfEdgesToRemove)
            {
                DeleteEdge(edge, false, true); //calls function to delete the edge from cavas
            }
            //mainCanvas.Children.Remove(activeVertex);
            InitiateDeleteVertexStoryboard(activeVertex, TimeSpan.FromSeconds(0.2)); //then delete the vertex
            vertexList.Remove(activeVertex);//delete it from the list
            TextBlock label = FindLabel(Convert.ToInt32(activeVertex.Name.Substring(3)));
            mainCanvas.Children.Remove(label);
            vertexTxBoxList.Remove(label);
            txAdjset.Text = Graph.PrintAdjList();
        }
        private void DeleteEdge(Tuple<Line, Ellipse, Ellipse, TextBlock> edge, bool rendering = false, bool deletingVertex = false) //deletes an edge, and the things connected to
        {
            if (!rendering && !deletingVertex)
            {
                Graph.RemoveEdge(Convert.ToInt32(edge.Item2.Name.Substring(3)), Convert.ToInt32(edge.Item3.Name.Substring(3))); //update the class graph
            }
            mainCanvas.Children.Remove(edge.Item1); //remove the line which is the first item
            mainCanvas.Children.Remove(edge.Item4);//remove the label which is the fourth element
            InitiateDeleteLineStoryboard(edge.Item1, TimeSpan.FromSeconds(0.1));
            edgeList.Remove(edge);//remove it from the graph
            GenerateAdjList();
        }
        public void DepthFirst(Ellipse startVertex)
        {
            RevertEllipseColour();
            RevertLineColour();
            startVertex.Fill = HighlightColour;
            int startVertexId = Convert.ToInt32(startVertex.Name.Substring(3));//id of the start vertex
            if (Graph.GetAdjVertices(startVertexId).Count() != 0) //make sure the node has atleast one edge
            {
                Tuple<List<Tuple<int, int>>, bool, List<int>> result = Graph.DepthFirst(startVertexId);
                List<Tuple<int, int>> edges = result.Item1;
                TraversalHighlightPath(edges); //highlight the traversal order
                List<int> traversalOrder = result.Item3;
                string outputString = "";
                for (int i = 0; i < traversalOrder.Count(); ++i)
                {
                    outputString += FindLabel(traversalOrder[i]).Text;
                    if (i != traversalOrder.Count() - 1) { outputString += "=>"; }//create a string containing the traversal order
                }
                txExtraInfo2.Text = "Traversal Order: " + outputString;
            }
        }
        public void DijkstraEven(Ellipse endVertex)
        {
            int vId = Convert.ToInt32(endVertex.Name.Substring(3));
            if (startVertex == vId) //if they are path finding it to itself, do nothing
            {
                RevertEllipseColour();
                EnableAllActionButtons();
                EnableAllAlgorithmButtons();
                EnableTabControl();
                btnSaveGraph.IsEnabled = false;
                btnLoadGraph.IsEnabled = false;
                labelExtraInfo.Content = "";
            }
            else
            {
                endVertex.Fill = HighlightColour;
                try
                {
                    List<int> path = Graph.DijkstrasAlgorithmShort(startVertex, vId).Item1; //get the path from the method
                    DijkstraHighlightPath(path);
                    labelExtraInfo.Content = "";
                }
                catch (NullReferenceException) //this means there was no path
                {
                    MessageBox.Show("There is no shortest path from these two points");
                }
                EnableTabControl();
                EnableAllAlgorithmButtons();
            }
        }
        public void DijkstraOdd(Ellipse startVertex)
        {
            DisableTabControl();
            DisableAllAlgorithmButtons();
            RevertEllipseColour();
            RevertLineColour();
            startVertex.Fill = HighlightColour;
            this.startVertex = Convert.ToInt32(startVertex.Name.Substring(3));
            labelExtraInfo.Content = "Shortest Path from " + FindLabel(Convert.ToInt32(this.startVertex)).Text + " to...";
        }
        public void HighlightPaths(Ellipse activeVertex)
        {
            List<int> adjVertices = Graph.GetAdjVertices(Convert.ToInt32(activeVertex.Name.Substring(3)));
            if (livePath.Count != 0)
            {
                if (!adjVertices.Contains(livePath.Last()))
                {
                    return;
                }
            }
            activeVertex.Fill = HighlightColour; //highlight the vertex pressed
            int activeVertexId = Convert.ToInt32(activeVertex.Name.Substring(3));
            livePath.Add(activeVertexId); //add the vertex to the path that they want to highlight
            if (livePath.Count() > 1) //if there is only one vertex in the path then dont do anything
            {
                if ((livePath.Last() == livePath[livePath.Count - 2]) || Graph.GetEdgeWeight(livePath.Last(), livePath[livePath.Count() - 2]) == -1) //if they are attempting to press the same vertex
                {
                    livePath.RemoveAt(livePath.Count() - 1); //remove the vertex from the list
                }
                else
                {
                    DijkstraHighlightPath(livePath); //highlight the path
                }
            }
        }
        public void Prims(Ellipse startVertex)
        {
            ClearHighlightedLines();
            RevertEllipseColour();
            startVertex.Fill = HighlightColour;
            int startVertexID = Convert.ToInt32(startVertex.Name.Substring(3));
            if (Graph.IsConnected())
            {
                List<Tuple<int, int, int>> mst = Graph.Prims(startVertexID);
                mstHighlightPath(mst);//highlight the path
            }
            else
            {
                MessageBox.Show("The graph is not connected.");
            }
        }
        public void RouteInspStartAndEndEven(Ellipse endVertex)
        {
            int endVertexId = Convert.ToInt32(endVertex.Name.Substring(3));
            if (Graph.GetValency(endVertexId) % 2 == 1) { endVertex.Fill = HighlightColour; }
            if (Graph.GetValency(endVertexId) % 2 == 0) //if the valency of the end vertex is odd, the algorithm isnt useful here
            {
                MessageBox.Show("The start and end Vertex must have an ODD valency.");
                rInspSelectionCount -= 1; //decrement it as if that selection didnt count
            }
            else if (rInspStart == endVertexId) //if they are path finding to itself, do nothing
            {
                EnableAllActionButtons();
                EnableTabControl();
                EnableAllAlgorithmButtons();
                RevertEllipseColour();
                btnLoadGraph.IsEnabled = false;
                btnSaveGraph.IsEnabled = false;
                labelExtraInfo.Content = "";
            }
            else
            {
                Tuple<List<Tuple<int, int>>, int> result = Graph.RInspStartAndEnd(rInspStart, endVertexId);//returns the edges to repeated (1) and the cost of repitition (2)
                if (!Graph.IsConnected()) //have to make sure that the graph is connected first
                {
                    MessageBox.Show("The graph is not connected");
                }
                else if (Graph.IsSemiEulerian()) //if the graph is already semi eulerian then it will be already traversable
                {
                    txExtraInfo2.Text = "No Extra Edges Need To Be Added.";
                    EnableAllActionButtons();
                    EnableTabControl();
                    EnableAllAlgorithmButtons();
                    labelExtraInfo.Content = "";
                }
                else if (result == null)
                {
                    MessageBox.Show("Appropriate graph was not entered"); //in this case, there was an unexpected eror
                }
                else
                {
                    List<Tuple<int, int>> edgesToRepeat = result.Item1; //first item of the tuple reps edges to repeat
                    int cost = result.Item2;//second item of the tuple reps the total cost
                    RevertEllipseColour();
                    RouteInspHighlightPath(edgesToRepeat, cost); //highlights the edges to be repeated and presents the cost
                    HideValencies();
                }
                EnableTabControl();
                EnableAllActionButtons();
                EnableAllAlgorithmButtons();
            }
        }
        public void RouteInspStartAndEndOdd(Ellipse startVertex)
        {
            RevertLineColour();
            RevertEllipseColour();
            DisableTabControl();
            DisableAllAlgorithmButtons();
            DisableAllActionButtons();
            rInspStart = Convert.ToInt32(startVertex.Name.Substring(3));
            if (Graph.GetValency(rInspStart) % 2 == 0) //make sure the vertex has an odd valency
            {
                EnableAllActionButtons();
                EnableTabControl();
                EnableAllAlgorithmButtons();
                MessageBox.Show("The start and end vertex must have an odd valency");
                labelExtraInfo.Content = "Choose a START vertex with ODD valency";
                rInspSelectionCount -= 1; //if it doesnt, decrement to act as if that selection didnt count
            }
            else
            {
                startVertex.Fill = HighlightColour;
                labelExtraInfo.Content = "Choose an END vertex with ODD valency.";
            }
        }
        public void RouteInspectionStartAtEnd()
        {
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
        public void RevertOneVertexPosition(Ellipse currentEllipse)
        {
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
    }
}

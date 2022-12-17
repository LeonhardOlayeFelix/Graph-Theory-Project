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
        /// <summary>
        /// Highlights the path that dijkstras algorithm has resulted in
        /// </summary>
        /// <param name="route"></param>
        /// <param name="livePathhighlighting">True only if the Highlight path button is activated</param>
        public void DijkstraHighlightRoute(List<int> route, bool livePathhighlighting = false) 
        {

            int total = 0;
            if (route.Count() > 1)
            {
                List<Line> highlightedLines = new List<Line>(); //converts the vertices list into lines
                for (int i = 0; i < route.Count() - 1; ++i)
                {
                    int Vertex1 = route.ElementAt(i);
                    int Vertex2 = route.ElementAt(i + 1);
                    int smallerId = GetMin(Vertex1, Vertex2);
                    int largerId = GetMax(Vertex1, Vertex2);
                    string lineName = "line" + smallerId.ToString() + "to" + largerId.ToString(); //uses this to check if theres a path
                    foreach (Tuple<Line, Ellipse, Ellipse, TextBlock> edge in edgeList)
                    {
                        if (edge.Item1.Name == lineName)//detetcs if theres a path because theres a matching name
                        {
                            total += graph.GetEdgeWeight(smallerId, largerId);
                            highlightedLines.Add(edge.Item1); //adds it to the list of edges
                        }
                    }
                }
                if (highlightedLines.Count() == route.Count() - 1)//if the path is found, then the size of the array is always 1 less than the n. of vertices passed in
                {
                    string routeString = "";
                    if (livePathhighlighting)
                    {
                        for (int i = 0; i < highlightedLines.Count(); ++i)
                        {
                            highlightedLines[i].Stroke = HighlightColour;
                            routeString += FindLabel(Convert.ToInt32(route[i].ToString())).Text + "=>";
                        }
                        routeString += FindLabel(Convert.ToInt32(route[route.Count() - 1])).Text;
                        txExtraInfo2.Text = "Traversal Order:\n" + routeString + "\nCost: " + total;
                        return;
                    }
                    for (int i = 0; i < highlightedLines.Count(); ++i)
                    {
                        routeString += FindLabel(Convert.ToInt32(route[i].ToString())).Text + "=>";
                    }
                    routeString += FindLabel(Convert.ToInt32(route[route.Count() - 1])).Text;
                    txExtraInfo2.Text = "Traversal Order:\n" + routeString + "\nCost: " + total;
                    InitiateHighlightPathStoryboard(route, TimeSpan.FromSeconds(1));
                    InitiatePathWalkerStoryboard(route);
                }
                else //if this isnt true, then a valid path was not passed in.
                {
                    MessageBox.Show("No Edge between these vertices was found");
                    RevertEllipseColour();
                }
            }
            else
            { //if this is true, then they didnt enter a path at all
                MessageBox.Show("No Edge between these vertices was found");
            }
        }
        /// <summary>
        /// Highlights the path that a minimum spanning tree algorithm has resulted in
        /// </summary>
        /// <param name="edges">The edges that will be highlighted tuple(vertex, vertex, weight)</param>
        /// <returns></returns>
        public bool mstHighlightTree(List<Tuple<int, int, int>> edges, int cost = -1)
        {
            RevertLineColour();
            if (edges.Count() != 0)
            {
                int total = 0;
                List<Line> highlightedLines = new List<Line>(); //gets the list of lines to highlight at the end
                foreach (Tuple<int, int, int> edge in edges)
                {
                    int smallerId = GetMin(edge.Item1, edge.Item2);
                    int largerId = GetMax(edge.Item1, edge.Item2);
                    string lineName = "line" + smallerId.ToString() + "to" + largerId.ToString(); //uses this to check if theres a path
                    foreach (Tuple<Line, Ellipse, Ellipse, TextBlock> line in edgeList)
                    {
                        if (line.Item1.Name == lineName)//detetcs if theres a path because theres a matching name
                        {
                            highlightedLines.Add(line.Item1); //adds it to the list of edges
                            total += Convert.ToInt32(line.Item4.Text);
                        }
                    }
                }
                if ((bool)cbManualGeneration.IsChecked)
                {
                    for (int i = 0; i < highlightedLines.Count(); ++i)
                    {
                        InitiateHighlightLineStoryboard(highlightedLines[i], TimeSpan.FromSeconds(1));
                        if (i != highlightedLines.Count() - 1)
                        {
                            MessageBox.Show("Press ok to show next edge");
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < highlightedLines.Count(); ++i)
                    {
                        InitiateHighlightLineStoryboard(highlightedLines[i], TimeSpan.FromSeconds(1));
                    }
                }
                txExtraInfo2.Text = "Minimum Spanning Tree Weight: " + Convert.ToString((cost == -1)? total: cost);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Highlights the edges that a route inspection algorithm has deemed needing repeating
        /// </summary>
        /// <param name="edges">The edges that will be highlighted tuple(vertex, vertex)</param>
        /// <param name="cost">The cost of repeating the edges</param>
        public void RouteInspHighlightPath(List<Tuple<int, int>> edges, int cost)
        {
            List<Line> highlightedLines = new List<Line>(); //gets the list of lines to highlight at the end
            foreach (Tuple<int, int> edge in edges)
            {
                int smallerId = GetMin(edge.Item1, edge.Item2);
                int largerId = GetMax(edge.Item1, edge.Item2);
                string lineName = "line" + smallerId.ToString() + "to" + largerId.ToString(); //uses this to check if theres a path
                foreach (Tuple<Line, Ellipse, Ellipse, TextBlock> line in edgeList)
                {
                    if (line.Item1.Name == lineName)//detetcs if theres a path because theres a matching name
                    {
                        FindEllipse(smallerId).Fill = HighlightColour;
                        FindEllipse(largerId).Fill = HighlightColour;
                        highlightedLines.Add(line.Item1); //adds it to the list of edges
                    }
                }
            }
            for (int i = 0; i < highlightedLines.Count(); ++i)
            {
                InitiateHighlightLineStoryboard(highlightedLines[i], TimeSpan.FromSeconds(1));
            }
            string info = "Edges to repeat:\n";
            foreach (Tuple<int, int> edge in edges)
            {
                info += "(" + FindLabel(edge.Item1).Text + ", " + FindLabel(edge.Item2).Text + ")" + "   ";
            }
            info += "\nCost: " + (cost + graph.GetSumOfWeights());
            txExtraInfo2.Text = info;
        }
        /// <summary>
        /// Highlighs the edges that a breadth or depth first search has specified
        /// </summary>
        /// <param name="edges">The edges that will be highlighted</param>
        private void TraversalHighlightPath(List<Tuple<int, int>> edges)
        {
            List<Line> highlightedLines = new List<Line>(); //gets the list of lines to highlight at the end
            foreach (Tuple<int, int> edge in edges)
            {
                int smallerId = GetMin(edge.Item1, edge.Item2);
                int largerId = GetMax(edge.Item1, edge.Item2);
                string lineName = "line" + smallerId.ToString() + "to" + largerId.ToString(); //uses this to check if theres a path
                foreach (Tuple<Line, Ellipse, Ellipse, TextBlock> line in edgeList)
                {
                    if (line.Item1.Name == lineName)//detetcs if theres a path because theres a matching name
                    {
                        highlightedLines.Add(line.Item1); //adds it to the list of edges
                    }
                }
            }
            for (int i = 0; i < highlightedLines.Count(); ++i)
            {
                InitiateHighlightLineStoryboard(highlightedLines[i], TimeSpan.FromSeconds(1));
                if (i != highlightedLines.Count() - 1)
                {
                    MessageBox.Show("Press ok to show next edge");
                }
            }

        }
    }
}

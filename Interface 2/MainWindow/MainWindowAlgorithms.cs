﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Interface_2
{

    public partial class MainWindow : Window
    {
        private void btnDijkstrasShort_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click a vertex to find the lowest cost route to the next clicked vertex";
            ActivateButton(sender);
        }
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
                highlightedLines[i].Stroke = HighlightColour;
            }
            string info = "Edges to repeat:\n";
            foreach (Tuple<int, int> edge in edges)
            {
                info += "(" + FindLabel(edge.Item1).Text + ", " + FindLabel(edge.Item2).Text + ")" + "   ";
            }
            info += "\nCost: " + (cost + Graph.GetSumOfWeights());
            txExtraInfo2.Text = info;
        }
        public bool mstHighlightPath(List<Tuple<int, int, int>> edges) //done
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
                            total += Graph.GetEdgeWeight(smallerId, largerId);
                            highlightedLines.Add(line.Item1); //adds it to the list of edges
                            total += Convert.ToInt32(line.Item4.Text);
                        }
                    }
                }
                for (int i = 0; i < highlightedLines.Count(); ++i)
                {
                    highlightedLines[i].Stroke = HighlightColour;
                    if (i != highlightedLines.Count() - 1) { MessageBox.Show("Press OK to show next edge"); }
                }
                txExtraInfo2.Text = "Minimum Spanning Tree Weight: " + total / 2;
                return true;
            }
            return false;
        }
        public void DijkstraHighlightPath(List<int> path, bool livePathhighlighting = false) //a path of vertexIds, in the order they want to be traversed //done
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
                if (highlightedLines.Count() == path.Count() - 1)//if the path is found, then the size of the array is always 1 less than the n. of vertices passed in
                {
                    string pathString = "";
                    if (livePathhighlighting)
                    {
                        for (int i = 0; i < highlightedLines.Count(); ++i)
                        {
                            highlightedLines[i].Stroke = HighlightColour;
                            pathString += FindLabel(Convert.ToInt32(path[i].ToString())).Text + "=>";
                        }
                        pathString += FindLabel(Convert.ToInt32(path[path.Count() - 1])).Text;
                        txExtraInfo2.Text = "Traversal Order:\n" + pathString + "\nCost: " + total;
                        return;
                    }
                    for (int i = 0; i < highlightedLines.Count(); ++i)
                    {
                        highlightedLines[i].Stroke = HighlightColour;
                        pathString += FindLabel(Convert.ToInt32(path[i].ToString())).Text + "=>"; //change the colour and update the path string
                        if (i != highlightedLines.Count() - 1) { MessageBox.Show("Press OK to show next edge"); }
                    }
                    pathString += FindLabel(Convert.ToInt32(path[path.Count() - 1])).Text;
                    txExtraInfo2.Text = "Traversal Order:\n" + pathString + "\nCost: " + total;

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

        public void RevertLineColour() //rebinds the colour of the lines to the colour picker
        {
            foreach (var edge in edgeList)
            {
                Binding bindingStroke = new Binding("SelectedBrush")
                {
                    Source = colourPickerLine,
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                edge.Item1.SetBinding(Line.StrokeProperty, bindingStroke);
            }
        }
        public void RevertEllipseColour()
        {
            foreach (var ctrl in mainCanvas.Children)
            {
                try
                {
                    Ellipse currentEllipse = (Ellipse)ctrl;
                    Binding bindingFill = new Binding("SelectedBrush")
                    {
                        Source = colourPickerVertex,
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };
                    currentEllipse.SetBinding(Ellipse.FillProperty, bindingFill);
                }
                catch
                {

                }
            }
        }
        private void btnHighlightPaths_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
            labelExtraInfo.Content = "Click on the vertices that you want the path to connect.";
        }
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
                highlightedLines[i].Stroke = HighlightColour;
                if (i != highlightedLines.Count() - 1)
                {
                    MessageBox.Show("Press ok to show next edge");
                }
            }

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
                    Text = Graph.GetValency(Convert.ToInt32(vertex.Name.Substring(3))).ToString() //calls the graph class, which gets the valency of a vertex 
                };
                Binding bindingBG = new Binding("SelectedBrush")
                {
                    Source = colourPickerLabel,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay
                };
                valency.SetBinding(TextBlock.ForegroundProperty, bindingBG);
                sumValency += Convert.ToInt32(valency.Text); //adds the valency we just evaluated to the total
                double vertexX = Canvas.GetLeft(vertex) - 5; //holds x coord
                double vertexY = Canvas.GetTop(vertex) - 40;//holds y coord
                Canvas.SetLeft(valency, vertexX);//setting x position
                Canvas.SetTop(valency, vertexY);//setting y position
                Canvas.SetZIndex(valency, int.MaxValue);//setting z index
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
    }
}

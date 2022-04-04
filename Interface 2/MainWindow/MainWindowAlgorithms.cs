using System;
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
        private void btnDijkstras_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click a vertex to find the lowest cost route to the next clicked vertex";
            ActivateButton(sender);
        }
        public void PrimsHighlightPath(List<Tuple<int, int, int>> edges)
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
                    }
                }
            }
            foreach(Line line in highlightedLines)
            {
                line.Stroke = new SolidColorBrush(Colors.Red);
            }
            MessageBox.Show("Press ok to clear Minimum Spanning Tree");
            foreach (Line line in highlightedLines)
            {
                line.Stroke = new SolidColorBrush(Colors.Black);//reset the colour
            }
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
                if (highlightedLines.Count() == path.Count() - 1)//if the path is found, then the size of the array is always 1 less that the n. of vertices passed in
                {
                    string pathString = "";
                    for (int i = 0; i < highlightedLines.Count(); ++i)
                    {
                        highlightedLines[i].Stroke = new SolidColorBrush(Colors.Red);
                        pathString += path[i].ToString() + "=>"; //change the colour and update the path string
                    }
                    pathString += path[path.Count() - 1];
                    MessageBox.Show("Press ok to clear \nTotal Cost: " + total +"\nPath: " + pathString);
                    foreach (Line line in highlightedLines)
                    {
                        line.Stroke = new SolidColorBrush(Colors.Black);//reset the colour
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
        
    }
}

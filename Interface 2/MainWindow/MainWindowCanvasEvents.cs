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
                else if (currentButton == btnDepthFirst)
                {
                    Ellipse startVertex = (Ellipse)e.OriginalSource;
                    int startVertexId = Convert.ToInt32(startVertex.Name.Substring(3));//id of the start vertex
                    if (Graph.GetAdjVertices(startVertexId).Count() != 0) //make sure the node has atleast one edge
                    {
                        Tuple<List<Tuple<int, int>>, bool ,string> result = Graph.DepthFirst(startVertexId);
                        List<Tuple<int, int>> edges = result.Item1;
                        TraversalHighlightPath(edges);
                        txExtraInfo2.Text = "Traversal Order: " + result.Item3;

                    }


                }
                else if (currentButton == btnBreadthFirst)
                {
                    Ellipse startVertex = (Ellipse)e.OriginalSource;
                    int startVertexId = Convert.ToInt32(startVertex.Name.Substring(3)); //id of the start vertex
                    if (Graph.GetAdjVertices(startVertexId).Count() != 0) //make sure the node has atleast one edge
                    {
                        Tuple<List<Tuple<int, int>>, string> result = Graph.BreadthFirst(startVertexId);
                        List<Tuple<int, int>> edges = result.Item1;
                        TraversalHighlightPath(edges);
                        txExtraInfo2.Text = "Traversal Order: " + result.Item2;
                    }

                }
                else if (currentButton == btnRouteInspStartAndEnd)
                {
                    if (!Graph.IsConnected())
                    {
                        MessageBox.Show("The graph is not connected");
                    }
                    else
                    {
                        rInspSelectionCount += 1;
                        if (rInspSelectionCount % 2 == 0) //if even, its the END vertex
                        {
                            Ellipse endVertex = (Ellipse)e.OriginalSource;
                            int endVertexId = Convert.ToInt32(endVertex.Name.Substring(3));
                            if (Graph.GetValency(endVertexId) % 2 == 0) //if the valency of the end vertex is odd, the algorithm isnt useful here
                            {
                                MessageBox.Show("The start and end Vertex must have an ODD valency.");
                                rInspSelectionCount -= 1; //decrement it as if that selection didnt count
                            }
                            else if (rInspStart == endVertexId) //if they are path finding to itself, do nothing
                            {
                                EnableAllActionButtons();
                                EnableTbCtrl();
                                EnableAllAlgoButtons();
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
                                else if (Graph.IsSemiEulerian()) //if the graph is already semi eulerian then it will be traversable
                                {
                                    MessageBox.Show("No edges need to be added");
                                    EnableAllActionButtons();
                                    EnableTbCtrl();
                                    EnableAllAlgoButtons();
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
                                    RouteInspHighlightPath(edgesToRepeat, cost); //highlights the edges to be repeated and presents the cost
                                    HideValencies();
                                }
                                EnableTbCtrl();
                                EnableAllActionButtons();
                                EnableAllAlgoButtons();
                            }

                        }
                        else if (rInspSelectionCount % 2 == 1) //if selectioncount is odd, then its the START vertex
                        {
                            DisableTbCtrl();
                            DisableAllAlgoButtons();
                            DisableAllActionButtons();
                            Ellipse v = (Ellipse)e.OriginalSource;
                            rInspStart = Convert.ToInt32(v.Name.Substring(3));
                            if (Graph.GetValency(rInspStart) % 2 == 0) //make sure the vertex has an odd valency
                            {
                                EnableAllActionButtons();
                                EnableTbCtrl();
                                EnableAllAlgoButtons();
                                MessageBox.Show("The start and end vertex must have an odd valency");
                                labelExtraInfo.Content = "Choose a START vertex with ODD valency";
                                rInspSelectionCount -= 1; //if it doesnt, decrement to act as if that selection didnt count
                            }
                            else
                            {
                                labelExtraInfo.Content = "Choose an END vertex with ODD valency.";
                            }
                            
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
                            btnSaveGraph.IsEnabled = false;
                            btnLoadGraph.IsEnabled = false;
                            labelExtraInfo.Content = "";
                        }
                        else if (connectEdges.ShowDialog() == true) // otherwise, open a new form and get the weight
                        {
                            int weight = Convert.ToInt32(connectEdges.txWeight.Text); //get weight from text box
                            ConnectVertices(lastSelectedVertex, vertexToConnectTo, weight); //add the edge
                            labelExtraInfo.Content = "";
                            EnableTbCtrl();
                            EnableAllActionButtons();
                            btnSaveGraph.IsEnabled = false;
                            btnLoadGraph.IsEnabled = false;
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
                else if (currentButton == btnPrims)
                {
                    if (vertexList.Count() != 0)
                    {
                        Ellipse startVertex = (Ellipse)e.OriginalSource;
                        int startVertexID = Convert.ToInt32(startVertex.Name.Substring(3));
                        if (Graph.IsConnected())
                        {
                            List<Tuple<int, int, int>> mst = Graph.Prims(startVertexID);
                            mstHighlightPath(mst);
                        }
                        else
                        {
                            MessageBox.Show("The graph is not connected.");
                        }
                    }
                }
                else if (currentButton == btnDijkstrasShort)
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
                            btnSaveGraph.IsEnabled = false;
                            btnLoadGraph.IsEnabled = false;
                            labelExtraInfo.Content = "";
                        }
                        else
                        {
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
                            
                            EnableTbCtrl();
                            EnableAllAlgoButtons();
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
                    TextBlock vertexLabel = new TextBlock()
                    {
                        Text = vertexId,
                        FontSize = 15,
                        Foreground = new SolidColorBrush(Colors.Black),
                        Name = "labelFor" + vertexId,
                        IsHitTestVisible = false //makes it so that the mouse clicks THROUGH the text block, and onto the ellipse
                    };

                    //binding the fill of the textblock to the colour picker
                    Binding bindingBG = new Binding("SelectedBrush")
                    {
                        Source = colourPickerLabel,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.TwoWay
                    };
                    vertexLabel.SetBinding(TextBlock.ForegroundProperty, bindingBG);
                    //set its position ontop of the vertex, and at its center, depending on the number of digits
                    if (vertexLabel.Text.Length == 1)
                    {
                        Canvas.SetTop(vertexLabel, Canvas.GetTop(vertexToAdd) - 9);
                        Canvas.SetLeft(vertexLabel, Canvas.GetLeft(vertexToAdd) - 4);
                    }
                    else if (vertexLabel.Text.Length == 2)
                    {
                        Canvas.SetTop(vertexLabel, Canvas.GetTop(vertexToAdd) - 9);
                        Canvas.SetLeft(vertexLabel, Canvas.GetLeft(vertexToAdd) - 9);
                    }
                    else
                    {
                        Canvas.SetTop(vertexLabel, Canvas.GetTop(vertexToAdd) - 9);
                        Canvas.SetLeft(vertexLabel, Canvas.GetLeft(vertexToAdd) - 13);
                    }
                    Canvas.SetZIndex(vertexLabel, 4);
                    

                    vertexTxBoxList.Add(vertexLabel);//add it to the label list

                    mainCanvas.Children.Add(vertexToAdd);//add the vertex to the canvas
                    mainCanvas.Children.Add(vertexLabel); //add the label to the canvas
                }
            }
            if (graphCreated == true)
            {
                GenerateAdjList();
            }
        }
    }
}

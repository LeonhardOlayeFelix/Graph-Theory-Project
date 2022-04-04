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
                            btnImportGraph.IsEnabled = false;
                            btnImportGraph.IsEnabled = false;
                            labelExtraInfo.Content = "";
                        }
                        else if (connectEdges.ShowDialog() == true) // otherwise, open a new form and get the weight
                        {
                            int weight = Convert.ToInt32(connectEdges.txWeight.Text); //get weight from text box
                            ConnectVertices(lastSelectedVertex, vertexToConnectTo, weight); //add the edge
                            labelExtraInfo.Content = "";
                            EnableTbCtrl();
                            EnableAllActionButtons();
                            btnImportGraph.IsEnabled = false;
                            btnImportGraph.IsEnabled = false;
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
                            btnImportGraph.IsEnabled = false;
                            btnImportGraph.IsEnabled = false;
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
                else if (currentButton == btnDijkstrasLong)
                {
                    dijkstraSelectionCount += 1;
                    if (dijkstraSelectionCount % 2 == 0)
                    {
                        Ellipse v = (Ellipse)e.OriginalSource;
                        int vId = Convert.ToInt32(v.Name.Substring(3)); //get id of vertex that was pressed
                        if (startVertex == vId) //if they are connecting it to itself, do nothing
                        {
                            EnableAllActionButtons();
                            EnableTbCtrl();
                            btnImportGraph.IsEnabled = false;
                            btnImportGraph.IsEnabled = false;
                            labelExtraInfo.Content = "";
                        }
                        else
                        {
                            try
                            {
                                List<int> path = Graph.DijkstrasAlgorithmLong(startVertex, vId).Item1; //get the path from the method
                                DijkstraHighlightPath(path);
                                labelExtraInfo.Content = "";
                            }
                            catch (NullReferenceException) //this means there was no path
                            {
                                MessageBox.Show("There is no longest path from these two points");
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
                    txLogsActions.AppendText(Graph.Name + ".AddVertex()\n");//update logs
                }
            }
            if (graphCreated == true)
            {
                GenerateAdjList();
            }
        }
    }
}

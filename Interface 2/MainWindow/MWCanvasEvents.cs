using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Media;
using System.Threading;
namespace Interface_2
{

    public partial class MainWindow : Window
    {
        bool mouseDown = false;
        Point mouseDownPos;
        List<Ellipse> selectedVertices = new List<Ellipse>();
        List<string> selectedLinesNames = new List<string>();
        /// <summary>
        /// The section of code that determines what happens when the canvas, or anything on the canvas is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (e.OriginalSource is Ellipse) //if they press the ellipse / vertex 
            {
                timer1.Stop();
                timer.Stop();
                if (currentButton == btnDeleteVertex) //if they are trying to delete a vertex
                {
                    Ellipse activeVertex = (Ellipse)e.OriginalSource; //give the button a variable to refer to
                    DeleteVertex(activeVertex);
                }
                else if (currentButton == btnAddConnection) //if they want to connect two vertices
                {
                    buttonSelectionCount += 1;
                    if (buttonSelectionCount % 2 == 0 && buttonSelectionCount != 0) //if even its the second vertex they want to connect to
                    {
                        vertexToConnectTo = (Ellipse)e.OriginalSource; //this is the vertex they are connecting to
                        AddConnectionEven(vertexToConnectTo);
                    }
                    else if (buttonSelectionCount % 2 == 1) //if odd, its the first vertex they pressed to connect to, so set it to lastSelectedVertex
                    {
                        Ellipse VertexToConnectFrom = (Ellipse)e.OriginalSource;
                        AddConnectionOdd(VertexToConnectFrom);
                    }
                }
                else if (currentButton == btnDefault) //check if they are using the default button
                {
                    RevertEllipseColour();
                    RevertLineColour();
                    labelExtraInfo.Content = "Position: " + Graph.GetVertex(Convert.ToInt32(((Ellipse)e.OriginalSource).Name.Substring(3))).Position.GetPositionTuple();
                    ((Ellipse)e.OriginalSource).Fill = HighlightColour;
                }
                else if (currentButton == btnDepthFirst) //check if they are trying to do a depth first traversal
                {
                    Ellipse startVertex = (Ellipse)e.OriginalSource;
                    DepthFirst(startVertex);
                }
                else if (currentButton == btnBreadthFirst)//check if they are trying to do a breadth first traversal
                {
                    Ellipse startVertex = (Ellipse)e.OriginalSource;
                    BreadthFirst(startVertex);
                }
                else if (currentButton == btnHighlightPaths)//check if they are trying to do highlight a path
                {
                    Ellipse activeVertex = (Ellipse)e.OriginalSource;
                    HighlightPaths(activeVertex);
                }
                else if (currentButton == btnRouteInspStartAndEnd) //if they are trying to do a route inspection
                {
                    ClearHighlightedLines();
                    if (!Graph.IsConnected()) //can only be done on a connected graph
                    {
                        MessageBox.Show("The graph is not connected");
                    }
                    else
                    {
                        rInspSelectionCount += 1;
                        if (rInspSelectionCount % 2 == 0) //if even, its the END vertex
                        {
                            Ellipse endVertex = (Ellipse)e.OriginalSource;
                            RouteInspStartAndEndEven(endVertex);
                        }
                        else if (rInspSelectionCount % 2 == 1) //if selectioncount is odd, then its the START vertex
                        {
                            Ellipse startVertex = (Ellipse)e.OriginalSource;
                            RouteInspStartAndEndOdd(startVertex);
                        }
                    }
                }
                else if (currentButton == btnRevertOnePositions)
                {
                    Ellipse currentEllipse = (Ellipse)e.OriginalSource;
                    RevertOneVertexPosition(currentEllipse);
                }
                else if (currentButton == btnPrims) //if they are trying to use prims algorithm
                {
                    if (vertexList.Count() != 0)
                    {
                        Ellipse startVertex = (Ellipse)e.OriginalSource; //the vertex to start the MST from
                        Prims(startVertex);
                    }
                }
                else if (currentButton == btnDijkstrasShort) //if they are trying to run dijkstras algorithm
                {
                    ClearHighlightedLines();
                    dijkstraSelectionCount += 1;
                    if (dijkstraSelectionCount % 2 == 0)
                    {
                        Ellipse endVertex = (Ellipse)e.OriginalSource;
                        DijkstraEven(endVertex);

                    }
                    else if (dijkstraSelectionCount % 2 == 1) //this is the start vertex
                    {
                        Ellipse startVertex = (Ellipse)e.OriginalSource;
                        DijkstraOdd(startVertex);
                    }
                }
            }
            else if (e.OriginalSource is Line) //if they click on a line in the canvas
            {
                if (currentButton == btnDeleteConnection)
                {
                    DeleteEdge(FindEdge(((Line)e.OriginalSource).Name));
                }
                if (currentButton == btnDefault) //if they are using the default button
                {
                    RevertEllipseColour(); //reset the colours
                    RevertLineColour();
                    ((Line)e.OriginalSource).Stroke = HighlightColour; //highlight the line
                }
            }
            else if (buttonSelectionCount % 2 == 1 && currentButton == btnAddConnection) //if they pressed the canvas to try and cancel an add connection 
            {
                DecrementSelectionCount(ref buttonSelectionCount);
            }
            else if (rInspSelectionCount % 2 == 1 && currentButton == btnRouteInspStartAndEnd)//if they pressed the canvas to try and cancel a route inspection
            {
                DecrementSelectionCount(ref rInspSelectionCount);
            }
            else if (dijkstraSelectionCount % 2 == 1 && currentButton == btnDijkstrasShort)//if they pressed the canvas to try and cancel a dijkstras algorithm
            {
                DecrementSelectionCount(ref dijkstraSelectionCount);
            }
            else if (currentButton == btnAddVertex) //where a user wants to add a vertex
            {
                int maxNumber = alphabet.Count(); //the highest number of uniquely representable nodes using the alphabet
                if (Graph.GetNumberOfVertices() + Graph.numberOfDeletedVertices > maxNumber - 1 && cbAlphabet.IsChecked == true)
                {
                    MessageBox.Show("Turn off Alphabet Labelling so more Nodes can be represented");
                }
                else
                {
                    Ellipse vertexToAdd = new Ellipse() { StrokeThickness = 2 }; //create the vertex that will be added


                    Binding bindingStroke = new Binding("SelectedBrush") //binding the stroke of the vertices to the color picker
                    {
                        Source = colourPickerVertexStroke,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.OneWay
                    };
                    vertexToAdd.SetBinding(Ellipse.StrokeProperty, bindingStroke);

                    Binding bindingFill = new Binding("SelectedBrush")//binding the fill colour of the vertices to the color picker
                    {
                        Source = colourPickerVertex,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.OneWay
                    };
                    vertexToAdd.SetBinding(Ellipse.FillProperty, bindingFill);

                    vertexToAdd.Height = 0;
                    vertexToAdd.Width = 0;

                    //positioning the vertex in the canvas.
                    double vertexCenterXMousePos = Mouse.GetPosition(mainCanvas).X;
                    double vertexCenterYMousePos = Mouse.GetPosition(mainCanvas).Y;
                    vertexToAdd.Margin = new Thickness(-100000); //margin of 100000 so that it resizes around the center.
                    Canvas.SetLeft(vertexToAdd, vertexCenterXMousePos);
                    Canvas.SetTop(vertexToAdd, vertexCenterYMousePos);
                    Canvas.SetZIndex(vertexToAdd, Zindex++);
                    //give the string a Name in the form btn(vertexId)
                    string vertexId = buttonId.ToString();
                    vertexToAdd.Name = "btn" + vertexId;
                    Graph.AddVertex(vertexCenterXMousePos, vertexCenterYMousePos); //update the class
                    labelExtraInfo.Content = "Placed at coordinates: " + Graph.GetVertex(Convert.ToInt32(vertexToAdd.Name.Substring(3))).Position.GetPositionTuple();
                    buttonId += 1; //increment button Id for unique buttons
                    vertexList.Add(vertexToAdd);//add the vertex to the list
                    vertexToAdd.MouseMove += mouseMove;//give the buttons drag and drop event handlers

                    TextBlock vertexLabel = new TextBlock()//label for the ID of the vertex
                    {
                        FontSize = 15,
                        Foreground = new SolidColorBrush(Colors.Black),
                        Name = "labelFor" + vertexId,
                        IsHitTestVisible = false //makes it so that the mouse clicks THROUGH the text block, and onto the ellipse
                    };
                    if ((bool)cbAlphabet.IsChecked)
                    {
                        vertexLabel.Text = alphabet.ElementAt(Convert.ToInt32(vertexId));
                    }
                    else
                    {
                        vertexLabel.Text = vertexId;
                    }
                    Binding bindingBG = new Binding("SelectedBrush")//binding the fill of the textblock to the colour picker
                    {
                        Source = colourPickerLabel,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.OneWay
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
                    Canvas.SetZIndex(vertexLabel, Zindex++);

                    vertexTxBoxList.Add(vertexLabel);//add it to the label list
                    vertexList.Add(vertexToAdd);
                    mainCanvas.Children.Add(vertexToAdd);//add the vertex to the canvas
                    mainCanvas.Children.Add(vertexLabel); //add the label to the canvas
                    InitiateVertexStoryboard(vertexDiameterSlider.Value, TimeSpan.FromSeconds(0.2), vertexToAdd); //begin story board
                    
                }
                if (graphCreated == true)
                {
                    GenerateAdjList();
                }
            }
            else if (currentButton == btnDefault)
            {
                selectionBox.Visibility = Visibility.Visible;
                mouseDown = true;
                mouseDownPos = e.GetPosition(mainCanvas);
                mainCanvas.CaptureMouse();
                selectedVertices.Clear();
                selectedLinesNames.Clear();
                RevertEllipseColour();
                RevertLineColour();
                // Initial placement of the drag selection box.         
                Canvas.SetLeft(selectionBox, mouseDownPos.X);
                Canvas.SetTop(selectionBox, mouseDownPos.Y);
                mainCanvas.Children.Add(selectionBox);
                selectionBox.Width = 0;
                selectionBox.Height = 0;
            }
        }
        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown && currentButton == btnDefault)
            {
                // When the mouse is held down, reposition the drag selection box.

                Point mousePos = e.GetPosition(mainCanvas);
                if (mouseDownPos.X < mousePos.X)
                {
                    Canvas.SetLeft(selectionBox, mouseDownPos.X);
                    selectionBox.Width = mousePos.X - mouseDownPos.X;
                }
                else
                {
                    Canvas.SetLeft(selectionBox, mousePos.X);
                    selectionBox.Width = mouseDownPos.X - mousePos.X;
                }
                if (mouseDownPos.Y < mousePos.Y)
                {
                    Canvas.SetTop(selectionBox, mouseDownPos.Y);
                    selectionBox.Height = mousePos.Y - mouseDownPos.Y;
                }
                else
                {
                    Canvas.SetTop(selectionBox, mousePos.Y);
                    selectionBox.Height = mouseDownPos.Y - mousePos.Y;
                }
            }
        }
        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (currentButton == btnDefault)
            {
                selectionBox.Visibility = Visibility.Collapsed;
                mouseDown = false;
                mainCanvas.ReleaseMouseCapture();
                mainCanvas.Children.Remove(selectionBox);
                Point mouseUpPos = e.GetPosition(mainCanvas);
                //two coordinates of rectangle
                double X1 = mouseUpPos.X;
                double X2 = mouseDownPos.X;
                double Y1 = mouseUpPos.Y;
                double Y2 = mouseDownPos.Y; 
                foreach (var ctrl in mainCanvas.Children)
                {
                    try
                    {
                        Ellipse selectedVertex = (Ellipse)ctrl;
                        double selVertexXCoord = Canvas.GetLeft(selectedVertex);
                        double selVertexYCoord = Canvas.GetTop(selectedVertex);
                        //check if vertex control is within the selection box
                        if ((selVertexXCoord >= X1 && selVertexXCoord <= X2 && selVertexYCoord <= Y2 && selVertexYCoord >= Y1) ||
                            (selVertexXCoord >= X2 && selVertexXCoord <= X1 && selVertexYCoord <= Y1 && selVertexYCoord >= Y2) ||
                            (selVertexXCoord >= X2 && selVertexXCoord <= X1 && selVertexYCoord <= Y2 && selVertexYCoord >= Y1) ||
                            (selVertexXCoord >= X1 && selVertexXCoord <= X2 && selVertexYCoord <= Y1 && selVertexYCoord >= Y2))
                        {
                            
                            if (Graph.IsInVertexList(Convert.ToInt32(selectedVertex.Name.Substring(3))))
                            {
                                selectedVertices.Add(selectedVertex);
                            }
                            selectedVertex.Fill = HighlightColour;
                        }
                    }
                    catch { };
                    try
                    {
                        Line selectedLine = (Line)ctrl;
                        double selLineXCoord = (selectedLine.X1 + selectedLine.X2) / 2;
                        double selLineYCoord = (selectedLine.Y1 + selectedLine.Y2) / 2;
                        //check if midpoint of Line control is within the selection box
                        if ((selLineXCoord >= X1 && selLineXCoord <= X2 && selLineYCoord <= Y2 && selLineYCoord >= Y1) ||
                            (selLineXCoord >= X2 && selLineXCoord <= X1 && selLineYCoord <= Y1 && selLineYCoord >= Y2) ||
                            (selLineXCoord >= X2 && selLineXCoord <= X1 && selLineYCoord <= Y2 && selLineYCoord >= Y1) ||
                            (selLineXCoord >= X1 && selLineXCoord <= X2 && selLineYCoord <= Y1 && selLineYCoord >= Y2))
                        {
                            Tuple<Line, Ellipse, Ellipse, TextBlock> currentLine = FindEdge(selectedLine.Name);
                            if (edgeList.Contains(currentLine))
                            {
                                selectedLinesNames.Add(selectedLine.Name); ;
                            }
                            selectedLine.Stroke = HighlightColour;
                        }
                    }
                    catch { };
                }
                labelExtraInfo.Content = "Now click 'Delete Vertex' or 'Delete Edge'";
            }
        }
    }
}

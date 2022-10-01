using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Timers;

namespace Interface_2
{
    public partial class MainWindow : Window
    {
        //All of the event handlers
        
        private void btnAddVertex_Click(object sender, RoutedEventArgs e)
        {

            HideValencies();
            labelExtraInfo.Content = "Click on the canvas to place a vertex.";
            ActivateButton(sender);
            ClearAllOperations();
            leftClickCanvasOperation = () => 
            {
                int maxNumber = alphabet.Count(); //the highest number of uniquely representable nodes using the alphabet
                if (graph.GetNumberOfVertices() + graph.numberOfDeletedVertices > maxNumber - 1 && cbAlphabet.IsChecked == true)
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
                    graph.AddVertex(vertexCenterXMousePos, vertexCenterYMousePos); //update the class
                    labelExtraInfo.Content = "Placed at coordinates: " + graph.GetVertex(Convert.ToInt32(vertexToAdd.Name.Substring(3))).Position.GetPositionTuple();
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
                    GenerateAdjMat();
                }
            }; //change operation to addvertex operation
        }
        private void btnAddConnection_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click two vertices to connect them.";
            ActivateButton(sender);
            ClearAllOperations();
            leftClickVertexOperation = (activeVertex) => {
                buttonSelectionCount += 1;
                if (buttonSelectionCount % 2 == 0) //if even its the second vertex they want to connect to
                {
                    AddConnectionEven(activeVertex);
                }
                else if (buttonSelectionCount % 2 == 1) //if odd, its the first vertex they pressed to connect to, so set it to lastSelectedVertex
                {
                    AddConnectionOdd(activeVertex);
                }
            };
        }
        private void cbAutoGenEdges_Checked(object sender, RoutedEventArgs e)
        {
            cbAutoGenEdgesValue.IsChecked = false; //only one check box can be selected at a time
        }
        private void cbAutoGenEdgesValue_Checked(object sender, RoutedEventArgs e)
        {
            cbAutoGenEdges.IsChecked = false;//ditto
        }
        private void cbAlphabet_Checked(object sender, RoutedEventArgs e)
        {
            int maxNumber = alphabet.Count(); //the highest number of uniquely representable vertices using the alphabet
            if (graph.GetMaxVertexID() >= maxNumber)
            {
                MessageBox.Show("Not enough Letters in the alphabet to represent each vertex"); //this wont occur but is just a precaution
                cbAlphabet.IsChecked = false;
            }
            else
            {
                for (int i = 0; i < graph.GetMaxVertexID() + 1; ++i) //loop through all the nodes
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
            for (int i = 0; i < graph.GetMaxVertexID() + 1; ++i) //loop through all the nodes
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
        private void btnBreadthFirst_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
            HideValencies();
            labelExtraInfo.Content = "Choose a root vertex";
            ClearAllOperations();
            leftClickVertexOperation = (activeVertex) => { BreadthFirst(activeVertex); };
        }
        private void btnCreateNewGraph_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(btnCreateNewGraph);
            string name = "";
            NameCreatedGraph nameGraphWindow = new NameCreatedGraph(); //create an instance of the new window
            nameGraphWindow.ShowDialog(); //opens a new window
            if (nameGraphWindow.DialogResult == true) //if they pressed ok rather than the exit button
            {
                name = nameGraphWindow.txBoxGraphName.Text; //re-initialise everything:
                DeleteGraph();
                CreateNewGraph(name);
                graphCreated = true;
                txAdjset.Clear();
            }
        }
        private void colourPickerHighlight_SelectedBrushChanged(object sender, Syncfusion.Windows.Tools.Controls.SelectedBrushChangedEventArgs e)
        {
            HighlightColour = (SolidColorBrush)colourPickerHighlight.SelectedBrush;
        }
        private void btnDeleteAllEdges_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
            List<Tuple<Line, Ellipse, Ellipse, TextBlock>> edgesToDelete = new List<Tuple<Line, Ellipse, Ellipse, TextBlock>>();
            foreach (var edge in edgeList)
            {
                edgesToDelete.Add(edge);
            }
            foreach (var edge in edgesToDelete)
            {
                DeleteEdge(edge);
            }
            ClearAllOperations();
        }
        private void btnDijkstrasShort_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Find the lowest cost route between two vertices";
            ActivateButton(sender);
            ClearAllOperations();
            leftClickVertexOperation = (activeVertex) => {
                ClearHighlightedLines();
                dijkstraSelectionCount += 1;
                if (dijkstraSelectionCount % 2 == 0)
                {
                    DijkstraEven(activeVertex);
                }
                else if (dijkstraSelectionCount % 2 == 1) //this is the start vertex
                {
                    DijkstraOdd(activeVertex);
                }
            };
        }
        private void btnDeleteGraph_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            txAdjset.Clear();
            DeleteGraph();
            DisableAllActionButtons();
            DisableTabControl();
            ClearAllOperations();
        }
        private void btnDepthFirst_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
            HideValencies();
            labelExtraInfo.Content = "Click the root vertex";
            ClearAllOperations();
            leftClickVertexOperation = (activeVertex) => { DepthFirst(activeVertex); };
        }
        private void btnDeleteVertex_Click(object sender, RoutedEventArgs e)
        {
            //delete any vertices that have been selected
            if (currentButton == btnDefault && selectedVertices.Count != 0)
            {
                foreach (Ellipse vertex in selectedVertices)
                {
                    DeleteVertex(vertex);
                }
                selectedVertices.Clear();
            }
            else
            {
                if (selectedVertices.Count == 0)
                {
                    ActivateButton(sender);
                    labelExtraInfo.Content = "Click a vertex to delete";
                }
                HideValencies();
                ClearAllOperations();
                leftClickVertexOperation = (activeVertex) => { DeleteVertex(activeVertex); };
            }
        }
        private void btnDeleteConnection_Click(object sender, RoutedEventArgs e)
        {
            if (currentButton == btnDefault && selectedLinesNames.Count != 0)
            {
                foreach (string name in selectedLinesNames)
                {
                    Tuple<Line, Ellipse, Ellipse, TextBlock> line = FindEdge(name);
                    DeleteEdge(line);
                }
            }
            else
            {
                if (selectedLinesNames.Count == 0)
                {
                    ActivateButton(sender);
                    labelExtraInfo.Content = "Click an edge to delete";
                }
                selectedLinesNames.Clear();
                HideValencies();
                ClearAllOperations();
                leftClickVertexOperation = (activeVertex) => { DeleteVertex(activeVertex); };
                leftClickLineOperation = (activeLine) => { DeleteEdge(FindEdge(activeLine.Name)); };
            }
            
        }
        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            labelExtraInfo.Content = "Click or Drag to select objects";
            ActivateButton(sender);
            ClearAllOperations();
            leftClickCanvasOperation = () => {
                selectionBox.Visibility = Visibility.Visible;
                mouseDown = true;
                mouseDownPos = Mouse.GetPosition(mainCanvas);
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
            };
            leftClickVertexOperation = (activeVertex) => {
                RevertEllipseColour();
                RevertLineColour();
                labelExtraInfo.Content = "Position: " + graph.GetVertex(Convert.ToInt32(activeVertex.Name.Substring(3))).Position.GetPositionTuple();
                activeVertex.Fill = HighlightColour;
            };
            leftClickLineOperation = (activeLine) => {
                RevertEllipseColour(); //reset the colours
                RevertLineColour();
                activeLine.Stroke = HighlightColour; 
            };
            leftMouseButtonUpOperation = () => {
                selectionBox.Visibility = Visibility.Collapsed;
                mouseDown = false;
                mainCanvas.ReleaseMouseCapture();
                mainCanvas.Children.Remove(selectionBox);
                Point mouseUpPos = Mouse.GetPosition(mainCanvas);
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

                            if (graph.IsInVertexList(Convert.ToInt32(selectedVertex.Name.Substring(3))))
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
            };
            }
        private void btnDragAndDrop_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click and Hold vertices to drag them around the canvas";
            ActivateButton(sender);
            ClearAllOperations();
        }
        private void btnFloyds_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(btnFloyds);
            ClearAllOperations();
            txExtraInfo2.Text = "Matrix of Shortest Paths:\n" + graph.FloydWarshallStr();
            
        }
        private void btnHighlightPaths_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
            labelExtraInfo.Content = "Click on a series of vertices to go on a walk";
            ClearAllOperations();
            leftClickVertexOperation = (activeVertex) => { HighlightPaths(activeVertex); };
        }
        private void btnKruskals_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
            HideValencies();
            if (!graph.IsConnected())
            {
                MessageBox.Show("The graph is not connected");
            }
            else
            {
                Tuple<List<Tuple<int, int, int>>, int> mst = graph.Kruskals();
                mstHighlightTree(mst.Item1, mst.Item2); //highlight the MST
            }
            ClearAllOperations();
        }
        private void btnLoadGraph_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(btnLoadGraph);
            LoadGraph();
            ClearAllOperations();
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(btnLogin);
            LoginStudent loginstudent = new LoginStudent();
            ClearAllOperations();
            if (loginstudent.ShowDialog() == true)
            {
                if (loginstudent.studentJustLogged != null) //this identifies whther they are logged in as a student
                {
                    loggedStudent = loginstudent.studentJustLogged; //initiliase the student instance
                    txLoggedID.Content = "ID: " + loggedStudent.ID;
                    txLoggedInAs.Content = "Logged in as: " + loggedStudent.firstname + " " + loggedStudent.lastname;
                    StudentLogInProcess();
                }
                else
                {
                    loggedTeacher = loginstudent.teacherJustLogged; //initialise the teacher instance
                    txLoggedID.Content = "ID: " + loggedTeacher.ID;
                    txLoggedInAs.Content = "Logged in as: " + loggedTeacher.title + " " + loggedTeacher.firstname[0] + " " + loggedTeacher.lastname;
                    TeacherLogInProcess();
                }
            }
        }
        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(btnLogOut);
            ClearAllOperations();
            LogOutProcess();
        }
        public void mouseMove(object sender, MouseEventArgs e)
        {
            //tracks the mouse as it hovers over a vertex
            if (e.LeftButton == MouseButtonState.Pressed && currentButton == btnDragAndDrop)//if, whilst hovering, they press the vertex
            {
                ellipseToDrop = sender as Ellipse;
                ellipseToDrop.Fill = HighlightColour;
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
            graph.GetVertex(ellipseToDropID).Position.X = dropPosition.X; //udpate its position in the class too
            Canvas.SetTop(ellipseToDrop, dropPosition.Y);//updates the y coordinate ever time its dragged
            graph.GetVertex(ellipseToDropID).Position.Y = dropPosition.Y; //update its position in the class too
            labelExtraInfo.Content = "Drag position: " + graph.GetVertex(ellipseToDropID).Position.GetPositionTuple();
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
        private void btnPrims_Click(object sender, RoutedEventArgs e)
        {
            labelExtraInfo.Content = "Choose a start Vertex";
            ActivateButton(sender);
            ClearAllOperations();
            leftClickVertexOperation = (activeVertex) => {
                if (vertexList.Count() != 0)
                {
                    Prims(activeVertex);
                }
            };
        }
        private void btnResetComponentShape_Click(object sender, RoutedEventArgs e)
        {
            //resets the sliders back to their original form
            ActivateButton(sender);
            ClearAllOperations();
            vertexDiameterSlider.Value = 40;
            pathWalkerDurationSlider.Value = pathWalkerDurationSlider.Minimum;
        }
        private void BtnRandomGraph_Click(object sender, RoutedEventArgs e)
        {
            ClearAllOperations();
            btnRandomGraph.IsEnabled = false;
            Timer enableTimer = new Timer() { Interval = 500};
            enableTimer.Elapsed += (sender1, e1) =>
            {
                enableTimer.Stop();
                this.Dispatcher.Invoke(() =>
                {
                    btnRandomGraph.IsEnabled = true;
                });
            };
            enableTimer.Start();
            int numVertices = Convert.ToInt32(txNumVertices.Text);
            if (numVertices > 18 || numVertices < 1)
            {
                MessageBox.Show("Number of vertices not within range");
                return;
            }
            Random rand = new Random();
            Graph randomGraph = new Graph();
            for (int i = 0; i < numVertices; ++i)
            {
                int canvasHeight = Convert.ToInt32(mainCanvas.ActualHeight);
                int canvasWidth = Convert.ToInt32(mainCanvas.ActualWidth);
                randomGraph.AddVertex(0, 0);
            }
            while (!randomGraph.IsConnected())//keeps generating random edges until the graph is connected
            {
                int weight = 0;
                if (cbAutoGenEdges.IsChecked == true)
                {
                    weight = rand.Next(Convert.ToInt32(txRandomGenLB.Text), Convert.ToInt32(txRandomGenUB.Text));
                }
                else if (cbAutoGenEdgesValue.IsChecked == true)
                {
                    weight = Convert.ToInt32(txAutoWeight.Text);
                }
                int v1 = rand.Next(numVertices);
                int v2 = rand.Next(numVertices);
                while (v1 == v2)
                {
                    v2 = rand.Next(numVertices);
                }
                randomGraph.AddEdge(v1, v2, weight);
                if (randomGraph.GetNumberOfEdges() >= numVertices * (numVertices - 1) / 3) //if theres far too many edges
                {
                    randomGraph = new Graph();
                    for (int i = 0; i < numVertices; ++i)
                    {
                        int canvasHeight = Convert.ToInt32(mainCanvas.ActualHeight);
                        int canvasWidth = Convert.ToInt32(mainCanvas.ActualWidth);
                        randomGraph.AddVertex(0, 0);
                    }
                }
            }
            ArrangeGraph(4, 100, randomGraph); //arranges graph into appropriate shape
            RenderGraph(randomGraph);
        }
        private void btnRouteInspStartAndEnd_Click(object sender, RoutedEventArgs e)
        {
            ShowValencies();
            labelExtraInfo.Content = "Choose a START vertex with an ODD valency";
            ActivateButton(sender);
            ClearAllOperations();
            leftClickVertexOperation = (activeVertex) => {
                ClearHighlightedLines();
                if (!graph.IsConnected()) //can only be done on a connected graph
                {
                    MessageBox.Show("The graph is not connected");
                }
                else
                {
                    rInspSelectionCount += 1;
                    if (rInspSelectionCount % 2 == 0) //if even, its the END vertex
                    {
                        RouteInspStartAndEndEven(activeVertex);
                    }
                    else if (rInspSelectionCount % 2 == 1) //if selectioncount is odd, then its the START vertex
                    {
                        RouteInspStartAndEndOdd(activeVertex);
                    }
                }
            };
        }
        private void btnRouteInspStartAtEnd_Click(object sender, RoutedEventArgs e)
        {
            ClearAllOperations();
            if (vertexList.Count() != 0)
            {
                ActivateButton(sender);
                RouteInspectionStartAtEnd();
            }
            HideValencies();
        }
        private void btnResetColour_Click(object sender, RoutedEventArgs e)
        {
            ClearAllOperations();
            //resets the colours of all the colour pickers
            colourPickerBackground.SelectedBrush = new SolidColorBrush(Color.FromRgb(64, 61, 61));
            colourPickerVertex.SelectedBrush = new SolidColorBrush(Colors.DodgerBlue);
            colourPickerWeight.SelectedBrush = new SolidColorBrush(Colors.Black);
            colourPickerLabel.SelectedBrush = new SolidColorBrush(Colors.White);
            colourPickerLine.SelectedBrush = new SolidColorBrush(Colors.Black);
            colourPickerVertexStroke.SelectedBrush = new SolidColorBrush(Colors.Black);
            colourPickerHighlight.SelectedBrush = new SolidColorBrush(Colors.Red);
            ActivateButton(sender);
        }
        private void btnRevertPositions_Click(object sender, RoutedEventArgs e)
        {
            ClearAllOperations();
            labelExtraInfo.Content = "";
            ActivateButton(btnRevertPositions);
            foreach (var ctrl in mainCanvas.Children)
            {
                try
                {
                    Ellipse currentEllipse = (Ellipse)ctrl;
                    RevertOneVertexPosition(currentEllipse);
                }
                catch
                {

                }
            }
        }
        private void btnRevertOnePositions_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
            ClearAllOperations();
            leftClickVertexOperation = (activeVertex) => { RevertOneVertexPosition(activeVertex); };
        }
        private void btnRegisterStudent_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(btnRegisterStudent);
            ClearAllOperations();
            RegisterStudent registerstudent = new RegisterStudent();
            registerstudent.ShowDialog();
        }
        private void btnRegisterTeacher_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(btnRegisterTeacher);
            ClearAllOperations();
            RegisterTeacher registerteacher = new RegisterTeacher();
            registerteacher.ShowDialog();
        }
        private void btnSaveGraph_Click(object sender, RoutedEventArgs e)   
        {
            ActivateButton(btnSaveGraph);
            ClearAllOperations();
            SaveGraph();
        }
        private void btnTakeScreenshot_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
            HideValencies();
            ClearAllOperations();
            labelExtraInfo.Content = "Screenshot Taken";
            
        }
        private void btnToggleValencies_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
            ClearAllOperations();
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
    }
}

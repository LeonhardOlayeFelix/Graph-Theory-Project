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
            labelExtraInfo.Content = "Click anywhere on the canvas to place a vertex";
            ActivateButton(sender);
        }
        private void btnAddConnection_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click two vertices you want to connect and provide the weight";
            ActivateButton(sender);
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
            int maxNumber = alphabet.Count(); //the highest number of uniquely representable nodes using the alphabet
            if (Graph.GetMaxVertexID() >= maxNumber)
            {
                MessageBox.Show("Not enough Letters in the alphabet to represent each vertex");
                cbAlphabet.IsChecked = false;
            }
            else
            {
                for (int i = 0; i < Graph.GetMaxVertexID() + 1; ++i) //loop through all the nodes
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
            for (int i = 0; i < Graph.GetMaxVertexID() + 1; ++i) //loop through all the nodes
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
        private void ColourPickerHighlight_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
        }
        private void btnDijkstrasShort_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click a vertex to find the lowest cost route to the next clicked vertex";
            ActivateButton(sender);
        }
        private void btnDeleteGraph_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            txAdjset.Clear();
            DeleteGraph();
            DisableAllActionButtons();
            DisableTabControl();
        }
        private void btnDepthFirst_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
            HideValencies();
            labelExtraInfo.Content = "Choose a root vertex";
        }
        private void btnDeleteVertex_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click a vertex to delete it from the canvas";
            ActivateButton(sender);
        }
        private void btnDeleteConnection_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click an edge to Delete it from the canvas";
            ActivateButton(sender);
        }
        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            labelExtraInfo.Content = "Click or Drag to select objects";
            ActivateButton(sender);
        }
        private void btnDragAndDrop_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click and Hold vertices to drag them around the canvas";
            ActivateButton(sender);
        }
        private void btnHighlightPaths_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
            labelExtraInfo.Content = "Click on the vertices that you want the path to connect.";
        }
        private void btnGenerateMatrix_Click(object sender, RoutedEventArgs e)
        {
            labelExtraInfo.Content = "Updated Adjacency Matrix.";
            GenerateAdjMat(); //function generates the matrix
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
                Tuple<List<Tuple<int, int, int>>, int> mst = Graph.Kruskals();
                mstHighlightPath(mst.Item1, mst.Item2); //highlight the MST
            }
        }
        private void btnLoadGraph_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(btnLoadGraph);
            LoadGraph();
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(btnLogin);
            LoginStudent loginstudent = new LoginStudent();
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
            LogOutProcess();
        }
        private void SelectionCavas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (currentButton == btnDefault)
            {
                // Release the mouse capture and stop tracking it.
                
            }
        }
        private void mouseMove(object sender, MouseEventArgs e)
        {
            //monitors the mouse as it hovers over a vertex
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
        }
        private void btnResetComponentShape_Click(object sender, RoutedEventArgs e)
        {
            //resets the sliders back to their original form
            ActivateButton(sender);
            vertexDiameterSlider.Value = 40;
            pathWalkerDurationSlider.Value = pathWalkerDurationSlider.Minimum;
        }
        private void BtnRandomGraph_Click(object sender, RoutedEventArgs e)
        {
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
            //if (numVertices > 18 || numVertices < 1)
            //{
            //    MessageBox.Show("Number of vertices not within range");
            //    return;
            //}
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

            }
            ArrangeGraph(4, 100, randomGraph); //arranges graph into appropriate shape
            RenderGraph(randomGraph);
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
                RouteInspectionStartAtEnd();
            }
            HideValencies();
        }
        private void btnResetColour_Click(object sender, RoutedEventArgs e)
        {
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
        }
        private void btnRegisterStudent_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(btnRegisterStudent);
            RegisterStudent registerstudent = new RegisterStudent();
            registerstudent.ShowDialog();
        }
        private void btnRegisterTeacher_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(btnRegisterTeacher);
            RegisterTeacher registerteacher = new RegisterTeacher();
            registerteacher.ShowDialog();
        }
        private void btnSaveGraph_Click(object sender, RoutedEventArgs e)   
        {
            ActivateButton(btnSaveGraph);
            SaveGraph();
        }
        private void btnTakeScreenshot_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Screenshot Taken";
            ActivateButton(sender);
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
    }
}

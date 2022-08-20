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
using System.Windows.Media.Animation;
using System.IO;

namespace Interface_2
{
    
    public partial class MainWindow : Window
    {
        private void ConnectVertices(Ellipse v1, Ellipse v2, int weight, bool rendering = false) //connects two vertices together 
        {
            
            //gets the smaller and larger vertex
            Ellipse smallerEllipse = GetMinEllipse(v1, v2);
            Ellipse largerEllipse = GetMaxEllipse(v1, v2);
            //creates the lines soon-to-be name as "line5to6 for example 
            string lineName = "line" + smallerEllipse.Name.Substring(3).ToString() + "to" + largerEllipse.Name.Substring(3).ToString(); 
            foreach (Tuple<Line, Ellipse, Ellipse, TextBlock> edge in edgeList)//before connecting, check if a line already exists
            {
                if (edge.Item1.Name == lineName)//if it does....
                {
                    if (rendering) { return; }
                    DeleteEdge(edge, rendering);//delete the edge
                    break;
                }
            }
            if (!rendering)
            {
                Graph.AddEdge(Convert.ToInt32(v1.Name.Substring(3)), Convert.ToInt32(v2.Name.Substring(3)), weight); //update the object
            }

            //below creates the line which will be connected
            Line line = new Line()//set properties
            {
                StrokeThickness = 4,
                Name = lineName,
                Stroke = new SolidColorBrush(Colors.Black)
            };
            Canvas.SetZIndex(line, 0); //make sure the line is underneath everything

            Binding bindingStroke = new Binding("SelectedBrush") //this will bind the stroke of the line to the colour picker
            {
                Source = colourPickerLine,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            line.SetBinding(Line.StrokeProperty, bindingStroke); 

            Binding bindingV1X = new Binding //this will bind the X1 coordinate of the line to the smaller ellipse
            {
                Source = smallerEllipse,
                Path = new PropertyPath(Canvas.LeftProperty),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            line.SetBinding(Line.X1Property, bindingV1X);

            Binding bindingV1Y = new Binding //this will bind the Y1 coordinates of the line to the smaller ellipse
            {
                Source = smallerEllipse,
                Path = new PropertyPath(Canvas.TopProperty),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            line.SetBinding(Line.Y1Property, bindingV1Y);

            Binding bindingV2X = new Binding //this will bind the X2 coordinate of the line to the larger ellipse
            {
                Source = largerEllipse,
                Path = new PropertyPath(Canvas.LeftProperty),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            line.SetBinding(Line.X2Property, bindingV2X);

            Binding bindingV2Y = new Binding //this will bind the Y2 coordinate of the line to the larger ellipse
            {
                Source = largerEllipse,
                Path = new PropertyPath(Canvas.TopProperty),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            line.SetBinding(Line.Y2Property, bindingV2Y);
            
            //below creates the label which represents the weight of the line
            TextBlock weightLabel = new TextBlock() //set its properties
            {
                Text = weight.ToString(),
                Name = "labelFor" + lineName,
                FontSize = 15,
                IsHitTestVisible = false
            };

            Binding bindingFG = new Binding("SelectedBrush") //binds the foreground of the label to the colour picker
            {
                Source = colourPickerLabel,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            weightLabel.SetBinding(TextBlock.ForegroundProperty, bindingFG);

            Binding bindingWeightBackcolor = new Binding("SelectedBrush")//binding the back colour of the weight to the colour 
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
            Canvas.SetZIndex(weightLabel, 1); //needs to be visible above the line
            line.MouseMove += mouseMove;
            //add a new edge tuple to the list
            edgeList.Add(Tuple.Create(line, smallerEllipse, largerEllipse, weightLabel));
            List<int> order = new List<int>() { Convert.ToInt32(v1.Name.Substring(3)), Convert.ToInt32(v2.Name.Substring(3)) };
            //InitiateHighlightPathStoryboard(edgeAsList, TimeSpan.FromSeconds((rendering) ? 0.75 : 0.2), false);
            InitiateLineStoryboard(line, TimeSpan.FromSeconds((rendering)? 0.75: 0.2), order); //start line storyboard
            if (weight != 0)
                mainCanvas.Children.Add(weightLabel);
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
        public Tuple<Line, Ellipse, Ellipse, TextBlock> FindEdge(string lineName)
        {
            foreach (Tuple<Line, Ellipse, Ellipse, TextBlock> edge in edgeList)
            {
                if (edge.Item1.Name == lineName)//detetcs if theres a path because theres a matching name
                {
                    return edge;
                }
            }
            return null;
        }
        public Ellipse FindEllipse(int vertexId) //returns the ellipse that matches to an Id
        {
            foreach (var ctrl in mainCanvas.Children)
            {
                try //incase the ctrl is not an ellipse
                {
                    Ellipse currentEllipse = (Ellipse)ctrl;
                    if (currentEllipse.Name.Substring(3) == vertexId.ToString()) //when youve found a match
                    {
                        return currentEllipse; 
                    }
                }
                catch { }
            }
            return null;
        }
        public TextBlock FindLabel(int vertexId)
        {
            foreach (var ctrl in mainCanvas.Children)
            {
                try
                {
                    TextBlock currentLabel = (TextBlock)ctrl;
                    if (currentLabel.Name.Substring(8) == vertexId.ToString())
                    {
                        return currentLabel;
                    }
                }
                catch { }
            }
            return null;
        }
        public HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>> GetListOfEdgesFromVertex(Ellipse activeVertex) //gets all of the edges coming out of a vertex
        {
            HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>> listOfEdges = new HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>>(); //data to return
            foreach (Ellipse vertex in vertexList) //loop through vertex list
            {
                if (vertex != activeVertex)//don't check for an edge from itself to itself
                {
                    Ellipse largerEllipse = GetMaxEllipse(vertex, activeVertex);
                    Ellipse smallerEllipse = GetMinEllipse(vertex, activeVertex); 
                    string lineNameToFind = "line" + smallerEllipse.Name.Substring(3).ToString() + "to" + largerEllipse.Name.Substring(3).ToString(); //the name of the line that we are expecting to find
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
        public void CreateNewGraph(string graphName, bool rendering = false) //creates a new graph
        {
            bool AlreadyExists = false; //tells us whether we should carry on with creating the graph later on
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            //check whether a graph with the given name already exists for that type of user
            if (StudentIsLoggedIn() && !rendering)
            {
                cmd.CommandText = $"SELECT * FROM StudentGraph WHERE StudentID = '{loggedStudent.ID}' AND GraphName = '{graphName}'";
                OleDbDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    AlreadyExists = true;
                    MessageBox.Show("There is a previously saved graph with this name, go to the menu to load it.");
                }
            }
            else if (TeacherIsLoggedIn() && !rendering)
            {
                cmd.CommandText = $"SELECT * FROM TeacherGraph WHERE TeacherID = '{loggedTeacher.ID}' AND GraphName = '{graphName}'";
                OleDbDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    AlreadyExists = true;
                    MessageBox.Show("There is a previously saved graph with this name, go to the menu to load it.");
                }
            }
            else if (!rendering)
            {
                cmd.CommandText = $"SELECT * FROM GuestGraph WHERE GraphName = '{graphName}'";
                OleDbDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    AlreadyExists = true;
                    MessageBox.Show("There is a previously saved graph with this name, go to the menu to load it.");
                }
            }
            if (!AlreadyExists)
            {
                //re-initiliase everything
                edgeList = new HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>>();
                Graph = new Graph();
                valencyState = "Hidden";
                valencyList = new List<TextBlock>();
                Graph.Name = graphName; 
                vertexTxBoxList = new List<TextBlock>();
                vertexList = new List<Ellipse>();
                graphCreated = true;
                labelGraphName.Content = Graph.Name;
                EnableAllActionButtons(); //can only navigate buttons when a graph is created
                EnableAllAlgoButtons();
                EnableTbCtrl();
                btnDeleteGraph.IsEnabled = true;
                btnSaveGraph.IsEnabled = true;
            }
        }
        public void ResetSelectionCounts()
        {
            buttonSelectionCount = 0;
            dijkstraSelectionCount = 0;
            rInspSelectionCount = 0;
        }
        public void DeleteGraph()
        {
            //set all of the variables to null
            DisableAllActionButtons();
            DisableAllAlgoButtons();
            mainCanvas.Children.Clear();
            btnDeleteGraph.IsEnabled = false;
            labelGraphName.Content = "";
            Graph = new Graph();
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
            btnSaveGraph.IsEnabled = false;
        }
        public void GenerateAdjList()
        {
            txAdjset.Text = Graph.PrintAdjList();
        }
        public List<List<int>> GenerateAdjMat() //makes the adjacenct list appear in the provided area
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
            return adjMat;
            //return incase its necessary for future use
        }
        private void StudentLogInProcess()
        {
            btnRegisterStudent.IsEnabled = false;
            btnLogin.IsEnabled = false;
            btnRegisterTeacher.IsEnabled = false;
            btnLogOut.IsEnabled = true;
            DeleteGraph();
        }
        private void TeacherLogInProcess()
        {
            btnRegisterStudent.IsEnabled = false;
            btnLogin.IsEnabled = false;
            btnRegisterTeacher.IsEnabled = false;
            btnLogOut.IsEnabled = true;
            DeleteGraph();
        }
        private void LogOutProcess()
        {
            loggedStudent = null;
            loggedTeacher = null;
            btnLogin.IsEnabled = true;
            btnRegisterStudent.IsEnabled = true;
            btnRegisterTeacher.IsEnabled = true;
            btnLogOut.IsEnabled = false;
            txLoggedID.Content = "";
            txLoggedInAs.Content = "Logged in as: Guest";
            DeleteGraph();

        }
        public void SaveGraph()
        {
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            OleDbCommand cmd = new OleDbCommand();
            conn.Open();
            cmd.Connection = conn;
            string filename = Graph.Name; //change this to have a filename that the user wants
            FileStream fs;
            if (StudentIsLoggedIn()) //do this portion if the user is saving graphs as a student
            {
                filename += loggedStudent.ID;
                filename = "StudentGraphs/" + filename; //format the filename we expect to find
                if (!File.Exists(filename)) //if that file didnt already exist then create the file and write the class instance to that file
                {
                    fs = File.Create(filename);
                    //update the database to include that new file
                    cmd.CommandText = $"INSERT INTO StudentGraph VALUES('{filename}','{loggedStudent.ID}','{Graph.Name}','{DateTime.Today.ToString("dd/MM/yyyy")}','{Graph.GetNumberOfVertices()}','{Graph.GetNumberOfEdges()}','{"s"}')";
                    cmd.ExecuteNonQuery();
                    fs.Close();
                    //write the class instance to the binary file
                    BinarySerialization.WriteToBinaryFile(filename, Graph, false);
                    MessageBox.Show("Graph saved");
                }
                else
                {
                    //if the file did exist then ask the user if they want to over write
                    Overwrite overwrite = new Overwrite();
                    if (overwrite.ShowDialog() == true)
                    {
                        //update the record since they are overwriting
                        cmd.CommandText = $"UPDATE StudentGraph SET NoVertices = {Graph.GetNumberOfVertices()}, NoEdges = {Graph.GetNumberOfEdges()} WHERE Filename = '{filename}' AND StudentID = '{loggedStudent.ID}'";
                        BinarySerialization.WriteToBinaryFile(filename, Graph, false);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Graph saved");
                    }
                }
            }
            else if (TeacherIsLoggedIn()) //do this portion if the user is saving graphs as a teacher
            {
                filename += loggedTeacher.ID;
                filename = "TeacherGraphs/" + filename; //format the filename we expect we find
                if (!File.Exists(filename))//if that file didnt already exist then create the file and write the class instance to that file
                {
                    fs = File.Create(filename);
                    cmd.CommandText = $"INSERT INTO TeacherGraph VALUES('{filename}','{loggedTeacher.ID}','{Graph.Name}','{DateTime.Today.ToString("dd/MM/yyyy")}','{Graph.GetNumberOfVertices()}','{Graph.GetNumberOfEdges()}','{"t"}')";
                    cmd.ExecuteNonQuery();
                    fs.Close();
                    //write the class instance to the database
                    BinarySerialization.WriteToBinaryFile(filename, Graph, false);
                    MessageBox.Show("Graph saved");
                }
                else
                {
                    //if the file did exist then ask the user if they want to over write
                    Overwrite overwrite = new Overwrite();
                    if (overwrite.ShowDialog() == true)
                    {
                        //update the record since they are overwriting
                        cmd.CommandText = $"UPDATE TeacherGraph SET NoVertices = {Graph.GetNumberOfVertices()}, NoEdges = {Graph.GetNumberOfEdges()} WHERE Filename = '{filename}' AND TeacherID = '{loggedTeacher.ID}'";
                        cmd.ExecuteNonQuery();
                        BinarySerialization.WriteToBinaryFile(filename, Graph, false);
                        MessageBox.Show("Graph saved");
                    }
                }
            }
            else //this is if the user is saving as a guest
            {
                filename = "GuestGraphs/" + filename; //format the filename
                if (!File.Exists(filename)) //if the file doesnt already exist
                {
                    fs = File.Create(filename);
                    //update the database
                    cmd.CommandText = $"INSERT INTO GuestGraph VALUES('{filename}','{Graph.Name}','{"g"}')";
                    cmd.ExecuteNonQuery();
                    fs.Close();
                    //write it to the file
                    BinarySerialization.WriteToBinaryFile(filename, Graph, false);
                    MessageBox.Show("Graph Saved");
                }
                else
                {
                    Overwrite overwrite = new Overwrite();
                    if (overwrite.ShowDialog() == true)
                    {
                        BinarySerialization.WriteToBinaryFile(filename, Graph, false);
                        MessageBox.Show("Graph saved");
                    }
                }
            }
        }
        public void LoadGraph()
        {

            LoadGraph loadGraph = new LoadGraph(); //open the loadgraph window
            if (loadGraph.ShowDialog() == true)
            {
                string fileName = loadGraph.graphToLoad; //get the file name from the other window
                if (fileName == "fail")
                {
                    MessageBox.Show("Please select one of the listed graphs");
                }
                else
                {
                    Graph toLoad = BinarySerialization.ReadFromBinaryFile<Graph>(fileName); //read the file into the toLoad class instance
                    RenderGraph(toLoad); //render the just-loaded graph onto the screen 
                    btnDeleteGraph.IsEnabled = true;
                }

            }


        }
        private void DeactivateButton() //'deactivates' button
        {
            if (currentButton != null)
                currentButton.Background = new SolidColorBrush(Color.FromRgb(221, 221, 221));
        }
        public void ClearHighlightedLines()
        {
            foreach (Line line in linesToDelete)
            {
                mainCanvas.Children.Remove(line);
            }
            linesToDelete.Clear();
        }
        private void ActivateButton(object btnSender) //highlight a button when its pressed
        {
            RevertEllipseColour();
            RevertLineColour();
            ClearHighlightedLines();
            livePath.Clear(); //incase they were in the midst of the highlight path action
            if (btnSender != null) //make sure that the button isnt null
            {
                if (currentButton != (Button)btnSender) //if the same button is not pressed
                {
                    DeactivateButton(); //'deactivate' the previous button
                    currentButton = (Button)btnSender;
                    currentButton.Background = new SolidColorBrush(btnActivatedColour); //'activate' the current button
                }
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

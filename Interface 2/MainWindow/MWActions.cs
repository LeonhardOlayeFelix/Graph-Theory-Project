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
        /// <summary>
        /// Highlights a button that has been pressed
        /// </summary>
        /// <param name="btnSender">Button that was pressed</param>
        /// <summary>
        /// Responsible for re-initialisation of attributes when a new graph is created
        /// </summary>
        /// <param name="graphName">The name of the new graph</param>
        /// <param name="rendering">True only if this method is being used to load a previously saved graph</param>
        public void CreateNewGraph(string graphName, bool rendering = false)
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
                edgeList = new List<Tuple<Line, Ellipse, Ellipse, TextBlock>>();
                graph = new Graph();
                valencyState = "Hidden";
                valencyList = new List<TextBlock>();
                graph.Name = graphName;
                vertexTxBoxList = new List<TextBlock>();
                vertexList = new List<Ellipse>();
                graphCreated = true;
                labelGraphName.Content = graph.Name;
                EnableAllActionButtons(); //can only navigate buttons when a graph is created
                EnableAllAlgorithmButtons();
                EnableTabControl();
                btnDeleteGraph.IsEnabled = true;
                btnSaveGraph.IsEnabled = true;
            }
        }
        /// <summary>
        /// Clears all of the highlighted lines that are on the screen
        /// </summary>
        public void ClearHighlightedLines()
        {
            foreach (Line line in linesToDelete)
            {
                mainCanvas.Children.Remove(line);
            }
            linesToDelete.Clear();
        }
        /// <summary>
        /// Clears all the Operations so no action can be performed
        /// </summary>
        private void ClearAllOperations()
        {
            leftClickCanvasOperation = () => { };
            leftClickVertexOperation = (activeVertex) => { };
            leftClickLineOperation = (activeLine) => { };
            leftMouseButtonUpOperation = () => { };
        }
        /// <summary>
        /// Connects two vertices together
        /// </summary>
        /// <param name="v1">Vertex from one end of the edge</param>
        /// <param name="v2">Vertex from the other end of the edge</param>
        /// <param name="weight">Edge Weight</param>
        /// <param name="rendering">True only if this method is being used to load a previously saved graph</param>
        private void ConnectVertices(Ellipse v1, Ellipse v2, int weight, bool rendering = false, bool floyds = false) 
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
                    if (rendering || floyds) { return; }
                    DeleteEdge(edge, rendering);//delete the edge
                    break;
                }
            }
            if (!rendering && !floyds)
            {
                graph.AddEdge(Convert.ToInt32(v1.Name.Substring(3)), Convert.ToInt32(v2.Name.Substring(3)), weight); //update the object
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
            if (!floyds)
            {
                edgeList.Add(Tuple.Create(line, smallerEllipse, largerEllipse, weightLabel));
            }
            List<int> order = new List<int>() { Convert.ToInt32(v1.Name.Substring(3)), Convert.ToInt32(v2.Name.Substring(3)) };
            //InitiateHighlightPathStoryboard(edgeAsList, TimeSpan.FromSeconds((rendering) ? 0.75 : 0.2), false);
            InitiateLineStoryboard(line, TimeSpan.FromSeconds(0.2), order); //start line storyboard
            if (weight != 0)
                mainCanvas.Children.Add(weightLabel);
            if (floyds) { line.Stroke = HighlightColour; }
            GenerateAdjList();
            GenerateAdjMat();
        }
        /// <summary>
        /// Responsible for decreasing the selection counts
        /// </summary>
        /// <param name="count">The type of selection count to be decremented</param>
        public void DecrementSelectionCount(ref int count)
        {
            count -= 1;
            labelExtraInfo.Content = "";
            RevertEllipseColour();
            EnableTabControl();
            EnableAllAlgorithmButtons();
            EnableAllActionButtons();
            btnSaveGraph.IsEnabled = true;
            btnLoadGraph.IsEnabled = true;
        }
        /// <summary>
        /// Responsible for the cleanup when a graph is deleted
        /// </summary>
        public void DeleteGraph()
        {
            //set all of the variables to null
            DisableAllActionButtons();
            DisableAllAlgorithmButtons();
            mainCanvas.Children.Clear();
            btnDeleteGraph.IsEnabled = false;
            labelGraphName.Content = "";
            graph = new Graph();
            ResetSelectionCounts();
            HideValencies();
            buttonId = 0; 
            graph = null;
            lastSelectedVertex = null;
            vertexTxBoxList = new List<TextBlock>();
            vertexList = new List<Ellipse>();
            edgeList = new List<Tuple<Line, Ellipse, Ellipse, TextBlock>>();
            graphCreated = false;
            btnSaveGraph.IsEnabled = false;
        }
        /// <summary>
        /// Deactivates the currently activated button
        /// </summary>
        private void DeactivateButton()
        {
            if (currentButton != null)
                currentButton.Background = new SolidColorBrush(Color.FromRgb(221, 221, 221));
        }
        /// <summary>
        /// Returns the ellipse that corresponds with a specified vertex ID
        /// </summary>
        /// <param name="vertexId">The ID of the Vertex whose Ellipse is to be found</param>
        /// <returns></returns>
        public Ellipse FindEllipse(int vertexId)
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
        /// <summary>
        /// Returns the label that corresponds with a specified vertex ID
        /// </summary>
        /// <param name="vertexId">The ID of the vertex whose Label is to be found</param>
        /// <returns></returns>
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
        /// <summary>
        /// Returns the edge that corresponds with a specified name
        /// </summary>
        /// <param name="lineName">Name of the edge of the form "lineatob" where a < b</param>
        /// <returns></returns>
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
        /// <summary>
        /// Returns the Ellipse that has the smaller ID
        /// </summary>
        /// <param name="Ellipse1"></param>
        /// <param name="Ellipse2"></param>
        /// <returns></returns>
        private Ellipse GetMinEllipse(Ellipse Ellipse1, Ellipse Ellipse2)
        {
            return (Convert.ToInt32(Ellipse1.Name.Substring(3)) < Convert.ToInt32(Ellipse2.Name.Substring(3))) ? Ellipse1 : Ellipse2;
        }
        /// <summary>
        /// Returns the Ellipse that has the larger ID
        /// </summary>
        /// <param name="Ellipse1"></param>
        /// <param name="Ellipse2"></param>
        /// <returns></returns>
        private Ellipse GetMaxEllipse(Ellipse Ellipse1, Ellipse Ellipse2)
        {
            return (Convert.ToInt32(Ellipse1.Name.Substring(3)) > Convert.ToInt32(Ellipse2.Name.Substring(3))) ? Ellipse1 : Ellipse2;
        }
        /// <summary>
        /// Returns the smaller number of a and b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int GetMax(int a, int b)
        {
            return (a > b) ? a : b;
        }
        /// <summary>
        /// Returns the larger number of a and b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int GetMin(int a, int b)
        {
            return (a < b) ? a : b;
        }
        /// <summary>
        /// Updates the on-screen textbox which displays the adjacency list
        /// </summary>
        public void GenerateAdjList()
        {
            txAdjset.Text = graph.PrintAdjList();
        }
        /// <summary>
        /// Returns a Hashset of all of the edges coming out an Ellipse
        /// </summary>
        /// <param name="activeEllipse">Ellipse to find neighbours for</param>
        /// <returns></returns>
        public HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>> GetListOfEdgesFromVertex(Ellipse activeEllipse)
        {
            HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>> listOfEdges = new HashSet<Tuple<Line, Ellipse, Ellipse, TextBlock>>(); //data to return
            foreach (Ellipse vertex in vertexList) //loop through vertex list
            {
                if (vertex != activeEllipse)//don't check for an edge from itself to itself
                {
                    Ellipse largerEllipse = GetMaxEllipse(vertex, activeEllipse);
                    Ellipse smallerEllipse = GetMinEllipse(vertex, activeEllipse); 
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
        /// <summary>
        /// Makes the adjacency list appear in the on-screen area
        /// </summary>
        /// <returns></returns>
        public void GenerateAdjMat()
        {
            Func<int, bool> function = weight => weight == -1;
            populateDataGrid(dataGridAdjacencyMatrix, graph.GetAdjacencyMatrix(), function);
        }
        public void populateDataGrid(DataGrid datagrid, int[,] list, Func<int,bool> noPathFunction)
        {
            DataTable dt = new DataTable();
            int vertex = 0;
            int nbColumns = graph.GetMaxVertexID() + 1;
            int nbRows = graph.GetMaxVertexID() + 1; ;
            dt.Columns.Add("-");
            for (int i = 0; i < nbColumns; i++)
            {
                dt.Columns.Add(i.ToString(), typeof(double));
            }

            for (int row = 0; row < nbRows; row++)
            {
                DataRow dr = dt.NewRow();
                for (int col = 0; col < nbColumns + 1; col++)
                {
                    if (col == 0)
                    {
                        dr[col] = vertex++;
                    }
                    else
                    {
                        int weight = list[row, col - 1];
                        if (!noPathFunction(weight))
                        {
                            dr[col] = (noPathFunction(weight)) ? -1 : weight;
                        }
                    }
                }
                dt.Rows.Add(dr);
            }
            datagrid.ItemsSource = dt.DefaultView;
        }
        /// <summary>
        /// Hides all of the valencies
        /// </summary>
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
        /// <summary>
        /// Manages the buttons that should be enabled and disabled once a user logs out
        /// </summary>
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
            tabControlClass.IsEnabled = false;
            tabControlAssignments.IsEnabled = false;
        }
        /// <summary>
        /// Loads a previously-saved graph
        /// </summary>
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
        /// <summary>
        /// Reinitialise all of the selection counts back to 0
        /// </summary>
        public void ResetSelectionCounts()
        {
            buttonSelectionCount = 0;
            dijkstraSelectionCount = 0;
            rInspSelectionCount = 0;
        }
        /// <summary>
        /// rebinds the colour of the Lines in the canvas to the colour picker
        /// </summary>
        public void RevertLineColour()
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
        /// <summary>
        /// rebinds the colour of the Ellipses in the canvas to the colour picker
        /// </summary>
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
        /// <summary>
        /// Manages the buttons that should be enabled and disabled once a student logs in
        /// </summary>
        private void StudentLogInProcess()
        {
            btnRegisterStudent.IsEnabled = false;
            btnLogin.IsEnabled = false;
            btnRegisterTeacher.IsEnabled = false;
            btnLogOut.IsEnabled = true;
            tabControlClass.IsEnabled = false;
            tabControlAssignments.IsEnabled = true;
            DeleteGraph();
        }
        /// <summary>
        /// Saves the graph that is currently made to the database
        /// </summary>
        public void SaveGraph()
        {
            if (graph.Name == "")
            {
                string newName = "";
                NameCreatedGraph nameGraphWindow = new NameCreatedGraph(); //create an instance of the new window
                nameGraphWindow.ShowDialog(); //opens a new window
                if (nameGraphWindow.DialogResult == true) //if they pressed ok rather than the exit button
                {
                    newName = nameGraphWindow.txBoxGraphName.Text; //re-initialise everything:
                    graph.Name = newName;
                }
                else 
                {
                    return;
                }
            }
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            OleDbCommand cmd = new OleDbCommand();
            conn.Open();
            cmd.Connection = conn;
            string filename = graph.Name; //change this to have a filename that the user wants
            FileStream fs;
            if (StudentIsLoggedIn()) //do this portion if the user is saving graphs as a student
            {
                filename += loggedStudent.ID;
                filename = "StudentGraphs/" + filename; //format the filename we expect to find
                if (!File.Exists(filename)) //if that file didnt already exist then create the file and write the class instance to that file
                {
                    fs = File.Create(filename);
                    //update the database to include that new file
                    cmd.CommandText = $"INSERT INTO StudentGraph VALUES('{filename}','{loggedStudent.ID}','{graph.Name}','{DateTime.Today.ToString("dd/MM/yyyy")}','{graph.GetNumberOfVertices()}','{graph.GetNumberOfEdges()}','{"s"}')";
                    cmd.ExecuteNonQuery();
                    fs.Close();
                    //write the class instance to the binary file
                    BinarySerialization.WriteToBinaryFile(filename, graph, false);
                    MessageBox.Show("Graph saved");
                }
                else
                {
                    //if the file did exist then ask the user if they want to over write
                    Overwrite overwrite = new Overwrite();
                    if (overwrite.ShowDialog() == true)
                    {
                        //update the record since they are overwriting
                        cmd.CommandText = $"UPDATE StudentGraph SET NoVertices = {graph.GetNumberOfVertices()}, NoEdges = {graph.GetNumberOfEdges()} WHERE Filename = '{filename}' AND StudentID = '{loggedStudent.ID}'";
                        BinarySerialization.WriteToBinaryFile(filename, graph, false);
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
                    cmd.CommandText = $"INSERT INTO TeacherGraph VALUES('{filename}','{loggedTeacher.ID}','{graph.Name}','{DateTime.Today.ToString("dd/MM/yyyy")}','{graph.GetNumberOfVertices()}','{graph.GetNumberOfEdges()}','{"t"}')";
                    cmd.ExecuteNonQuery();
                    fs.Close();
                    //write the class instance to the database
                    BinarySerialization.WriteToBinaryFile(filename, graph, false);
                    MessageBox.Show("Graph saved");
                }
                else
                {
                    //if the file did exist then ask the user if they want to over write
                    Overwrite overwrite = new Overwrite();
                    if (overwrite.ShowDialog() == true)
                    {
                        //update the record since they are overwriting
                        cmd.CommandText = $"UPDATE TeacherGraph SET NoVertices = {graph.GetNumberOfVertices()}, NoEdges = {graph.GetNumberOfEdges()} WHERE Filename = '{filename}' AND TeacherID = '{loggedTeacher.ID}'";
                        cmd.ExecuteNonQuery();
                        BinarySerialization.WriteToBinaryFile(filename, graph, false);
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
                    cmd.CommandText = $"INSERT INTO GuestGraph VALUES('{filename}','{graph.Name}','{"g"}')";
                    cmd.ExecuteNonQuery();
                    fs.Close();
                    //write it to the file
                    BinarySerialization.WriteToBinaryFile(filename, graph, false);
                    MessageBox.Show("Graph Saved");
                }
                else
                {
                    Overwrite overwrite = new Overwrite();
                    if (overwrite.ShowDialog() == true)
                    {
                        BinarySerialization.WriteToBinaryFile(filename, graph, false);
                        MessageBox.Show("Graph saved");
                    }
                }
            }
        }
        /// <summary>
        /// Display all of the valencies on the screen
        /// </summary>
        public void ShowValencies()
        {
            int sumValency = 0; //represents the total valency
            foreach (Ellipse vertex in vertexList)
            {
                TextBlock valency = new TextBlock()
                {
                    FontSize = 20,
                    Name = "Val" + vertex.Name.Substring(3), //set the name to Val(buttonId)
                    Text = graph.GetValency(Convert.ToInt32(vertex.Name.Substring(3))).ToString() //calls the graph class, which gets the valency of a vertex 
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
        /// <summary>
        /// Manages the buttons that should be enabled and disabled once a student logs in
        /// </summary>
        private void TeacherLogInProcess()
        {
            btnRegisterStudent.IsEnabled = false;
            btnLogin.IsEnabled = false;
            btnRegisterTeacher.IsEnabled = false;
            btnLogOut.IsEnabled = true;
            tabControlClass.IsEnabled = true;
            tabControlAssignments.IsEnabled = true;
            DeleteGraph();
        }
    }
}

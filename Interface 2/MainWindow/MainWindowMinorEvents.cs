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
using System.IO;
using System.Data.OleDb;
using System.Windows.Media.Animation;

namespace Interface_2
{
    public partial class MainWindow : Window
    {
        private void ActivateButton(object btnSender) //highlight a button when its pressed
        {
            RevertEllipseColour();
            RevertLineColour();
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
        private void DeactivateButton() //'deactivates' button
        {
            if (currentButton != null)
                currentButton.Background = new SolidColorBrush(Color.FromRgb(221, 221, 221));
        }
        private void btnDepthFirst_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
            HideValencies();
            labelExtraInfo.Content = "Choose a root vertex";
        }
        private void btnBreadthFirst_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
            HideValencies();
            labelExtraInfo.Content = "Choose a root vertex";
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
        public void WidenObject(double newDiameter, TimeSpan duration, Ellipse vertex)
        {
            DoubleAnimation animation = new DoubleAnimation(newDiameter, duration);
            animation.FillBehavior = FillBehavior.Stop;
            animation.Completed += new EventHandler(Story_Completed);
            vertex.BeginAnimation(Ellipse.HeightProperty, animation);
            vertex.BeginAnimation(Ellipse.WidthProperty, animation);
        }
        public void Story_Completed(object sender, EventArgs e)
        {
            Binding bindingDiameter = new Binding("Value")//binding the diameter of the vertices to the slider
            {
                Source = vertexDiameterSlider,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            foreach (var ctrl in mainCanvas.Children)
            {
                try
                {
                    Ellipse vertex = (Ellipse)ctrl;
                    vertex.SetBinding(Ellipse.HeightProperty, bindingDiameter);
                    vertex.SetBinding(Ellipse.WidthProperty, bindingDiameter);

                }
                catch { }
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
        private void btnLoadGraph_Click(object sender, RoutedEventArgs e)
        {
            LoadGraph();

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
            else if(TeacherIsLoggedIn()) //do this portion if the user is saving graphs as a teacher
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
        private void btnSaveGraph_Click(object sender, RoutedEventArgs e)   
        {
            SaveGraph();
        }
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

        private void btnTakeScreenshot_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Screenshot Taken";
            ActivateButton(sender);
        }
        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            labelExtraInfo.Content = "Click freely around the Canvas";
            ActivateButton(sender);
        }
       
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
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
        private void btnRegisterStudent_Click(object sender, RoutedEventArgs e)
        {
            RegisterStudent registerstudent = new RegisterStudent();
            registerstudent.ShowDialog();
        }
        private void btnRegisterTeacher_Click(object sender, RoutedEventArgs e)
        {
            RegisterTeacher registerteacher = new RegisterTeacher();
            registerteacher.ShowDialog();
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
       

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            LogOutProcess();
        }

    }
}

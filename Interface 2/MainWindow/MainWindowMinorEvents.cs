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

        public void LoadGraph()
        {
            //OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            //OleDbCommand cmd = new OleDbCommand();
            //conn.Open();
            //cmd.Connection = conn;
            //cmd.CommandText = "SELECT GraphName FROM Graph";
            //OleDbDataReader reader2 = cmd.ExecuteReader();
            //if (!reader2.HasRows)
            //{
            //    MessageBox.Show("No graphs have previously been saved");
            //}
            //else
            //{
            //    LoadGraph loadGraph = new LoadGraph();
            //    if (loadGraph.ShowDialog() == true)
            //    {
            //        string fileName = loadGraph.graphToLoad;
            //        if (fileName == "fail")
            //        {
            //            MessageBox.Show("Please select one of the listed graphs");
            //        }
            //        else
            //        {
            //        Network toLoad = BinarySerialization.ReadFromBinaryFile<Network>(fileName); //change this to have a file the user wants to open
            //        RenderGraph(toLoad, fileName); //change this to have a file the user wants to open
            //        btnDeleteGraph.IsEnabled = true;
            //        }
                    
            //    }
            //}
            
            
        }
        private void btnLoadGraph_Click(object sender, RoutedEventArgs e)
        {
            LoadGraph();

        }

        public void SaveGraph()
        {
            //OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            //OleDbCommand cmd = new OleDbCommand();
            //conn.Open();
            //cmd.Connection = conn;
            //string filename = Graph.Name; //change this to have a filename that the user wants
            //FileStream fs;
            //if (!File.Exists(filename))
            //{
            //    fs = File.Create(filename);
            //    cmd.CommandText = "INSERT INTO Graph VALUES('" + filename + "','" + DateTime.Today.ToString("MM/dd/yyyy") + "')";
            //    cmd.ExecuteNonQuery();
            //    fs.Close();
            //    BinarySerialization.WriteToBinaryFile(filename, Graph, false);
            //}
            //else
            //{
            //    Overwrite overwrite = new Overwrite();
            //    if (overwrite.ShowDialog() == true)
            //    {
            //        BinarySerialization.WriteToBinaryFile(filename, Graph, false);
            //    }
            //}
            //MessageBox.Show("Graph saved");
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
            this.Hide();
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
            this.Show();
            
        }
        private void btnRegisterStudent_Click(object sender, RoutedEventArgs e)
        {
            RegisterStudent registerstudent = new RegisterStudent();
            this.Hide();
            registerstudent.ShowDialog();
            this.Show();
        }
        private void btnRegisterTeacher_Click(object sender, RoutedEventArgs e)
        {
            RegisterTeacher registerteacher = new RegisterTeacher();
            this.Hide();
            registerteacher.ShowDialog();
            this.Show();
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

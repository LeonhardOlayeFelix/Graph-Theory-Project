using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.OleDb;
namespace Interface_2
{
    /// <summary>
    /// Interaction logic for LoginStudent.xaml
    /// </summary>
    public partial class LoginStudent : Window
    {
        public Student studentJustLogged = null;
        public Teacher teacherJustLogged = null;
        public LoginStudent()
        {
            InitializeComponent();
        }

        private void btnExitStudentLogin_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnLoginStudentConfirm_Click(object sender, RoutedEventArgs e)
        {
            bool isTeacher = false;
            bool isStudent = false;
            string email = txEmail.Text;
            string password = txPassword.Text;
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            bool correctCredentials = false;
            cmd.CommandText = $"SELECT * FROM Student WHERE Email = '{email}' AND Spassword = '{password}'"; //look for where both the password and email match
            OleDbCommand cmd2 = new OleDbCommand();
            cmd2.Connection = conn;
            cmd2.CommandText = $"SELECT * FROM Teacher WHERE Email = '{email}' AND Tpassword = '{password}'";
            OleDbDataReader reader = cmd.ExecuteReader();
            OleDbDataReader reader2 = cmd2.ExecuteReader();
            if (reader.HasRows) //if they entered the credentials for a student
            {
                isStudent = true;
                correctCredentials = true;
            }
            else if (reader2.HasRows) //if they entered the credentials for a teacher
            {
                isTeacher = true;
                correctCredentials = true;
            }
            if (correctCredentials && isStudent)
            {
                studentJustLogged = MainWindow.InitialiseStudent(MainWindow.GetStudentID(email)); //updates the student just logged in instance
                this.DialogResult = true;
                this.Close();
            }
            else if (correctCredentials && isTeacher)
            {
                teacherJustLogged = MainWindow.InitialiseTeacher(MainWindow.GetTeacherID(email)); //updates the teacher just logged in instance
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Email or Password was incorrect. Please try again.");
            }
        }
    }
}

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
using System.Net.Mail;
namespace Interface_2
{
    /// <summary>
    /// Interaction logic for RegisterStudent.xaml
    /// </summary>
    public partial class RegisterStudent : Window
    {
        Student newStudent = null;
        public RegisterStudent()
        {
            InitializeComponent();
        }

        private void btnExitStudentLogin_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private static bool IsValidEmail(string email)
        {
            var valid = true;

            try
            {
                var emailAddress = new MailAddress(email);
            }
            catch
            {
                valid = false;
            }

            return valid;
        }

        private void btnRegisterStudentConfirm_Click(object sender, RoutedEventArgs e)
        {
            
            string email = txEmail.Text;
            string password = txPassword.Text;
            string firstname = txFirstname.Text;
            string lastname = txLastname.Text;
            string dob = dpDob.SelectedDate == null ? null : dpDob.SelectedDate.Value.ToString("dd/MM/yyyy");
            if (MainWindow.emailExists(email))
            {
                MessageBox.Show("This email is already in use");
            }
            else if (!IsValidEmail(email))
            {
                MessageBox.Show("Invalid Email enetered.");
            }
            else if (email.Length != 0 && password.Length != 0 && firstname.Length != 0 && lastname.Length != 0 && dob != null)
            {
                string ID = MainWindow.NextID("S");
                newStudent = new Student(firstname, lastname, email, password, dpDob.SelectedDate.Value, ID);
                MainWindow.SaveStudent(newStudent); //adds the student to the database using a function defined in another file
                this.DialogResult = true;
                this.Close();
                MessageBox.Show("Successuflly Signed Up. You can now log in with this account");
            }
            else
            {
                this.DialogResult = false;
                MessageBox.Show("Please fill all fields");
            }
        }
    }
}

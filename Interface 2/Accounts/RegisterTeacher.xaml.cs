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
using System.Net.Mail;
namespace Interface_2
{
    /// <summary>
    /// Interaction logic for RegisterTeacher.xaml
    /// </summary>
    public partial class RegisterTeacher : Window
    {
        Teacher newTeacher = null;
        string title = "";
        public RegisterTeacher()
        {
            InitializeComponent();
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
        private void btnRegisterTeacherConfirm_Click(object sender, RoutedEventArgs e)
        {
            string email = txEmail.Text;
            string password = txPassword.Text;
            string firstname = txFirstname.Text;
            string lastname = txLastname.Text;
            if (MainWindow.emailExists(email))
            {
                MessageBox.Show("This email is already in use");
            }
            else if (!IsValidEmail(email))
            {
                MessageBox.Show("Invalid Email enetered.");
            }
            else if (email.Length != 0 && password.Length != 0 && firstname.Length != 0 && lastname.Length != 0 && title.Length != 0)
            {
                string ID = MainWindow.NextID("T");
                newTeacher = new Teacher(firstname, lastname, email, password, title, ID);
                MainWindow.SaveTeacher(newTeacher); //add the new teacher to the database using a function defined in another file
                this.DialogResult = true;
                this.Close();
                MessageBox.Show("Successuflly Signed Up. You can now log in with this account");
            }
            else
            {
                MessageBox.Show("Please fill all fields");
            }
        }

        private void btnExitTeacherRegister_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void rbMr_Checked(object sender, RoutedEventArgs e)
        {
            title = "Mr";
        }

        private void rbMrs_Checked(object sender, RoutedEventArgs e)
        {
            title = "Mrs";
        }

        private void rbMiss_Checked(object sender, RoutedEventArgs e)
        {
            title = "Miss";
        }

        private void rbMs_Checked(object sender, RoutedEventArgs e)
        {
            title = "Ms";
        }
    }
}

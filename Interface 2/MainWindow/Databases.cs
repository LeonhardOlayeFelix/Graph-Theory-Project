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
using System.Data.OleDb;
using System.IO;

namespace Interface_2
{
    public partial class MainWindow : Window
    {
        public static bool SaveTeacher(Teacher teacher)
        {
            if (!emailExists(teacher.email)) //makes sure the teacher hasnt already signed up with this email
            {
                string ID = NextID("T"); //generates the next ID for teacher
                teacher.ID = ID; //initialises the teachers ID in the class
                OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                conn.Open();
                //insert the new teacher into the database
                cmd.CommandText = $"INSERT INTO Teacher VALUES('{ID}','{teacher.firstname}','{teacher.lastname}','{teacher.email}','{teacher.password}','{teacher.title}')";
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            return false;
        }//Saves a teacher to the database
        public static void SaveStudent(Student student)
        {
            if (!emailExists(student.email))//makes sure the student hasnt already signed up with this email
            {
                
                string ID = NextID("S"); //generates the next ID for the student
                student.ID = ID;//initiliases the students ID in the class
                OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                conn.Open();
                //insert the new student into the databse
                cmd.CommandText = $"INSERT INTO Student VALUES('{ID}', '{student.firstname}', '{student.lastname}','{student.dob}','{student.email}','{student.password}'," +
                    $"{0}, {0}, {0},{0},{0},{0},{0},'{DateTime.Today.ToString("dd/MM/yyyy")}')";
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }//saves a student to the database
        public static void CreateClass(string className, Teacher teacher)
        {
            string classID = NextID("C"); //generates the class ID 
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            conn.Open();
            //inserts the new class into the database
            cmd.CommandText = $"INSERT INTO Class VALUES('{classID}', '{teacher.ID}', '{className}')";
            cmd.ExecuteNonQuery();
            conn.Close();
        }//saves a new class into the database
        public static void DeleteClass(string classID)
        {
            OleDbConnection conn = new OleDbConnection(ConStr);
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            conn.Open();
            //delete all existing enrollments with that class
            cmd.CommandText = $"DELETE FROM ClassEnrollment WHERE ClassID = '{classID}'";
            cmd.ExecuteNonQuery();
            //now clear the class record
            cmd.CommandText = $"DELETE FROM Class WHERE ClassID = '{classID}'";
            cmd.ExecuteNonQuery();
        }//delete a class from the database
        public static bool emailExists(string email)
        {
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            OleDbCommand cmd2 = new OleDbCommand();
            cmd2.Connection = conn;
            //check in both the student and teacher tables
            cmd.CommandText = $"SELECT * FROM Student WHERE Email = '{email}'";
            cmd2.CommandText = $"SELECT * FROM Teacher WHERE Email = '{email}'";
            OleDbDataReader reader = cmd.ExecuteReader();
            OleDbDataReader reader2 = cmd2.ExecuteReader();
            if (reader.HasRows) //if either of these tables have this email then return true;
            {
                conn.Close();
                return true;
            }
            else if (reader2.HasRows)
            {
                conn.Close();
                return true;
            }
            conn.Close();
            return false;
        }//checks if the email passed in already exists in the database
        public static bool TeacherAlreadySaved(Teacher teacher)
        {
            string ID = teacher.ID;
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"SELECT * FROM Teacher WHERE TeacherID = '{teacher.ID}'";
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                conn.Close();
                return true;
            }
            conn.Close();
            return false;
        } //checks if a teacher is already in a database given a class instance
        public static bool StudentAlreadySaved(Student student)
        {
            string ID = student.ID;
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"SELECT * FROM Student WHERE StudentID = '{student.ID}'";
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                conn.Close();
                return true;
            }
            conn.Close();
            return false;
        } //checks if a student is already in a database given a class instance
        public static string GetStudentID(string email)
        {
            string ID = "";
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"SELECT * FROM Student WHERE Email = '{email}'"; //selects the single student with that email
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                try
                {
                    ID = cmd.ExecuteScalar().ToString();
                }
                catch { }
            }
            conn.Close();
            return ID;
        } //returns the ID of a student when passed their email
        public static string GetTeacherID(string email)
        {
            string ID = "";
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"SELECT * FROM Teacher WHERE Email = '{email}'"; //selects the single teacher with that email
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                try
                {
                    ID = cmd.ExecuteScalar().ToString();
                }
                catch { }
            }
            conn.Close();
            return ID;
        } //Returns the ID of the Teacher when passed their email
        public static void EnrollStudent(string ClassID, Student student)
        {
            if (!IsInClass(ClassID, student)) //only do this if the student is not already in this class
            {
                OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                conn.Open();
                //insert the record into the table
                cmd.CommandText = $"INSERT INTO ClassEnrollment VALUES('{ClassID}', '{GetStudentID(student.email)}','{student.firstname}', '{student.lastname}','{DateTime.Today}')";
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        } //enrolls a student into the passed in class
        public static void RemoveStudent(string ClassID, Student student)
        {
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            //deletes the record 
            cmd.CommandText = $"DELETE FROM ClassEnrollment WHERE ClassID = '{ClassID}' AND StudentID = '{student.ID}'";
            cmd.ExecuteNonQuery();
        } //remoevs a student from the class
        public static bool IsInClass(string ClassID, Student student)
        {
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            conn.Open();
            //selects the specified student in the specified class
            cmd.CommandText = $"SELECT * FROM ClassEnrollment WHERE ClassID = '{ClassID}' AND StudentID = '{student.ID}'";
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows) //if any matches were found
            {
                conn.Close();//theres a student so return true
                return true;
            }
            conn.Close();
            return false;
        }//returns whether a student is in a class
        public static List<Student> ListClass(string ClassID) //returns a list of all the students in a class
        {
            List<Student> Ids = new List<Student>();
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"SELECT * FROM ClassEnrollment WHERE ClassID = '{ClassID}'"; //selects each record in that class
            OleDbDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string ID = (string)reader["StudentID"]; //save each student found to the list
                Ids.Add(InitialiseStudent(ID));
            }
            conn.Close();
            return Ids;
        }
        public static Student InitialiseStudent(string StudentID)
        {
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"SELECT * FROM Student WHERE StudentID = '{StudentID}'"; //searches for the correct student record
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                Student student = new Student((string)reader["FirstName"], (string)reader["LastName"], (string)reader["Email"], (string)reader["SPassword"], (DateTime)reader["DateOfBirth"], (string)reader["StudentID"]); //intialise instance with correct params
                conn.Close();
                return student;
            }
            conn.Close();
            return null;
        }//initialises a student class instance given a student ID
        public static Teacher InitialiseTeacher(string TeacherID)
        {
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"SELECT * FROM Teacher WHERE TeacherID = '{TeacherID}'"; //searches for the correct Teacher record
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                Teacher teacher = new Teacher((string)reader["FirstName"], (string)reader["LastName"], (string)reader["Email"], (string)reader["TPassword"], (string)reader["Title"], (string)reader["TeacherID"]); //intialise instance with correct params
                conn.Close();
                return teacher;
            }
            conn.Close();
            return null;
        }//initialises a Teacher class instance given a Teacher ID
        public bool TeacherIsLoggedIn()
        {
            return (loggedTeacher != null);
        }
        public bool StudentIsLoggedIn()
        {
            return (loggedStudent != null);
        }
        public static string NextID(string IDType)
        {
            int NextID;
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            conn.Open();
            if (IDType == "T") //If i am generating a teachers ID
            {
                cmd.CommandText = "SELECT MAX(TeacherID) AS MaxID FROM Teacher";
            }
            else if (IDType == "S") //if i am generating a students ID
            {
                cmd.CommandText = "SELECT MAX(StudentID) AS MaxID FROM Student";
            }
            else if (IDType == "C") //if i am generating a class ID
            {
                cmd.CommandText = "SELECT MAX(ClassID) AS MaxID FROM Class";
            }
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                NextID = Convert.ToInt32(cmd.ExecuteScalar().ToString().Substring(1)) + 1; //set NextID to highest ID found
            }
            else
            {
                NextID = 1;
            }
            conn.Close();
            string ID = "0000" + NextID.ToString(); //format the ID once found the next ID
            ID = ID.Substring(ID.Length - 4);
            ID = ID.Insert(0, IDType);
            return ID;
        }
    }
}

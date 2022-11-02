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
        private bool Authorised(string classID)
        {
            string teacherID = loggedTeacher.ID;
            OleDbConnection conn = new OleDbConnection(ConStr);
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            conn.Open();
            cmd.CommandText = $"SELECT * FROM Class WHERE ClassID = '{classID}' AND TeacherID = '{teacherID}'";
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                conn.Close();
                return true;
            }
            conn.Close();
            return false;
        }
        public static void CreateDatabase()
        {
            if (!File.Exists("NetworkDB.accdb")) //if a file doesnt already exist for the database
            {
                //establish the connection and then create database 
                ADOX.Catalog cat = new ADOX.Catalog();
                cat.Create(ConStr);
                OleDbConnection conn = new OleDbConnection(ConStr);
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = "CREATE TABLE Student(StudentID VARCHAR(5), FirstName VARCHAR(30), LastName VARCHAR(30), DateOfBirth DATE, Email VARCHAR(100), SPassword VARCHAR(30), NoAssignmentsSubmitted INTEGER, NoDijkstras INTEGER, NoRInsp INTEGER, NoBFS INTEGER, NoDFS INTEGER, NoPrims INTEGER, NoGraph INTEGER, DateCreated DATE, PRIMARY KEY(StudentID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE Teacher(TeacherID VARCHAR(5), FirstName VARCHAR(30), LastName VARCHAR(30), Email VARCHAR(100), TPassword VARCHAR(30), Title VARCHAR(7), PRIMARY KEY(TeacherID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE StudentGraph(Filename VARCHAR(30), StudentID VARCHAR(5), GraphName VARCHAR(25), DateCreated DATE, NoVertices INTEGER, NoEdges INTEGER, CreatedBy CHAR(1), PRIMARY KEY(Filename), FOREIGN KEY (StudentID) REFERENCES Student(StudentID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE TeacherGraph(Filename VARCHAR(30), TeacherID VARCHAR(5), GraphName VARCHAR(25), DateCreated DATE, NoVertices INTEGER, NoEdges INTEGER, CreatedBy CHAR(1), PRIMARY KEY(Filename), FOREIGN KEY (TeacherID) REFERENCES Teacher(TeacherID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE GuestGraph(Filename VARCHAR(30), GraphName VARCHAR(25), CreatedBy CHAR(1), PRIMARY KEY(Filename))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE Assignment(AssignmentID VARCHAR(5), StudentID VARCHAR(5), Filename VARCHAR(30), SetBy VARCHAR(5), GraphName VARCHAR(25), DateSet DATE, DateDue DATE, isLate CHAR(1), isCompleted CHAR(1), PRIMARY KEY(AssignmentID), FOREIGN KEY (StudentID) REFERENCES Student(StudentID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE Class(ClassID VARCHAR(5), TeacherID VARCHAR(5), ClassName VARCHAR(30), PRIMARY KEY(ClassID), FOREIGN KEY (TeacherID) REFERENCES Teacher(TeacherID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE ClassEnrollment(ClassID VARCHAR(5), StudentID VARCHAR(5), FirstName VARCHAR(30), LastName VARCHAR(30), EnrollDate DATE, FOREIGN KEY (ClassID) REFERENCES Class(ClassID), FOREIGN KEY (StudentID) REFERENCES Student(StudentID))";
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        /// <summary>
        /// Saves a teacher to the database
        /// </summary>
        /// <param name="teacher">The Teacher that will be saved</param>
        /// <returns></returns>
        public static bool SaveTeacher(Teacher teacher)
        {
            if (!emailExists(teacher.email)) //makes sure the teacher hasnt already signed up with this email
            {
                string ID = NextID("T"); //generates the next ID for teacher
                teacher.ID = ID; //initialises the teachers ID in the class
                OleDbConnection conn = new OleDbConnection(ConStr);
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
        }
        /// <summary>
        /// Saves a student to the database
        /// </summary>
        /// <param name="student">The Student that will be saved</param>
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
        }
        /// <summary>
        /// Creates a class and saves it to the database
        /// </summary>
        /// <param name="className">The Name of the class</param>
        /// <param name="teacher">The class's Teacher</param>
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
        }
        /// <summary>
        /// Deletes a class from the database
        /// </summary>
        /// <param name="classID">ID of class to delete</param>
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
        }
        /// <summary>
        /// Returns True if the specified email is already present within the database
        /// </summary>
        /// <param name="email">The email the database will check for</param>
        /// <returns></returns>
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
        }
        /// <summary>
        /// Returns true if a specified teacher is already present within the database
        /// </summary>
        /// <param name="teacher">The teacher the database will check for</param>
        /// <returns></returns>
        public static bool TeacherAlreadySaved(string ID)
        {
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"SELECT * FROM Teacher WHERE TeacherID = '{ID}'";
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                conn.Close();
                return true;
            }
            conn.Close();
            return false;
        }
        /// <summary>
        /// Returns true if a specified student is already present within the database
        /// </summary>
        /// <param name="student">The student the database will check for</param>
        /// <returns></returns>
        public static bool StudentAlreadySaved(string ID)
        {
            
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"SELECT * FROM Student WHERE StudentID = '{ID}'";
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                conn.Close();
                return true;
            }
            conn.Close();
            return false;
        }
        public static bool ClassExists(string ID)
        {
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"SELECT * FROM Class WHERE ClassID = '{ID}'";
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                conn.Close();
                return true;
            }
            conn.Close();
            return false;
        }
        /// <summary>
        /// Returns the ID of a specified student using their email
        /// </summary>
        /// <param name="email">The email of student</param>
        /// <returns></returns>
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
        }
        /// <summary>
        /// Returns the ID of a specified teacher using their email
        /// </summary>
        /// <param name="email">The email of the teacher</param>
        /// <returns></returns>
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
        }
        /// <summary>
        /// Enrolls a specified student into a specified class, saving it to the database
        /// </summary>
        /// <param name="ClassID">ID of the class that the student will be added to</param>
        /// <param name="student">The student that will be added to the class</param>
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
        }
        /// <summary>
        /// Removes a specified student from a specified class
        /// </summary>
        /// <param name="ClassID">ID of the class the student will be removed from</param>
        /// <param name="student">The student that is being removed from the class</param>
        public static void RemoveStudent(string ClassID, Student student)
        {
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            //deletes the record 
            cmd.CommandText = $"DELETE FROM ClassEnrollment WHERE ClassID = '{ClassID}' AND StudentID = '{student.ID}'";
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// Returns true if a specified student is in a specified class
        /// </summary>
        /// <param name="ClassID">ID of the Class to check against</param>
        /// <param name="student">The student to check for</param>
        /// <returns></returns>
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
        }
        /// <summary>
        /// Returns a list of all the students in a class
        /// </summary>
        /// <param name="ClassID">The ID of the class</param>
        /// <returns></returns>
        public static List<Student> ListClass(string ClassID)
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
        /// <summary>
        /// Initialises a Student instance with a passed in StudentID
        /// </summary>
        /// <param name="StudentID">ID of the student</param>
        /// <returns></returns>
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
        }
        /// <summary>
        /// Initialises a Teacher instance with a passed in TeacherID
        /// </summary>
        /// <param name="TeacherID">ID of the teacher</param>
        /// <returns></returns>
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
        }
        /// <summary>
        /// Returns true if a teacher is currently logged in
        /// </summary>
        /// <returns></returns>
        public static bool TeacherIsLoggedIn()
        {
            return (loggedTeacher != null);
        }
        /// <summary>
        /// Returns true if a student is currently logged in
        /// </summary>
        /// <returns></returns>
        public static bool StudentIsLoggedIn()
        {
            return (loggedStudent != null);
        }
        /// <summary>
        /// Generates the next ID
        /// </summary>
        /// <param name="IDType">The Type of ID that is to be generated</param>
        /// <returns></returns>
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
        public static string GetClassName(string ClassID)
        {
            string ID = "";
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"SELECT ClassName FROM Class WHERE ClassID = '{ClassID}'";
            OleDbDataReader reader = cmd.ExecuteReader();
            try
            {
                ID = cmd.ExecuteScalar().ToString();
            }
            catch
            {
                MessageBox.Show("fail");
            }
            conn.Close();
            return ID;
        }
    }
}

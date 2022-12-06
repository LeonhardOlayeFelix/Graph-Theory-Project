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
using System.Threading;

namespace Interface_2
{
    public class Database
    {
        const string ConStr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=NetworkDB.accdb";
        public Database()
        {
            CreateDatabase();
        }
        private bool Authorised(string classID, Teacher loggedTeacher)
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
            reader.Close();
            return false;
        }
        public void CreateDatabase()
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
                cmd.CommandText = "CREATE TABLE Student(StudentID VARCHAR(5), FirstName VARCHAR(30), LastName VARCHAR(30), Alias VARCHAR(200), DateOfBirth DATE, Email VARCHAR(100), SPassword VARCHAR(30), NoAssignmentsSubmitted INTEGER, NoDijkstras INTEGER, NoFloyds INTEGER, NoRInsp INTEGER, NoBFS INTEGER, NoDFS INTEGER, NoPrims INTEGER, NoKruskals INTEGER, NoGraph INTEGER, DateCreated DATE, PRIMARY KEY(StudentID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE Teacher(TeacherID VARCHAR(5), FirstName VARCHAR(30), LastName VARCHAR(30), Alias VARCHAR(200), Email VARCHAR(100), TPassword VARCHAR(30), Title VARCHAR(7), PRIMARY KEY(TeacherID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE StudentGraph(Filename VARCHAR(200), StudentID VARCHAR(5), GraphName VARCHAR(100), DateCreated DATE, NoVertices INTEGER, NoEdges INTEGER, CreatedBy CHAR(1), PRIMARY KEY(Filename), FOREIGN KEY (StudentID) REFERENCES Student(StudentID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE TeacherGraph(Filename VARCHAR(200), TeacherID VARCHAR(5), GraphName VARCHAR(100), DateCreated DATE, NoVertices INTEGER, NoEdges INTEGER, CreatedBy CHAR(1), PRIMARY KEY(Filename), FOREIGN KEY (TeacherID) REFERENCES Teacher(TeacherID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE GuestGraph(Filename VARCHAR(200), GraphName VARCHAR(100), CreatedBy CHAR(1), PRIMARY KEY(Filename))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE Assignment(AssignmentID VARCHAR(5), StudentID VARCHAR(5), AssignmentNote VARCHAR(50), Alias VARCHAR(200), Filename VARCHAR(30), SetBy VARCHAR(5), GraphName VARCHAR(25), DateSet DATE, DateDue DATE, isLate CHAR(1), isCompleted CHAR(1), PRIMARY KEY(AssignmentID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE Class(ClassID VARCHAR(5), TeacherID VARCHAR(5), ClassName VARCHAR(30), Alias VARCHAR(200), PRIMARY KEY(ClassID), FOREIGN KEY (TeacherID) REFERENCES Teacher(TeacherID))";
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
        public bool SaveTeacher(Teacher teacher)
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
                cmd.CommandText = "INSERT INTO Teacher VALUES(?, ?, ?, ?, ?, ?, ?)";
                cmd.Parameters.Add(new OleDbParameter("TeacherID", ID));
                cmd.Parameters.Add(new OleDbParameter("FirstName", teacher.firstname));
                cmd.Parameters.Add(new OleDbParameter("LastName", teacher.lastname));
                cmd.Parameters.Add(new OleDbParameter("Alias", teacher.alias));
                cmd.Parameters.Add(new OleDbParameter("Email", teacher.email));
                cmd.Parameters.Add(new OleDbParameter("TPassword", teacher.password));
                cmd.Parameters.Add(new OleDbParameter("Title", teacher.title));
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
        public void SaveStudent(Student student)
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
                cmd.CommandText = $"INSERT INTO Student VALUES(?, ?, ?, ?, ?, ?, ?, '{0}', '{0}', '{0}', '{0}', '{0}'," +
                    $" '{0}', '{0}', '{0}', '{0}', '{DateTime.Today.ToString("dd/MM/yyyy")}')";
                cmd.Parameters.Add(new OleDbParameter("StudentID", ID));
                cmd.Parameters.Add(new OleDbParameter("FirstName", student.firstname));
                cmd.Parameters.Add(new OleDbParameter("LastName", student.lastname));
                cmd.Parameters.Add(new OleDbParameter("Alias", student.alias));
                cmd.Parameters.Add(new OleDbParameter("DateOfBirth", student.dob));
                cmd.Parameters.Add(new OleDbParameter("Email", student.email));
                cmd.Parameters.Add(new OleDbParameter("SPassword", student.password));
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        /// <summary>
        /// Creates a class and saves it to the database
        /// </summary>
        /// <param name="className">The Name of the class</param>
        /// <param name="teacher">The class's Teacher</param>
        public void CreateClass(string className, Teacher teacher)
        {
            string classID = NextID("C"); //generates the class ID 
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            OleDbCommand cmd = new OleDbCommand();
            string classAlias = classID + ": " + className;
            cmd.Connection = conn;
            conn.Open();
            //inserts the new class into the database
            cmd.CommandText = $"INSERT INTO Class VALUES(?, ?, ?, ?)";
            cmd.Parameters.Add(new OleDbParameter("ClassID", classID));
            cmd.Parameters.Add(new OleDbParameter("TeacherID", teacher.ID));
            cmd.Parameters.Add(new OleDbParameter("ClassName", className));
            cmd.Parameters.Add(new OleDbParameter("Alias", classAlias));
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
        public bool emailExists(string email)
        {
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            OleDbCommand cmd2 = new OleDbCommand();
            cmd2.Connection = conn;
            //check in both the student and teacher tables
            cmd.CommandText = $"SELECT * FROM Student WHERE Email = ?";
            cmd.Parameters.Add(new OleDbParameter("Email", email));
            cmd2.CommandText = $"SELECT * FROM Teacher WHERE Email = ?";
            cmd2.Parameters.Add(new OleDbParameter("Email", email));
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
            reader.Close();
            reader2.Close();
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
            reader.Close();
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
            reader.Close();
            return false;
        }
        public bool ClassExists(string ID)
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
        public string GetStudentID(string email)
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
        public string GetTeacherID(string email)
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
        public void EnrollStudent(string ClassID, Student student)
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
        public void RemoveStudent(string ClassID, Student student)
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
        public bool IsInClass(string ClassID, Student student)
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
            reader.Close();
            conn.Close();
            return false;
        }
        /// <summary>
        /// Returns a list of all the students in a class
        /// </summary>
        /// <param name="ClassID">The ID of the class</param>
        /// <returns></returns>
        public List<Student> ListClass(string ClassID)
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
            reader.Close();
            return Ids;
        }
        /// <summary>
        /// Initialises a Student instance with a passed in StudentID
        /// </summary>
        /// <param name="StudentID">ID of the student</param>
        /// <returns></returns>
        public Student InitialiseStudent(string StudentID)
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
            reader.Close();
            return null;
        }
        /// <summary>
        /// Initialises a Teacher instance with a passed in TeacherID
        /// </summary>
        /// <param name="TeacherID">ID of the teacher</param>
        /// <returns></returns>
        public  Teacher InitialiseTeacher(string TeacherID)
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
            reader.Close();
            conn.Close();
            return null;
        }
        /// <summary>
        /// Returns true if a teacher is currently logged in
        /// </summary>
        /// <returns></returns>
        public bool TeacherIsLoggedIn(Teacher loggedTeacher)
        {
            return (loggedTeacher != null);
        }
        /// <summary>
        /// Returns true if a student is currently logged in
        /// </summary>
        /// <returns></returns>
        public bool StudentIsLoggedIn(Student loggedStudent)
        {
            return (loggedStudent != null);
        }
        /// <summary>
        /// Generates the next ID
        /// </summary>
        /// <param name="IDType">The Type of ID that is to be generated</param>
        /// <returns></returns>
        public string NextID(string IDType)
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
            else if (IDType == "A")
            {
                cmd.CommandText = "SELECT MAX(AssignmentID) AS MaxID FROM Assignment";
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
        public string GetClassName(string ClassID)
        {
            string ID = "";
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"SELECT * FROM Class WHERE ClassID = '{ClassID}'";
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                ID = (string)reader["ClassName"];
            }
            reader.Close();
            conn.Close();
            return ID;
        }
        public void SetAssignment(string ClassID, string assingmentNote, Teacher loggedTeacher, Graph graph)
        {
            string assignmentID = NextID("A");
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            OleDbCommand cmd = new OleDbCommand();
            conn.Open();
            cmd.Connection = conn;
            FileStream fs;
            List<Student> studentsInClass = ListClass(ClassID);
            string alias = loggedTeacher.title + " " + loggedTeacher.lastname + ": " + assingmentNote;
            foreach (Student student in studentsInClass)
            {
                string filename = "AssignmentGraphs/" + assignmentID + student.ID;
                fs = File.Create(filename);
                cmd.CommandText = $"INSERT INTO Assignment VALUES('{assignmentID}','{student.ID}','{assingmentNote}','{alias}','{filename}','{loggedTeacher.ID}','{graph.Name}','{DateTime.Today.ToString("dd/MM/yyyy")}', '{DateTime.Today.ToString("dd/MM/yyyy")}','{"n"}', '{"n"}')";
                cmd.ExecuteNonQuery();
                int NextID = Convert.ToInt32(assignmentID.Substring(1)) + 1;
                assignmentID = "0000" + NextID.ToString();
                assignmentID = assignmentID.Substring(assignmentID.Length - 4);
                assignmentID = assignmentID.Insert(0, "A");
                fs.Close();
                //write the class instance to the database
                BinarySerialization.Write(filename, graph, false);
            }
            MessageBox.Show("This graph has been set as an assignment for: " + GetClassName(ClassID) + "(" + ClassID + ")");
        }
        public void IncrementStudentField(string ID, string field)
        {
            if (!StudentAlreadySaved(ID))
            {
                throw new Exception("Invalid student ID was passed in to IncrementStudentField function");
            }
            string[] fields = new string[] { "NoAssignmentsSubmitted", "NoDijkstras", "NoRInsp", "NoBFS", "NoDFS", "NoPrims", "NoGraph", "NoKruskals", "NoFloyds" };
            if (!fields.Contains(field))
            {
                throw new Exception("Invalid Field was passed in to IncrementStudentField function.");
            }
            OleDbConnection conn = new OleDbConnection(MainWindow.ConStr);
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            conn.Open();
            cmd.CommandText = $"SELECT {field} FROM Student WHERE StudentID = '{ID}'";
            //OleDbDataReader reader = cmd.ExecuteReader();
            //reader.Read();
            //int newFieldValue = Convert.ToInt32(reader[0]);
            int newFieldValue = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
            cmd.CommandText = $"UPDATE Student SET {field} = {newFieldValue} WHERE StudentID = '{ID}'";
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}

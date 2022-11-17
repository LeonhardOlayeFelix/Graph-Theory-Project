using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    public class User
    {
        //Both students and teachers have the following attributes
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string ID { get; set; }
        public string alias { get; set; }
        public User(string Firstname, string Lastname, string Email, string Password) //cosntructor to initialise
        {
            firstname = Firstname;
            lastname = Lastname;
            email = Email;
            password = Password;
        }
    }
    public class Teacher : User //inherits from user
    {
        //Only teachers have the following attributes
        public string title { get; set; }
        public Teacher(string Firstname, string Lastname, string Email, string Password, string Title, string Id = "") //constructor to initialise
            : base(Firstname, Lastname, Email, Password) //pass to base
        {
            ID = Id;
            title = Title;
            alias = ID + ": " + firstname + " " + lastname;
        }
    }
    public class Student : User //inherits from user
    {
        public DateTime dob { get; set; } //Only Students have the following attributes
        public Student(string Firstname, string Lastname, string Email, string Password, DateTime Dob, string Id = "") //constructor to initialise
            : base(Firstname, Lastname, Email, Password) //pass to base
        {
            ID = Id;
            dob = Dob;
            alias = ID + ": " + firstname + " " + lastname;
        }

    }
}

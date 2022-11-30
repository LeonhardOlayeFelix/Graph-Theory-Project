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
using System.Text.RegularExpressions;
using System.IO;

namespace Interface_2
{
    public static class BinarySerialization //this class will read and write an object into a binary file
    {
        public static void Write<T>(string path, T obj, bool append = false) //takes in type T which is the type of the object
        {
            using (Stream stream = File.Open(path, append ? FileMode.Append : FileMode.Create))
            {
                var Writer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter(); 
                Writer.Serialize(stream, obj);//writes the object to the binary file 
            }   
        }
        public static T Read<T>(string path) //needs the file to read and type 
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                var reader = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)reader.Deserialize(stream); //writes the object to the binary file through deserialization
            }
        }
    }
}

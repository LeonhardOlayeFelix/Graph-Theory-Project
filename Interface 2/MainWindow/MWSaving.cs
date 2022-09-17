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
    public static class BinarySerialization //this class will read and write a given object into a binary file
    {
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false) //takes in type T which is the type of the object
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var BinaryWriter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter(); 
                BinaryWriter.Serialize(stream, objectToWrite);//writes the object to the binary file 
            }   
        }
        public static T ReadFromBinaryFile<T>(string filePath) //needs the file to read and type 
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var BinaryReader = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)BinaryReader.Deserialize(stream); //writes the object to the binary file through deserialization
            }
        }
    }
}

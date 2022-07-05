using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2
{
    [Serializable]

    public class MyPoint
    {
        public double X { get; set; } // x coordinate of this point
        public double Y { get; set; } // y coordinate of this point
        public double originalX { get; } //the x coordinate that this point started with
        public double originalY { get; } //the y coordinate that this point started with
        public MyPoint(double x, double y)//constructor 
        {
            X = x;
            Y = y;
            originalX = x;
            originalY = y;
        }
        public void SetPosition(double x, double y) //sets the position of a point
        {
            X = x;
            Y = y;
        }
        public MyPoint GetPosition() //returns the point as a class instance
        {
            return new MyPoint(X, Y);
        }
        public Tuple<double, double> GetPositionTuple() //returns position as tuple
        {
            return Tuple.Create(X, Y);
        }
        public MyPoint GetOriginalPosition() //returns original position
        {
            return new MyPoint(originalX, originalY);
        }
        public Tuple<double, double> GetOriginalPositionTuple() //returns original position as tuple
        {
            return Tuple.Create(originalX, originalY);
        }
        public double GetOriginalX() //returns original x coord
        {
            return originalX;
        }
        public double GetOriginalY()
        {
            return originalY;
        }
    }
}

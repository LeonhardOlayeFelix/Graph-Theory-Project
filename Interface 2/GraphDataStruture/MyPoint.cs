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
        //declare attributes
        public double X { get; set; }
        public double Y { get; set; }
        public double originalX { get; }
        public double originalY { get; }
        public MyPoint(double x, double y)
        {
            //initialise attributes
            X = x;
            Y = y;
            originalX = x;
            originalY = y;
        }
        public void SetPosition(double x, double y)
        {
            //updates position with arguments
            X = x;
            Y = y;
        }
        public MyPoint GetPosition()
        {
            //returns position as class
            return new MyPoint(X, Y);
        }
        public Tuple<double, double> GetPositionTuple()
        {
            //returns position as tuple
            return Tuple.Create(X, Y);
        }
        public MyPoint GetOriginalPosition()
        {
            //returns original position as class
            return new MyPoint(originalX, originalY);
        }
        public Tuple<double, double> GetOriginalPositionTuple()
        {
            //returns original position as tuple
            return Tuple.Create(originalX, originalY);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_2.GraphDataStruture
{
    class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double originalX { get; }
        public double originalY { get; }
        public Point(double x, double y)
        {
            X = x;
            Y = y;
            originalX = x;
            originalY = y;
        }
        public void SetPosition(double x, double y)
        {
            X = x;
            Y = y;
        }
        public Point GetPosition()
        {
            return new Point(X, Y);
        }
        public Tuple<double, double> GetPositionTuple()
        {
            return Tuple.Create(X, Y);
        }
        public Point GetOriginalPosition()
        {
            return new Point(originalX, originalY);
        }
        public Tuple<double, double> GetOriginalPositionTuple()
        {
            return Tuple.Create(originalX, originalY);
        }
        public double GetOriginalX()
        {
            return originalX;
        }
        public double GetOriginalY()
        {
            return originalY;
        }
    }
}

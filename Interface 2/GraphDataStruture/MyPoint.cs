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
        public double X { get; set; }
        public double Y { get; set; }
        /// <summary>
        /// the x coordinate that this point was initialised with before it was potentially changed
        /// </summary>
        public double originalX { get; }
        /// <summary>
        /// the y coordinate that this point was initialised with before it was potentially changed 
        /// </summary>
        public double originalY { get; }
        public MyPoint(double x, double y)
        {
            X = x;
            Y = y;
            originalX = x;
            originalY = y;
        }
        /// <summary>
        /// updates the position
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        public void SetPosition(double x, double y)
        {
            //updates position with arguments
            X = x;
            Y = y;
        }
        /// <summary>
        /// returns the position as a tuple(x coord, y coord)
        /// </summary>
        /// <returns></returns>
        public Tuple<double, double> GetPositionTuple()
        {
            return Tuple.Create(X, Y);
        }
        public MyPoint GetOriginalPosition()
        {
            //returns original position as class
            return new MyPoint(originalX, originalY);
        }
        /// <summary>
        /// returns the original position as a tuple(x coord, y coord)
        /// </summary>
        /// <returns></returns>
        public Tuple<double, double> GetOriginalPositionTuple()
        {
            return Tuple.Create(originalX, originalY);
        }
    }
}

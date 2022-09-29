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
using System.Threading;
namespace Interface_2
{
    public partial class MainWindow : Window
    {
        private void mainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Ellipse) //if they just click on an ellipse / vertex 
            {
                timer1.Stop();
                timer.Stop();
                Ellipse activeVertex = (Ellipse)e.OriginalSource;
                leftClickVertexOperation(activeVertex);
            }
            else if (e.OriginalSource is Line) //if they click on a line in the canvas
            {
                Line activeLine = (Line)e.OriginalSource;
                leftClickLineOperation(activeLine);
            }  
            else //if they just click on the canvas
            {
                if (buttonSelectionCount % 2 == 1 && currentButton == btnAddConnection) //if they pressed the canvas to try and cancel an add connection 
                {
                    DecrementSelectionCount(ref buttonSelectionCount);
                }
                else if (rInspSelectionCount % 2 == 1 && currentButton == btnRouteInspStartAndEnd)//if they pressed the canvas to try and cancel a route inspection
                {
                    DecrementSelectionCount(ref rInspSelectionCount);
                }
                else if (dijkstraSelectionCount % 2 == 1 && currentButton == btnDijkstrasShort)//if they pressed the canvas to try and cancel a dijkstras algorithm
                {
                    DecrementSelectionCount(ref dijkstraSelectionCount);
                }
                else
                {
                    leftClickCanvasOperation();
                }
            }
        }
        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown && currentButton == btnDefault)
            {
                // When the mouse is held down, reposition the drag selection box.

                Point mousePos = e.GetPosition(mainCanvas);
                if (mouseDownPos.X < mousePos.X)
                {
                    Canvas.SetLeft(selectionBox, mouseDownPos.X);
                    selectionBox.Width = mousePos.X - mouseDownPos.X;
                }
                else
                {
                    Canvas.SetLeft(selectionBox, mousePos.X);
                    selectionBox.Width = mouseDownPos.X - mousePos.X;
                }
                if (mouseDownPos.Y < mousePos.Y)
                {
                    Canvas.SetTop(selectionBox, mouseDownPos.Y);
                    selectionBox.Height = mousePos.Y - mouseDownPos.Y;
                }
                else
                {
                    Canvas.SetTop(selectionBox, mousePos.Y);
                    selectionBox.Height = mouseDownPos.Y - mousePos.Y;
                }
            }
        }
        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            leftMouseButtonUpOperation();
        }
    }
}

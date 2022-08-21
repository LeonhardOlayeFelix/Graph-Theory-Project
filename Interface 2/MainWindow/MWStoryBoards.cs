using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Timers;

namespace Interface_2
{

    public partial class MainWindow : Window
    {
        Timer timer = new Timer();
        Timer timer1 = new Timer();
        public void InitiateVertexStoryboard(double newDiameter, TimeSpan duration, Ellipse vertex)
        {
            //initialise animation
            DoubleAnimation animation = new DoubleAnimation(newDiameter, duration);
            animation.FillBehavior = FillBehavior.Stop; //allows the object to be editted after animation is complete
            animation.Completed += (sender, e) =>
            {
                Binding bindingDiameter = new Binding("Value")//binding the diameter of the vertices to the slider
                {
                    Source = vertexDiameterSlider,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                };
                vertex.SetBinding(Ellipse.HeightProperty, bindingDiameter);
                vertex.SetBinding(Ellipse.WidthProperty, bindingDiameter);
            };
            //being animation
            vertex.BeginAnimation(Ellipse.HeightProperty, animation);
            vertex.BeginAnimation(Ellipse.WidthProperty, animation);
        }
        public void InitiateLineStoryboard(Line line, TimeSpan duration, List<int> order)
        {
            //make a copy of the line
            Line line1 = new Line();
            mainCanvas.Children.Add(line1);
            line1.Stroke = line.Stroke;
            line1.StrokeThickness = line.StrokeThickness;
            Ellipse fromVertex = FindEllipse(order[0]);
            Ellipse toVertex = FindEllipse(order[1]);
            line1.X1 = Canvas.GetLeft(fromVertex);
            line1.Y1 = Canvas.GetTop(fromVertex);
            line1.X2 = Canvas.GetLeft(toVertex);
            line1.Y2 = Canvas.GetTop(toVertex);

            //initialise the storyboard
            Storyboard sb = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation(line1.Y1, line1.Y2, duration);
            DoubleAnimation animation1 = new DoubleAnimation(line1.X1, line1.X2, duration);
            animation.FillBehavior = FillBehavior.Stop;
            animation1.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTargetProperty(animation, new PropertyPath("(Line.Y2)"));
            Storyboard.SetTargetProperty(animation1, new PropertyPath("(Line.X2)"));
            sb.Children.Add(animation);
            sb.Children.Add(animation1);

            //add a completion event handler for animation
            animation1.Completed += (sender, e) => 
            {
                //once done animation, remove the copy and add the actualy line to canvas
                mainCanvas.Children.Add(line);
                mainCanvas.Children.Remove(line1);
            };
            
            //begin storyboard
            line1.BeginStoryboard(sb);
        }
        public void InitiateDeleteLineStoryboard(Line line, TimeSpan duration)
        {
            //make a copy of the line
            Line line1 = new Line();
            mainCanvas.Children.Add(line1);
            line1.Stroke = line.Stroke;
            line1.StrokeThickness = line.StrokeThickness;
            line1.X1 = line.X1;
            line1.Y1 = line.Y1;
            line1.X2 = line.X2;
            line1.Y2 = line.Y2;

            //initialise storyboard
            Storyboard sb = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation(line1.Y2, line1.Y1, duration);
            DoubleAnimation animation1 = new DoubleAnimation(line1.X2, line1.X1, duration);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(Line.Y2)"));
            Storyboard.SetTargetProperty(animation1, new PropertyPath("(Line.X2)"));
            sb.Children.Add(animation);
            sb.Children.Add(animation1);

            //add a completion event handler for animation
            animation1.Completed += (sender, e) =>
            {
                mainCanvas.Children.Remove(line);
            };
            
            //begin the animation
            line1.BeginStoryboard(sb);
        }
        public void InitiateDeleteVertexStoryboard(Ellipse vertex, TimeSpan duration)
        {
            //initialise animation
            DoubleAnimation animation = new DoubleAnimation(0, duration);
            vertex.BeginAnimation(Ellipse.HeightProperty, animation);
            vertex.BeginAnimation(Ellipse.WidthProperty, animation);

            //add a completion event handler for animation
            animation.Completed += (sender, e) =>
            {
                mainCanvas.Children.Remove(vertex);
            };
        }
        public void InitiateHighlightLineStoryboard(Line line, TimeSpan duration)
        {
            //make a copy of the line
            Line line1 = new Line();
            mainCanvas.Children.Add(line1);
            line1.Stroke = HighlightColour;
            line1.StrokeThickness = line.StrokeThickness;
            line1.X1 = line.X1;
            line1.Y1 = line.Y1;
            line1.X2 = line.X2;
            line1.Y2 = line.Y2;
            //initialise storyboard

            Storyboard sb = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation(line1.Y1, line1.Y2, duration);
            DoubleAnimation animation1 = new DoubleAnimation(line1.X1, line1.X2, duration);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(Line.Y2)"));
            Storyboard.SetTargetProperty(animation1, new PropertyPath("(Line.X2)"));
            sb.Children.Add(animation);
            sb.Children.Add(animation1);
            linesToDelete.Add(line1);

            //start storyboard
            line1.BeginStoryboard(sb);
        }
        public void InitiateHighlightPathStoryboard(List<int> path, TimeSpan duration)
        {
            for (int i = 0; i < path.Count - 1; ++i)
            {
                //identify the edge using the vertices on both ends
                Ellipse fromVertex = FindEllipse(path[i]);
                Ellipse toVertex = FindEllipse(path[i + 1]);

                //initialise a line
                Line line1 = new Line();
                mainCanvas.Children.Add(line1);
                line1.StrokeThickness = 4;
                line1.Stroke = HighlightColour;
                line1.X1 = Canvas.GetLeft(fromVertex);
                line1.Y1 = Canvas.GetTop(fromVertex);
                line1.X2 = Canvas.GetLeft(toVertex);
                line1.Y2 = Canvas.GetTop(toVertex);

                //initialise the storyboard
                Storyboard sb = new Storyboard();
                DoubleAnimation animation = new DoubleAnimation(line1.Y1, line1.Y2, duration);
                DoubleAnimation animation1 = new DoubleAnimation(line1.X1, line1.X2, duration);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Line.Y2)"));
                Storyboard.SetTargetProperty(animation1, new PropertyPath("(Line.X2)"));
                sb.Children.Add(animation);
                sb.Children.Add(animation1);
                linesToDelete.Add(line1);

                //begin storyboard
                line1.BeginStoryboard(sb);
            }
        }
        public void InitiatePathWalkerStoryboard(List<int> path, bool recursed = false)
        {
            //initialise pathwalker
            Ellipse pathWalker = new Ellipse
            {
                Fill = HighlightColour,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 2,
                Margin = new Thickness(-100000),
                Height = 25,
                Width = 25,
            };
            int pathWalkerDuration = Convert.ToInt32(pathWalkerDurationSlider.Value);
            int count = 0;
            int totalTime = 1000 * pathWalkerDuration * (path.Count); //time taken for pathwalker to complete journey
            timer = new Timer() { Interval = 1000 * pathWalkerDuration};
            timer1 = new Timer() { Interval = totalTime };
            timer1.Elapsed += (sender, e) =>
            {
                timer.Stop(); 
                timer1.Stop();
                this.Dispatcher.Invoke(() =>
                {
                    InitiatePathWalkerStoryboard(path);
                });
            };
            timer.Elapsed += (sender, e) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    //identify the edge using the vertices on both ends
                    mainCanvas.Children.Add(pathWalker);
                    int from = path[count];
                    int to = path[count + 1];
                    Ellipse fromVertex = FindEllipse(from);
                    Ellipse toVertex = FindEllipse(to);

                    //initialise animation and story board
                    Storyboard sb = new Storyboard();
                    DoubleAnimation animation = new DoubleAnimation(Canvas.GetTop(fromVertex), Canvas.GetTop(toVertex), TimeSpan.FromSeconds(pathWalkerDuration));
                    DoubleAnimation animation1 = new DoubleAnimation(Canvas.GetLeft(fromVertex), Canvas.GetLeft(toVertex), TimeSpan.FromSeconds(pathWalkerDuration));
                    animation.FillBehavior = FillBehavior.Stop;
                    animation1.FillBehavior = FillBehavior.Stop;
                    animation.Completed += (s, ew) =>
                    {
                        mainCanvas.Children.Remove(pathWalker);
                    };
                    Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.TopProperty));
                    Storyboard.SetTargetProperty(animation1, new PropertyPath(Canvas.LeftProperty));
                    sb.Children.Add(animation);
                    sb.Children.Add(animation1);

                    //begin animation
                    pathWalker.BeginStoryboard(sb);
                    count++;
                });
            };
            timer.Start();
            timer1.Start();
            
        }
    }
}

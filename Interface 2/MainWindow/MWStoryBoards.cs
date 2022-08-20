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
        public void InitiateVertexStoryboard(double newDiameter, TimeSpan duration, Ellipse vertex)
        {
            DoubleAnimation animation = new DoubleAnimation(newDiameter, duration);
            animation.FillBehavior = FillBehavior.Stop;
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
            vertex.BeginAnimation(Ellipse.HeightProperty, animation);
            vertex.BeginAnimation(Ellipse.WidthProperty, animation);
        }
        public void InitiateLineStoryboard(Line line, TimeSpan duration, List<int> order)
        {
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
            Storyboard sb = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation(line1.Y1, line1.Y2, duration);
            DoubleAnimation animation1 = new DoubleAnimation(line1.X1, line1.X2, duration);
            animation.FillBehavior = FillBehavior.Stop;
            animation1.FillBehavior = FillBehavior.Stop;
            animation1.Completed += (sender, e) =>
            {
                mainCanvas.Children.Add(line);
                mainCanvas.Children.Remove(line1);
            };
            Storyboard.SetTargetProperty(animation, new PropertyPath("(Line.Y2)"));
            Storyboard.SetTargetProperty(animation1, new PropertyPath("(Line.X2)"));
            sb.Children.Add(animation);
            sb.Children.Add(animation1);
            line1.BeginStoryboard(sb);
        }
        public void InitiateDeleteLineStoryboard(Line line, TimeSpan duration)
        {
            Line line1 = new Line();
            mainCanvas.Children.Add(line1);
            line1.Stroke = line.Stroke;
            line1.StrokeThickness = line.StrokeThickness;
            line1.X1 = line.X1;
            line1.Y1 = line.Y1;
            line1.X2 = line.X2;
            line1.Y2 = line.Y2;
            Storyboard sb = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation(line1.Y2, line1.Y1, duration);
            DoubleAnimation animation1 = new DoubleAnimation(line1.X2, line1.X1, duration);
            animation1.Completed += (sender, e) =>
            {
                mainCanvas.Children.Remove(line);
            };
            Storyboard.SetTargetProperty(animation, new PropertyPath("(Line.Y2)"));
            Storyboard.SetTargetProperty(animation1, new PropertyPath("(Line.X2)"));
            sb.Children.Add(animation);
            sb.Children.Add(animation1);
            line1.BeginStoryboard(sb);
        }
        public void InitiateDeleteVertexStoryboard(Ellipse vertex, TimeSpan duration)
        {
            DoubleAnimation animation = new DoubleAnimation(0, duration);
            //animation.FillBehavior = FillBehavior.Stop;
            vertex.BeginAnimation(Ellipse.HeightProperty, animation);
            vertex.BeginAnimation(Ellipse.WidthProperty, animation);
            animation.Completed += (sender, e) =>
            {
                mainCanvas.Children.Remove(vertex);
            };
        }
        public void InitiateHighlightLineStoryboard(Line line, TimeSpan duration)
        {
            Line line1 = new Line();
            mainCanvas.Children.Add(line1);
            line1.Stroke = HighlightColour;
            line1.StrokeThickness = line.StrokeThickness;
            line1.X1 = line.X1;
            line1.Y1 = line.Y1;
            line1.X2 = line.X2;
            line1.Y2 = line.Y2;
            Storyboard sb = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation(line1.Y1, line1.Y2, duration);
            DoubleAnimation animation1 = new DoubleAnimation(line1.X1, line1.X2, duration);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(Line.Y2)"));
            Storyboard.SetTargetProperty(animation1, new PropertyPath("(Line.X2)"));
            sb.Children.Add(animation);
            sb.Children.Add(animation1);
            linesToDelete.Add(line1);
            line1.BeginStoryboard(sb);
        }
        public void InitiateHighlightPathStoryboard(List<int> path, TimeSpan duration)
        {
            for (int i = 0; i < path.Count - 1; ++i)
            {
                Ellipse fromVertex = FindEllipse(path[i]);
                Ellipse toVertex = FindEllipse(path[i + 1]);
                Line line1 = new Line();
                mainCanvas.Children.Add(line1);
                line1.StrokeThickness = 4;
                line1.Stroke = HighlightColour;
                line1.X1 = Canvas.GetLeft(fromVertex);
                line1.Y1 = Canvas.GetTop(fromVertex);
                line1.X2 = Canvas.GetLeft(toVertex);
                line1.Y2 = Canvas.GetTop(toVertex);
                Storyboard sb = new Storyboard();
                DoubleAnimation animation = new DoubleAnimation(line1.Y1, line1.Y2, duration);
                DoubleAnimation animation1 = new DoubleAnimation(line1.X1, line1.X2, duration);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Line.Y2)"));
                Storyboard.SetTargetProperty(animation1, new PropertyPath("(Line.X2)"));
                sb.Children.Add(animation);
                sb.Children.Add(animation1);
                linesToDelete.Add(line1);
                line1.BeginStoryboard(sb);
            }
        }
        public void InitiatePathWalkerStoryboard(List<int> path)
        {

            Ellipse pathWalker = new Ellipse
            {
                Fill = new SolidColorBrush(Colors.Red),
                Stroke = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(-100000),
                Height = 25,
                Width = 25,
            };
            int count = 0;
            int msNeeded = 1000 * (path.Count);

            Timer timer = new Timer() { Interval = 1000 };
            Timer timer1 = new Timer() { Interval = msNeeded };
            timer1.Elapsed += (sender, e) => { timer.Stop(); timer1.Stop(); };
            timer.Elapsed += (sender, e) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    mainCanvas.Children.Add(pathWalker);
                    int from = path[count];
                    int to = path[count + 1];
                    Ellipse fromVertex = FindEllipse(from);
                    Ellipse toVertex = FindEllipse(to);
                    Storyboard sb = new Storyboard();
                    //set up pathwalker animation
                    DoubleAnimation animation = new DoubleAnimation(Canvas.GetTop(fromVertex), Canvas.GetTop(toVertex), TimeSpan.FromSeconds(1));
                    DoubleAnimation animation1 = new DoubleAnimation(Canvas.GetLeft(fromVertex), Canvas.GetLeft(toVertex), TimeSpan.FromSeconds(1));
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
                    pathWalker.BeginStoryboard(sb);
                    count++;
                });
            };
            timer.Start();
            timer1.Start();
        }
    }
}

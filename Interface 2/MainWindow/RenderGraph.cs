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

namespace Interface_2
{

    public partial class MainWindow : Window
    {
        public void RenderGraph(Network GraphToRender, string name) //renders a graph onto the canvas
        {
            DeleteGraph();
            CreateNewGraph(name);
            Graph = GraphToRender;
            RenderVertices(GraphToRender);
            RenderEdges(GraphToRender);
        }
        private void RenderVertices(Network GraphToRender)
        {
            buttonId = GraphToRender.GetMaxNodeID() + 1;
            foreach (int vertex in Graph.GetListOfVertices())
            {
                Ellipse vertexToAdd = new Ellipse() { StrokeThickness = 2 }; //create the vertex that will be added
                Binding bindingStroke = new Binding("SelectedBrush") //binding the stroke of the vertices to the color picker
                {
                    Source = colourPickerVertexStroke,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.OneWay
                };
                vertexToAdd.SetBinding(Ellipse.StrokeProperty, bindingStroke);

                Binding bindingFill = new Binding("SelectedBrush")//binding the fill colour of the vertices to the color picker
                {
                    Source = colourPickerVertex,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.OneWay
                };
                vertexToAdd.SetBinding(Ellipse.FillProperty, bindingFill);

                Binding bindingDiameter = new Binding("Value")//binding the diameter of the vertices to the slider
                {
                    Source = vertexDiameterSlider,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                };
                vertexToAdd.SetBinding(Ellipse.HeightProperty, bindingDiameter);
                vertexToAdd.SetBinding(Ellipse.WidthProperty, bindingDiameter);

                //positioning the vertex in the canvas.
                double vertexCenterXMousePos = GraphToRender.GetVertex(Convert.ToInt32(vertex)).Position.X;
                double vertexCenterYMousePos = GraphToRender.GetVertex(Convert.ToInt32(vertex)).Position.Y;
                vertexToAdd.Margin = new Thickness(-100000); //margin of 100000 so that it resizes around the center.
                Canvas.SetLeft(vertexToAdd, vertexCenterXMousePos);
                Canvas.SetTop(vertexToAdd, vertexCenterYMousePos);
                Canvas.SetZIndex(vertexToAdd, Zindex++);

                vertexList.Add(vertexToAdd);//add the vertex to the list
                vertexToAdd.MouseMove += mouseMove;//give the buttons drag and drop event handlers

                //give the string a Name in the form btn(vertexId)
                string vertexId = vertex.ToString();
                vertexToAdd.Name = "btn" + vertexId;
                mainCanvas.Children.Add(vertexToAdd);//add the vertex to the canvas
                TextBlock vertexLabel = new TextBlock()//label for the ID of the vertex
                {
                    FontSize = 15,
                    Foreground = new SolidColorBrush(Colors.Black),
                    Name = "labelFor" + vertexId,
                    IsHitTestVisible = false //makes it so that the mouse clicks THROUGH the text block, and onto the ellipse
                };
                if ((bool)cbAlphabet.IsChecked)
                {
                    vertexLabel.Text = alphabet.ElementAt(Convert.ToInt32(vertexId));
                }
                else
                {
                    vertexLabel.Text = vertexId;
                }
                Binding bindingBG = new Binding("SelectedBrush")//binding the fill of the textblock to the colour picker
                {
                    Source = colourPickerLabel,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.OneWay
                };
                vertexLabel.SetBinding(TextBlock.ForegroundProperty, bindingBG);

                //set its position ontop of the vertex, and at its center, depending on the number of digits
                if (vertexLabel.Text.Length == 1)
                {
                    Canvas.SetTop(vertexLabel, Canvas.GetTop(vertexToAdd) - 9);
                    Canvas.SetLeft(vertexLabel, Canvas.GetLeft(vertexToAdd) - 4);
                }
                else if (vertexLabel.Text.Length == 2)
                {
                    Canvas.SetTop(vertexLabel, Canvas.GetTop(vertexToAdd) - 9);
                    Canvas.SetLeft(vertexLabel, Canvas.GetLeft(vertexToAdd) - 9);
                }
                else
                {
                    Canvas.SetTop(vertexLabel, Canvas.GetTop(vertexToAdd) - 9);
                    Canvas.SetLeft(vertexLabel, Canvas.GetLeft(vertexToAdd) - 13);
                }
                Canvas.SetZIndex(vertexLabel, Zindex++);

                vertexTxBoxList.Add(vertexLabel);//add it to the label list

                mainCanvas.Children.Add(vertexLabel); //add the label to the canvas
            }
        } 
        private void RenderEdges(Network GraphToRender)
        {
            foreach (Node vertex in GraphToRender.GetAdjacencyList())
            {
                foreach (Tuple<int, int> connection in vertex.GetAdjVertices())
                {
                    ConnectVertices(FindEllipse(vertex.GetVertexId()), FindEllipse(connection.Item1), connection.Item2, true);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Interface_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Ellipse lastSelectedVertex;
        Ellipse vertexToConnectTo;
        int buttonSelectionCount = 0;
        private bool graphCreated = false;
        private Button currentButton = null;
        private Color btnActivatedColour = Color.FromRgb(190, 230, 253);
        int buttonId = 0;
        List<Ellipse> vertexList = null;
        List<TextBlock> vertexTxBoxList = null;
        HashSet<Line> edgeList = null;
        Ellipse ellipseToDrop = null;
        private AdjacencySetGraph Graph = null;
        public MainWindow()
        {
            InitializeComponent();
            DisableAllActionButtons();
        }
        public void CreateNewGraph(string graphName) 
        {
            edgeList = new HashSet<Line>();
            Graph = new AdjacencySetGraph();
            Graph.Name = graphName;
            vertexTxBoxList = new List<TextBlock>();
            vertexList = new List<Ellipse>();
            graphCreated = true;
            labelGraphName.Content = graphName;
            EnableAllActionButtons();
        }
        public void DisableTabControlsNotAction()
        {
            tabControlAlgorithms.IsEnabled = false;
            tabControlDisplay.IsEnabled = false;
            tabControlHelp.IsEnabled = false;
            tabControlLogs.IsEnabled = false;
        }
        public void EnableTabControlsNotAction()
        {
            tabControlAlgorithms.IsEnabled = true;
            tabControlDisplay.IsEnabled = true;
            tabControlHelp.IsEnabled = true;
            tabControlLogs.IsEnabled = true;
        }
        public void DisableAllActionButtons()
        {
            btnAddVertex.IsEnabled = false;
            btnAddConnection.IsEnabled = false;
            btnDeleteConnection.IsEnabled = false;
            btnDeleteVertex.IsEnabled = false;
            btnTakeScreenshot.IsEnabled = false;
            btnGenerateAdjList.IsEnabled = false;
            btnGenerateAdjMatrix.IsEnabled = false;
            btnDragAndDrop.IsEnabled = false;
            btnDefault.IsEnabled = false;
        }
        public void EnableAllActionButtons()
        {
            btnAddVertex.IsEnabled = true;
            btnAddConnection.IsEnabled = true;
            btnDeleteConnection.IsEnabled = true;
            btnDeleteVertex.IsEnabled = true;
            btnTakeScreenshot.IsEnabled = true;
            btnGenerateAdjList.IsEnabled = true;
            btnGenerateAdjMatrix.IsEnabled = true;
            btnDragAndDrop.IsEnabled = true;
            btnDefault.IsEnabled = true;
        }
        private void ActivateButton(object btnSender)
        {
            if (btnSender != null )
            {
                if (currentButton != (Button)btnSender && currentButton != btnSender as Button)
                {
                    DeactivateButton();
                    currentButton = (Button)btnSender;
                    currentButton.Background = new SolidColorBrush(btnActivatedColour);
                }
            }
        }
        private void DeactivateButton()
        {
            if (currentButton != null)
                currentButton.Background = new SolidColorBrush(Color.FromRgb(221, 221, 221));
        }
        

        private void btnResetColour_Click(object sender, RoutedEventArgs e)
        {
            colourPickerBackground.SelectedBrush = new SolidColorBrush(Color.FromRgb(64, 61, 61));
            colourPickerVertex.SelectedBrush = new SolidColorBrush(Colors.DodgerBlue);
            colourPickerWeight.SelectedBrush = new SolidColorBrush(Colors.White);
            colourPickerLabel.SelectedBrush = new SolidColorBrush(Colors.Black);
            colourPickerVertexStroke.SelectedBrush = new SolidColorBrush(Colors.Black);
        }

        private void mainCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {

        }
        private void mouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && currentButton == btnDragAndDrop)
            {
                ellipseToDrop = sender as Ellipse;
                DragDrop.DoDragDrop(sender as Ellipse, sender as Ellipse, DragDropEffects.Move);
            }
        }
        private void mainCanvas_Drop(object sender, DragEventArgs e)
        {
            
        }
        private void mainCanvas_DragOver(object sender, DragEventArgs e)
        {
            Point dropPosition = e.GetPosition(mainCanvas);
            Canvas.SetLeft(ellipseToDrop, dropPosition.X);
            Canvas.SetTop(ellipseToDrop, dropPosition.Y);
            foreach (TextBlock label in vertexTxBoxList)
            {

                if (label.Text == ellipseToDrop.Name.Substring(3))
                {
                    Canvas.SetLeft(label, dropPosition.X - 4);
                    Canvas.SetTop(label, dropPosition.Y - 9);
                }
            }
        }

        private void btnResetComponentShape_Click(object sender, RoutedEventArgs e)
        {
            edgeThicknessSlider.Value = edgeThicknessSlider.Minimum;
            vertexDiameterSlider.Value = vertexDiameterSlider.Minimum;
            weightAndLabelFontSizeSlider.Value = weightAndLabelFontSizeSlider.Minimum;
        }

        private void btnClearOtherAlgoLogs_Click(object sender, RoutedEventArgs e)
        {
            txLogsOtherAlgorithms.Clear();
        }

        private void btnClearPathFindingLogs_Click(object sender, RoutedEventArgs e)
        {
            txLogsPathFinding.Clear();
        }
        private void btnClearActionLogs_Click(object sender, RoutedEventArgs e)
        {
            txLogsActions.Clear();
        }

        private void btnAddVertex_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
        }

        private void btnAddConnection_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
        }

        private void btnDeleteVertex_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
        }

        private void btnDeleteConnection_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
        }

        private void btnTakeScreenshot_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
        }
        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
        }

        private void btnGenerateAdjList_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
            txLogsActions.AppendText(Graph.coutAdjList());
        }

        private void btnGenerateAdjMatrix_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
        }
        private void btnDragAndDrop_Click(object sender, RoutedEventArgs e)
        {
            ActivateButton(sender);
        }
        public void DisableFileButtons()
        {
            btnCreateNewGraph.IsEnabled = false;
            btnLoadNewGraph.IsEnabled = false;
            btnSaveGraph.IsEnabled = false;
            btnDeleteGraph.IsEnabled = false;
            btnGenerateAdjList.IsEnabled = false;
            btnLoadSavedGraph.IsEnabled = false;
        }
        public void EnableFileButtons()
        {
            btnCreateNewGraph.IsEnabled = true;
            btnLoadNewGraph.IsEnabled = true;
            btnSaveGraph.IsEnabled = true;
            btnDeleteGraph.IsEnabled = true;
            btnGenerateAdjList.IsEnabled = true;
            btnLoadSavedGraph.IsEnabled = true;
        }
        private void mainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            
            if (e.OriginalSource is Ellipse) //if they press the Vertex 
            {
                //check if they are joining vertices together or deleting vertices (make sure deleting is pressed)
                if (currentButton == btnDeleteVertex)
                {
                    Ellipse activeVertex = (Ellipse)e.OriginalSource;
                    List<Line> listOfLinesToRemove = new List<Line>();
                    //loop through lines and delete any lines that come out of it
                    foreach (Ellipse vertex in vertexList)
                    {
                        if (vertex != activeVertex)
                        {
                            Ellipse largerEllipse = GetMaxEllipse(vertex, activeVertex);
                            Ellipse smallerEllipse = GetMinEllipse(vertex, activeVertex);
                            string lineNameToFind = "line" + smallerEllipse.Name.Substring(3).ToString() + "to" + largerEllipse.Name.Substring(3).ToString();
                            foreach (Line line in edgeList)
                            {
                                if (line.Name == lineNameToFind)
                                {
                                    listOfLinesToRemove.Add(line);
                                }
                            }
                        }
                    }
                    foreach(Line line in listOfLinesToRemove)
                    {
                        edgeList.Remove(line);
                        mainCanvas.Children.Remove(line);
                    }
                    mainCanvas.Children.Remove(activeVertex);
                    foreach (TextBlock vertexLabel in vertexTxBoxList)
                    {
                        if (vertexLabel.Text == activeVertex.Name.Substring(3))
                        {
                            mainCanvas.Children.Remove(vertexLabel);
                            vertexList.Remove(activeVertex);//delete it from the list
                            vertexTxBoxList.Remove(vertexLabel);
                            Graph.RemoveVertex(Convert.ToInt32(activeVertex.Name.Substring(3)));
                            txLogsActions.AppendText(Graph.Name + ".RemoveVertex(" + Convert.ToInt32(activeVertex.Name.Substring(3)).ToString() + ");\n");
                            break;
                        }
                    }
                    
                }
                else if (currentButton == btnAddConnection)
                {
                    buttonSelectionCount += 1;

                    if (buttonSelectionCount % 2 == 0 && buttonSelectionCount != 0)
                    {
                        vertexToConnectTo = (Ellipse)e.OriginalSource;
                        ConnectEdges connectEdges = new ConnectEdges();
                        if (lastSelectedVertex == vertexToConnectTo)
                        { 
                            EnableAllActionButtons();
                            EnableTabControlsNotAction();
                            EnableFileButtons();
                            labelConnectionDescription.Content = "";
                        }
                        else if (connectEdges.ShowDialog() == true)
                        {
                            ConnectVertices(lastSelectedVertex, vertexToConnectTo); //add the edge
                            labelConnectionDescription.Content = "";
                            EnableTabControlsNotAction();
                            EnableAllActionButtons();
                            EnableFileButtons();
                        }
                        else
                        {
                            buttonSelectionCount -= 1;
                        }
                    }
                    else if (buttonSelectionCount % 2 == 1)
                    {
                        
                        lastSelectedVertex = (Ellipse)e.OriginalSource;
                        labelConnectionDescription.Content = "From vertex " + lastSelectedVertex.Name.Substring(3) + " to.....";
                        DisableFileButtons();
                        DisableTabControlsNotAction();
                        DisableAllActionButtons();
                    }

                }
            }
            else if (e.OriginalSource is Line)
            {
                //check if they are deleting edges (make sure deleting is pressed)
                
                //Line activeEdge = (Line)e.OriginalSource;
                //mainCanvas.Children.Remove(activeEdge);
            }
            else
            {
                if (currentButton == btnAddVertex) //this is where the button will be added to the canvas
                {
                    
                    Ellipse vertexToAdd = new Ellipse() {StrokeThickness=2};
                    //binding the stroke of the vertices to the color picker
                    Binding bindingStroke = new Binding("SelectedBrush")
                    {
                        Source = colourPickerVertexStroke,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.TwoWay
                    };
                    vertexToAdd.SetBinding(Ellipse.StrokeProperty, bindingStroke);
                    //binding the fill colour of the vertices to the color picker
                    Binding bindingFill = new Binding("SelectedBrush")
                    {
                        Source = colourPickerVertex,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.TwoWay
                    };
                    vertexToAdd.SetBinding(Ellipse.FillProperty, bindingFill);
                    Binding bindingDiameter = new Binding("Value")
                    {
                        Source = vertexDiameterSlider,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    };
                    vertexToAdd.SetBinding(Ellipse.HeightProperty, bindingDiameter);
                    vertexToAdd.SetBinding(Ellipse.WidthProperty, bindingDiameter);

                    //positioning the vertex in the canvas.
                    double vertexCenterXMousePos = Mouse.GetPosition(mainCanvas).X;
                    double vertexCenterYMousePos = Mouse.GetPosition(mainCanvas).Y;
                    vertexToAdd.Margin = new Thickness(-100000); //margin of 100000 so that it resizes around the center.
                    Canvas.SetLeft(vertexToAdd, vertexCenterXMousePos);
                    Canvas.SetTop(vertexToAdd, vertexCenterYMousePos);
                    string vertexId = buttonId.ToString();
                    vertexToAdd.Name = "btn" + vertexId;
                    buttonId += 1;
                    vertexList.Add(vertexToAdd);
                    Canvas.SetZIndex(vertexToAdd, 3);
                    mainCanvas.Children.Add(vertexToAdd);
                    //give the buttons drag and drop event handlers
                    vertexToAdd.MouseMove += mouseMove;
                    TextBlock vertexLabel = new TextBlock() { Text = vertexId, FontSize=15};
                    Binding bindingLabelForeground = new Binding("SelectedBrush")
                    {
                        Source = colourPickerLabel,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.TwoWay
                    };
                    vertexLabel.SetBinding(TextBlock.ForegroundProperty, bindingLabelForeground);
                    //add a vertex to the data structure
                    Graph.AddVertex();
                    txLogsActions.AppendText(Graph.Name + ".AddVertex()\n");
                    Canvas.SetZIndex(vertexLabel, 4);
                    Canvas.SetTop(vertexLabel, Canvas.GetTop(vertexToAdd) - 9);
                    Canvas.SetLeft(vertexLabel, Canvas.GetLeft(vertexToAdd) - 4);
                    vertexTxBoxList.Add(vertexLabel);
                    mainCanvas.Children.Add(vertexLabel);
                }
            }
        }
        private Ellipse GetMinEllipse(Ellipse vertex1, Ellipse vertex2)
        {
            return (Convert.ToInt32(vertex1.Name.Substring(3)) < Convert.ToInt32(vertex2.Name.Substring(3))) ? vertex1 : vertex2;
        }
        private Ellipse GetMaxEllipse(Ellipse vertex1, Ellipse vertex2)
        {
            return (Convert.ToInt32(vertex1.Name.Substring(3)) > Convert.ToInt32(vertex2.Name.Substring(3))) ? vertex1 : vertex2;
        }
        public void DeleteLine(string lineName)
        {
            /////above
        }
        private void ConnectVertices(Ellipse v1, Ellipse v2)
        {
            Ellipse smallerEllipse = GetMinEllipse(v1, v2);
            Ellipse largerEllipse = GetMaxEllipse(v1, v2);
            string lineName = "line" + smallerEllipse.Name.Substring(3).ToString() + "to" + largerEllipse.Name.Substring(3).ToString(); //have the name of the line in the form atob where a < b
            foreach (Line line in edgeList) //check if a line already exists
            {
                if (line.Name == lineName)
                {
                    //call delete line and pass in the line name
                }
            }
            Line temp = new Line();
            temp.StrokeThickness = 4;
            temp.Name = lineName;
            temp.Stroke = new SolidColorBrush(Colors.Black);
            Canvas.SetZIndex(temp, 0);
            Binding bindingV1X = new Binding
            {
                Source = smallerEllipse,
                Path = new PropertyPath(Canvas.LeftProperty),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            temp.SetBinding(Line.X1Property, bindingV1X);
            Binding bindingV1Y = new Binding
            {
                Source = smallerEllipse,
                Path = new PropertyPath(Canvas.TopProperty),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            temp.SetBinding(Line.Y1Property, bindingV1Y);
            Binding bindingV2X = new Binding
            {
                Source = largerEllipse,
                Path = new PropertyPath(Canvas.LeftProperty),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            temp.SetBinding(Line.X2Property, bindingV2X);
            Binding bindingV2Y = new Binding
            {
                Source = largerEllipse,
                Path = new PropertyPath(Canvas.TopProperty),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            temp.SetBinding(Line.Y2Property, bindingV2Y);
            edgeList.Add(temp);
            //bind the weight to the center of the midpoint now.
            mainCanvas.Children.Add(temp);
        }
        private void btnCreateNewGraph_Click(object sender, RoutedEventArgs e)
        {
            string name = "";
            NameCreatedGraph nameGraphWindow = new NameCreatedGraph();
            nameGraphWindow.ShowDialog();
            if (nameGraphWindow.DialogResult == true)
            {
                name = nameGraphWindow.txBoxGraphName.Text;
                txLogsActions.AppendText("CreateNewGraph(" + name +")\n");
                DeleteGraph();
                CreateNewGraph(name);
                btnDeleteGraph.IsEnabled = true;
                graphCreated = true;
            }
        }
        public void DeleteGraph()
        {
            mainCanvas.Children.Clear();
            btnDeleteGraph.IsEnabled = false;
            labelGraphName.Content = "";
            buttonId = 0;
            Graph = null;
            lastSelectedVertex = null;
            vertexToConnectTo = null;
            vertexTxBoxList = new List<TextBlock>();
            vertexList = new List<Ellipse>();
            edgeList = new HashSet<Line>();
            graphCreated = false;
        }
        
        private void btnLoadNewGraph_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLoadSavedGraph_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDeleteGraph_Click(object sender, RoutedEventArgs e)
        {
            DeleteGraph();
            DisableAllActionButtons();
        }

        private void btnSaveGraph_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

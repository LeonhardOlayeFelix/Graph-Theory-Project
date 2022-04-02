using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace Interface_2
{
    public partial class MainWindow : Window
    {
        private void ActivateButton(object btnSender) //highlight a button when its pressed
        {
            if (btnSender != null) //make sure that the button isnt null
            {
                if (currentButton != (Button)btnSender) //if the same button is not pressed
                {
                    DeactivateButton(); //'deactivate' the previous button
                    currentButton = (Button)btnSender;
                    currentButton.Background = new SolidColorBrush(btnActivatedColour); //'activate' the current button
                }
            }
        }
        private void DeactivateButton() //'deactivates' button
        {
            if (currentButton != null)
                currentButton.Background = new SolidColorBrush(Color.FromRgb(221, 221, 221));
        }


        private void btnResetColour_Click(object sender, RoutedEventArgs e)
        {
            //resets the colours of all the colour pickers
            colourPickerBackground.SelectedBrush = new SolidColorBrush(Color.FromRgb(64, 61, 61));
            colourPickerVertex.SelectedBrush = new SolidColorBrush(Colors.DodgerBlue);
            colourPickerWeight.SelectedBrush = new SolidColorBrush(Colors.Black);
            colourPickerLabel.SelectedBrush = new SolidColorBrush(Colors.White);
            colourPickerVertexStroke.SelectedBrush = new SolidColorBrush(Colors.Black);
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            //monitors the mouse as it hovers over a vertex
            if (e.LeftButton == MouseButtonState.Pressed && currentButton == btnDragAndDrop)//if, whilst hovering, they press the vertex
            {
                ellipseToDrop = sender as Ellipse; //update teh
                DragDrop.DoDragDrop(sender as Ellipse, sender as Ellipse, DragDropEffects.Move); //start the drag function on this vertex
            }
        }

        private void btnImportGraph_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
        }

        private void btnExportGraph_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
        }
        private void btnClearOtherAlgoLogs_Click(object sender, RoutedEventArgs e)
        {
            labelExtraInfo.Content = "Logs Cleared.";
            txLogsOtherAlgorithms.Clear();
        }

        private void btnClearPathFindingLogs_Click(object sender, RoutedEventArgs e)
        {
            labelExtraInfo.Content = "Logs Cleared.";
            txLogsPathFinding.Clear();
        }
        private void btnClearActionLogs_Click(object sender, RoutedEventArgs e)
        {
            labelExtraInfo.Content = "Logs Cleared.";
            txLogsActions.Clear();
        }

        private void btnAddVertex_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click anywhere on the canvas to place a vertex";
            ActivateButton(sender);
        }

        private void btnAddConnection_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click two vertices you want to connect and provide the weight";
            ActivateButton(sender);
        }

        private void btnDeleteVertex_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click a vertex to delete it from the canvas";
            ActivateButton(sender);
        }

        private void btnDeleteConnection_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Click an edge to Delete it from the canvas";
            ActivateButton(sender);
        }

        private void btnTakeScreenshot_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
            labelExtraInfo.Content = "Screenshot Taken";
            ActivateButton(sender);
        }
        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            labelExtraInfo.Content = "Click freely around the Canvas";
            ActivateButton(sender);
        }
        private void btnRouteInspStartAtEnd_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
        }

        private void btnRouteInspStartAndEnd_Click(object sender, RoutedEventArgs e)
        {
            HideValencies();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

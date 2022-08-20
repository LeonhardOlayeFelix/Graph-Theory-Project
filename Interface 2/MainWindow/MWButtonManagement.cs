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
    [Serializable]

    public partial class MainWindow : Window
    {
        public void DisableAllAlgorithmButtons()
        {
            btnDijkstrasShort.IsEnabled = false;
            btnHighlightPaths.IsEnabled = false;
            btnToggleValencies.IsEnabled = false;
            btnRouteInspStartAndEnd.IsEnabled = false;
            btnRouteInspStartAtEnd.IsEnabled = false;
            btnBreadthFirst.IsEnabled = false;
            btnDepthFirst.IsEnabled = false;
            btnPrims.IsEnabled = false;
            btnKruskals.IsEnabled = false;
            btnHighlightPaths.IsEnabled = false;
        }
        public void DisableAllActionButtons()
        {
            //disables all of the action buttons but the file ones
            btnAddVertex.IsEnabled = false;
            btnAddConnection.IsEnabled = false;
            btnDeleteConnection.IsEnabled = false;
            btnDeleteVertex.IsEnabled = false;
            btnTakeScreenshot.IsEnabled = false;
            btnDragAndDrop.IsEnabled = false;
            btnDefault.IsEnabled = false;
            btnRevertOnePositions.IsEnabled = false;
            btnRevertPositions.IsEnabled = false;
            btnDeleteAllEdges.IsEnabled = false; ;

        }
        public void DisableTabControl()
        {
            //disable all the tab controls apart from action and adjset
            tabControlAlgorithms.IsEnabled = false;
            tabControlDisplay.IsEnabled = false;
            tabControlActions.IsEnabled = false;
        }
        public void EnableAllAlgorithmButtons()
        {
            btnDijkstrasShort.IsEnabled = true;
            btnHighlightPaths.IsEnabled = true;
            btnToggleValencies.IsEnabled = true;
            btnRouteInspStartAndEnd.IsEnabled = true;
            btnRouteInspStartAtEnd.IsEnabled = true;
            btnBreadthFirst.IsEnabled = true;
            btnDepthFirst.IsEnabled = true;
            btnPrims.IsEnabled = true;
            btnKruskals.IsEnabled = true;
            btnHighlightPaths.IsEnabled = true;
        }
        public void EnableAllActionButtons()
        {
            //enables all of the action buttons 
            btnAddVertex.IsEnabled = true;
            btnAddConnection.IsEnabled = true;
            btnDeleteConnection.IsEnabled = true;
            btnDeleteVertex.IsEnabled = true;
            btnTakeScreenshot.IsEnabled = true;
            btnDragAndDrop.IsEnabled = true;
            btnDefault.IsEnabled = true;
            btnRevertOnePositions.IsEnabled = true;
            btnRevertPositions.IsEnabled = true;
            btnDeleteAllEdges.IsEnabled = true;
        }
        public void EnableTabControl()
        {
            //enable all the tab controls
            tabControlAlgorithms.IsEnabled = true;
            tabControlDisplay.IsEnabled = true;
            tabControlActions.IsEnabled = true;
        }

    }
}

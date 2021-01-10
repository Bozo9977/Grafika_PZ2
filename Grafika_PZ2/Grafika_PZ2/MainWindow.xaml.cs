using Grafika_PZ2.Helpers;
using Grafika_PZ2.Model;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Grafika_PZ2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NetworkModel networkModel = new NetworkModel();

        public static double WindowWidth = System.Windows.SystemParameters.PrimaryScreenWidth, WindowHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

        public ulong[,] grid = new ulong[501, 501];

        System.Windows.Point scrollMousePoint = new System.Windows.Point();
        double hOff = 1;
        double vEff = 1;
        public MainWindow()
        {
            InitializeComponent();
            networkModel = NetworkModel.InitModel(@"..\..\Xml\Geographic.xml");

            GridHelper.InitializeGrid(grid);
            GridHelper.PositionElemntsWithinGrid(networkModel, grid);
            
            Node[,] linesGrid = new Node[501, 501];
            InitializeGridForLines(linesGrid);
            
            foreach(var line in networkModel.lines)
            {
                BFS(line.FirstEnd, line.SecondEnd, line.Id, line.Name, linesGrid);
            }
            

            List<Polyline> pom = new List<Polyline>();

            int pointsCount = 2;

            while(Lines.Count!=0)
            {
                var lines = Lines.FindAll(x => x.Points.Count == pointsCount);
                pom.AddRange(lines);
                for(int i = 0; i<Lines.Count; i++)
                {
                    for(int j =0; j<lines.Count; j++)
                    {
                        if (Lines[i].Uid == lines[j].Uid)
                        {
                            Lines.Remove(Lines[i]);
                        }
                    }
                }
                pointsCount++;
            }
            
            List<Polyline> drawnLines = new List<Polyline>();
            pointsCount = 2;
            System.Windows.Point point1 = new System.Windows.Point();
            System.Windows.Point point2 = new System.Windows.Point();

            for (int i=0; i<pom.Count; i++)
            {
                for (int j=0; j<pom[i].Points.Count-1; j++)
                {
                    Polyline newLine = new Polyline();
                    try
                    {
                        point1 = pom[i].Points[j];
                        point2 = pom[i].Points[j + 1];

                        Polyline p = drawnLines.SingleOrDefault(x => x.Points.Contains(point1) && x.Points.Contains(point2));
                        if(p != null)
                        {
                            break;
                        }
                        
                        newLine.Uid = pom[i].Uid;
                        newLine.Stroke = Brushes.Black;
                        newLine.StrokeThickness = 0.5;
                        ToolTip tt = new ToolTip();
                        tt.Content = ((ToolTip)pom[i].ToolTip).Content;
                        newLine.ToolTip = tt;
                        newLine.Points.Add(point1);
                        newLine.Points.Add(point2);
                    }
                    catch { }
                    drawnLines.Add(newLine);
                }
            }
            
            DrawLines1(drawnLines);

            DrawHelper.DrawElements(grid, canvas, networkModel);
            ScaleTransform st = canvas.LayoutTransform as ScaleTransform;
            if (st == null)
            {
                st = new ScaleTransform();
                canvas.LayoutTransform = st;
            }
            st.ScaleX = st.ScaleY = st.ScaleX * 2.5;

            scrollViewer.ScrollToHorizontalOffset(hOff + st.ScaleX);
            scrollViewer.ScrollToVerticalOffset(vEff + st.ScaleY);
            
        }

       
        #region BFS
        public void DrawLines1(List<Polyline> lines)
        {
            foreach(var line in lines)
            {
                canvas.Children.Add(line);
            }
        }

        public struct Node
        {
            public ulong Id;
            public int row;
            public int column;
            public int cellId;
            public int pred;
        }

        public void InitializeGridForLines(Node[,] linesGrid)
        {
            for(int i = 0; i < 501; i++)
            {
                for(int j = 0; j < 501; j++)
                {
                    Node node = new Node();
                    node.Id = grid[i, j];
                    node.row = i;
                    node.column = j;
                    node.cellId = i * 501 + j;
                    node.pred = -1;
                    linesGrid[i, j] = node;
                }
            }
        }
        

        List<Polyline> Lines = new List<Polyline>();
        private void BFS(ulong idStart, ulong idEnd, ulong lineId, string lineName, Node[,] linesGrid)
        {


            Node startNode = new Node(), endNode = new Node();
            foreach (SubstationEntity item in networkModel.substations)
            {
                if (item.Id == idStart)
                    startNode = linesGrid[item.Row, item.Column];
                else if (item.Id == idEnd)
                    endNode = linesGrid[item.Row, item.Column];

            }

            foreach (NodeEntity item in networkModel.nodes)
            {
                if (item.Id == idStart)
                    startNode = linesGrid[item.Row, item.Column];
                else if (item.Id == idEnd)
                    endNode = linesGrid[item.Row, item.Column];
            }

            foreach (SwitchEntity item in networkModel.switches)
            {
                if (item.Id == idStart)
                    startNode = linesGrid[item.Row, item.Column];
                else if (item.Id == idEnd)
                    endNode = linesGrid[item.Row, item.Column];
            }

            //int countStep = 0;
            Queue<Node> Q = new Queue<Node>();
            Q.Enqueue(startNode);

            while (!Q.Count.Equals(0))
            {
                Node u = Q.Dequeue();

                if (u.Id == endNode.Id)
                {
                    Node temp = u;
                    Node last = u;

                    PointCollection path = new PointCollection();

                    while (!temp.Id.Equals(startNode.Id))
                    {

                        int row1 = temp.row;
                        int column1 = temp.column;
                        path.Add(new System.Windows.Point(column1 * WindowWidth / 501 + 0.5, row1 * (WindowHeight - 60) / 501 + 0.5));
                        int row2 = temp.pred / 501;
                        int column2 = temp.pred % 501;
                        last = temp;
                        temp = linesGrid[row2, column2];

                    }

                    int row = temp.cellId / 501;
                    int column = temp.cellId % 501;

                    path.Add(new System.Windows.Point(column * WindowWidth / 501 + 0.5, row * (WindowHeight - 60) / 501 + 0.5));

                    Polyline p = new Polyline();
                    p.Uid = lineId.ToString();
                    p.Stroke = Brushes.Black;
                    p.StrokeThickness = 0.5;
                    p.Points = path;
                    ToolTip tt = new ToolTip();
                    tt.Content = $"{lineId.ToString()}\nName: {lineName}";
                    p.ToolTip = tt;
                    Lines.Add(p);
                    return;
                }
                else
                {
                    int row = u.cellId / 501;
                    int column = u.cellId % 501;


                    int exit = 0;
                    int step = 1;
                    bool[] flagArray = new bool[] { false, false, false, false };

                    int endRow = endNode.cellId / 501;
                    int endColumn = endNode.cellId % 501;

                    while (!exit.Equals(4))
                    {

                        try
                        {
                            if (row - step < endRow && !flagArray[0])
                            {
                                exit++;
                                flagArray[0] = true;
                            }
                            else if (((row - step) == endRow && !flagArray[0]))
                            {
                                flagArray[0] = true;
                                linesGrid[row - step, column].pred = u.cellId;
                                exit++;
                                if (linesGrid[row - step, column].Id.Equals(endNode.Id))
                                {
                                    Q.Clear();
                                    Q.Enqueue(linesGrid[row - step, column]);
                                    break;
                                }
                                Q.Enqueue(linesGrid[row - step, column]);
                            }
                        }
                        catch { }
                        try
                        {
                            if (row + step > endRow && !flagArray[1])
                            {
                                exit++;
                                flagArray[1] = true;
                            }
                            else if (((row + step) == endRow && !flagArray[1]))
                            {

                                flagArray[1] = true;
                                linesGrid[row + step, column].pred = u.cellId;
                                exit++;
                                if (linesGrid[row + step, column].Id.Equals(endNode.Id))
                                {
                                    Q.Clear();
                                    Q.Enqueue(linesGrid[row + step, column]);
                                    break;

                                }
                                Q.Enqueue(linesGrid[row + step, column]);
                            }
                        }
                        catch { }
                        try
                        {
                            if (column - step < endColumn && !flagArray[2])
                            {
                                exit++;
                                flagArray[2] = true;
                            }
                            else
                               if (((column - step) == endColumn && !flagArray[2]))
                            {

                                flagArray[2] = true;
                                linesGrid[row, column - step].pred = u.cellId;
                                exit++;
                                if (linesGrid[row, column - step].Id.Equals(endNode.Id))
                                {
                                    Q.Clear();
                                    Q.Enqueue(linesGrid[row, column - step]);
                                    break;

                                }
                                Q.Enqueue(linesGrid[row, column - step]);
                            }
                        }
                        catch { }
                        try
                        {
                            if (column + step > endColumn && !flagArray[3])
                            {
                                exit++;
                                flagArray[3] = true;
                            }
                            else
                               if (((column + step) == endColumn && !flagArray[3]))
                            {

                                flagArray[3] = true;
                                linesGrid[row, column + step].pred = u.cellId;
                                exit++;
                                if (linesGrid[row, column + step].Id.Equals(endNode.Id))
                                {
                                    Q.Clear();
                                    Q.Enqueue(linesGrid[row, column + step]);
                                    break;

                                }
                                Q.Enqueue(linesGrid[row, column + step]);
                            }
                        }
                        catch { }
                        step++;


                    }

                   
                }

            }
        }
        #endregion

        string firstEnd ="";
        string secondEnd ="";
        Brush prevFirstBrush;
        Brush prevSecondBrush;
        private void Canvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Change color of connected nodes?", "Line click", MessageBoxButton.YesNoCancel);
            if ( res == MessageBoxResult.Yes)
            {
                if (e.OriginalSource is Polyline)
                {
                    Polyline p = new Polyline();
                    p = ((Polyline)e.OriginalSource);

                    ToolTip tt = new ToolTip();
                    tt = (ToolTip)p.ToolTip;

                    ulong idLine = ulong.Parse((tt.Content.ToString()).Split('\n')[0]);
                    ulong first = 0;
                    ulong second = 0;

                    foreach (var l in networkModel.lines)
                    {
                        if (idLine == l.Id)
                        {
                            first = l.FirstEnd;
                            second = l.SecondEnd;
                        }
                    }

                    if (!String.IsNullOrEmpty(firstEnd))
                    {
                        foreach (var item in canvas.Children)
                        {
                            if (((Shape)item).Uid == firstEnd)
                            {
                                ((Shape)item).Fill = prevFirstBrush;
                            }
                            if (((Shape)item).Uid == secondEnd)
                            {
                                ((Shape)item).Fill = prevSecondBrush;
                            }
                        }
                    }

                    foreach (var item in canvas.Children)
                    {
                        if (((Shape)item).Uid == first.ToString())
                        {
                            prevFirstBrush = ((Shape)item).Fill;
                            ((Shape)item).Fill = Brushes.Orange;
                            firstEnd = ((Shape)item).Uid;
                        }
                        if (((Shape)item).Uid == second.ToString())
                        {
                            prevSecondBrush = ((Shape)item).Fill;
                            ((Shape)item).Fill = Brushes.Orange;
                            secondEnd = ((Shape)item).Uid;
                        }
                    }

                }
            }

            
        }
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Shape)
            {
                if(((Shape)e.OriginalSource).Width == 1)
                {
                    DoubleAnimation da = new DoubleAnimation();
                    da.Duration = new Duration(TimeSpan.FromSeconds(0.01));
                    da.To = 50;
                    ((Shape)e.OriginalSource).Height *= 10;
                    ((Shape)e.OriginalSource).Width *= 10;
                    PropertyPath ppH = new PropertyPath("((Shape)e.OriginalSource).Height");
                    PropertyPath ppW = new PropertyPath("((Shape)e.OriginalSource).Width");
                    Storyboard.SetTargetProperty(da, ppH);
                    Storyboard.SetTargetProperty(da, ppW);
                }
                else
                {
                    DoubleAnimation da = new DoubleAnimation();
                    da.To = 5;
                    da.Duration = new Duration(TimeSpan.FromSeconds(0.01));
                    ((Shape)e.OriginalSource).Height /= 10;
                    ((Shape)e.OriginalSource).Width /= 10;
                    PropertyPath ppH1 = new PropertyPath("((Shape)e.OriginalSource).Height");
                    PropertyPath ppW1 = new PropertyPath("((Shape)e.OriginalSource).Width");
                    Storyboard.SetTargetProperty(da, ppH1);
                    Storyboard.SetTargetProperty(da, ppW1);
                }
            }

        }

        #region Scroll

        private void scrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            scrollMousePoint = e.GetPosition(scrollViewer);
            hOff = scrollViewer.HorizontalOffset;
            vEff = scrollViewer.VerticalOffset;
            scrollViewer.CaptureMouse();
        }

        private void scrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (scrollViewer.IsMouseCaptured)
            {
                scrollViewer.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(scrollViewer).X));
                scrollViewer.ScrollToVerticalOffset(vEff + (scrollMousePoint.Y - e.GetPosition(scrollViewer).Y));
            }
        }

        private void scrollViewer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            scrollViewer.ReleaseMouseCapture();
        }

       

        private void scrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            System.Windows.Point mouseAtImage = e.GetPosition(canvas);
            System.Windows.Point mouseAtScrollViewer = e.GetPosition(scrollViewer);

            ScaleTransform st = canvas.LayoutTransform as ScaleTransform;
            if (st == null)
            {
                st = new ScaleTransform();
                canvas.LayoutTransform = st;
            }

            if (e.Delta > 0)
            {
                st.ScaleX = st.ScaleY = st.ScaleX * 1.25;
                if (st.ScaleX > 64) st.ScaleX = st.ScaleY = 64;
            }
            else
            {
                st.ScaleX = st.ScaleY = st.ScaleX / 1.25;
                if (st.ScaleX < 1) st.ScaleX = st.ScaleY = 1;
            }
            #region [this step is critical for offset]
            scrollViewer.ScrollToHorizontalOffset(0);
            scrollViewer.ScrollToVerticalOffset(0);
            this.UpdateLayout();
            #endregion

            Vector offset = canvas.TranslatePoint(mouseAtImage, scrollViewer) - mouseAtScrollViewer; // (Vector)middleOfScrollViewer;
            scrollViewer.ScrollToHorizontalOffset(offset.X);
            scrollViewer.ScrollToVerticalOffset(offset.Y);
            this.UpdateLayout();

            e.Handled = true;
        }
        #endregion
    }
}

using Grafika_PZ2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Grafika_PZ2.Helpers
{
    public class DrawHelper
    {

        public static void DrawElements(ulong[,] grid, Canvas canvas, NetworkModel model)
        {
            DrawSubstations(grid, canvas, model.substations);
            DrawSwitches(grid, canvas, model.switches);
            DrawNodes(grid, canvas, model.nodes);

        }

        public static void DrawSubstations(ulong[,] grid, Canvas canvas, List<SubstationEntity> substations)
        {
            for(int i = 0; i < substations.Count; i++)
            {
                Ellipse el = new Ellipse();
                el.Width = 1;
                el.Height = 1;
                el.Fill = Brushes.Red;
                el.Uid = substations[i].Id.ToString();
                ToolTip tt = new ToolTip();
                tt.Content = $"{substations[i].Id}\nName: {substations[i].Name}";
                el.ToolTip = tt;
                Canvas.SetLeft(el, substations[i].Column * MainWindow.WindowWidth / 501);
                Canvas.SetTop(el, substations[i].Row * (MainWindow.WindowHeight - 60) / 501);
                canvas.Children.Add(el);
            }
        }

        public static void DrawSwitches(ulong[,] grid, Canvas canvas, List<SwitchEntity> switches)
        {
            for (int i = 0; i < switches.Count; i++)
            {
                Ellipse el = new Ellipse();
                el.Width = 1;
                el.Height = 1;
                el.Fill = Brushes.Blue;
                el.Uid = switches[i].Id.ToString();
                ToolTip tt = new ToolTip();
                tt.Content = $"{switches[i].Id}\nStatus: {switches[i].Status}";
                el.ToolTip = tt;
                Canvas.SetLeft(el, switches[i].Column * MainWindow.WindowWidth / 501);
                Canvas.SetTop(el, switches[i].Row * (MainWindow.WindowHeight - 60) / 501);
                canvas.Children.Add(el);
            }
        }

        public static void DrawNodes(ulong[,] grid, Canvas canvas, List<NodeEntity> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                Ellipse el = new Ellipse();
                el.Width = 1;
                el.Height = 1;
                el.Fill = Brushes.Green;
                el.Uid = nodes[i].Id.ToString();
                ToolTip tt = new ToolTip();
                tt.Content = $"{nodes[i].Id}\nName: {nodes[i].Name}";
                el.ToolTip = tt;
                Canvas.SetLeft(el, nodes[i].Column * MainWindow.WindowWidth / 501);
                Canvas.SetTop(el, nodes[i].Row * (MainWindow.WindowHeight - 60) / 501);
                canvas.Children.Add(el);
            }
        }
    }
}

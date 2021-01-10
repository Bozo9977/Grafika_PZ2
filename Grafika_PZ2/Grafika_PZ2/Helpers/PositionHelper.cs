using Grafika_PZ2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafika_PZ2.Helpers
{
    public static class PositionHelper
    {

        public static void TranslatePositions(NetworkModel model)
        {
            double newLongitude, newLatitude;
            for(int i =0; i< model.substations.Count; i++)
            {
                ToLatLon(model.substations[i].X, model.substations[i].Y, 34, out newLongitude, out newLatitude);
                model.substations[i].X = newLongitude;
                model.substations[i].Y = newLatitude;                
            }
            for (int i = 0; i < model.switches.Count; i++)
            {
                ToLatLon(model.switches[i].X, model.switches[i].Y, 34, out newLongitude, out newLatitude);
                model.switches[i].X = newLongitude;
                model.switches[i].Y = newLatitude;
            }
            for (int i = 0; i < model.nodes.Count; i++)
            {
                ToLatLon(model.nodes[i].X, model.nodes[i].Y, 34, out newLongitude, out newLatitude);
                model.nodes[i].X = newLongitude;
                model.nodes[i].Y = newLatitude;
            }
            

            double minx = minX(model), maxx = maxX(model), miny = minY(model), maxy = maxY(model);

            for(int i = 0; i<model.substations.Count; i++)
            {
                int newRow = (int)(((model.substations[i].X - minx) / (maxx - minx)) * 500);
                int newColumn = 500 - (int)(((model.substations[i].Y - miny) / (maxy - miny)) * 500);
                
                model.substations[i].Column = newRow;
                model.substations[i].Row = newColumn;
            }

            for (int i = 0; i < model.switches.Count; i++)
            {
                int newRow = (int)(((model.switches[i].X - minx) / (maxx - minx)) * 500);
                int newColumn = 500 - (int)(((model.switches[i].Y - miny) / (maxy - miny)) * 500);

                model.switches[i].Column = newRow;
                model.switches[i].Row = newColumn;
            }

            for (int i = 0; i < model.nodes.Count; i++)
            {
                int newRow = (int)(((model.nodes[i].X - minx) / (maxx - minx)) * 500);
                int newColumn = 500 - (int)(((model.nodes[i].Y - miny) / (maxy - miny)) * 500);
                
                model.nodes[i].Column = newRow;
                model.nodes[i].Row = newColumn;
            }
        }

        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double longitude, out double latitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }

        #region necessary functions for Row/Column translation
        public static double minX(NetworkModel model)
        {
            double minx = model.substations[0].X;

            foreach(var item in model.substations){
                if (minx > item.X)
                    minx = item.X;
            }

            foreach (var item in model.nodes)
            {
                if (minx > item.X)
                    minx = item.X;
            }

            foreach (var item in model.switches)
            {
                if (minx > item.X)
                    minx = item.X;
            }
            return minx;
        }
        public static double maxX(NetworkModel model)
        {
            double maxx = model.substations[0].X;

            foreach (var item in model.substations)
            {
                if (maxx < item.X)
                    maxx = item.X;
            }

            foreach (var item in model.nodes)
            {
                if (maxx < item.X)
                    maxx = item.X;
            }

            foreach (var item in model.switches)
            {
                if (maxx < item.X)
                    maxx = item.X;
            }
            return maxx;
        }
        public static double minY(NetworkModel model)
        {
            double miny = model.substations[0].Y;

            foreach (var item in model.substations)
            {
                if (miny > item.Y)
                    miny = item.Y;
            }

            foreach (var item in model.nodes)
            {
                if (miny > item.Y)
                    miny = item.Y;
            }

            foreach (var item in model.switches)
            {
                if (miny > item.Y)
                    miny = item.Y;
            }
            return miny;
        }
        public static double maxY(NetworkModel model)
        {
            double maxy = model.substations[0].Y;

            foreach (var item in model.substations)
            {
                if (maxy < item.Y)
                    maxy = item.Y;
            }

            foreach (var item in model.nodes)
            {
                if (maxy < item.Y)
                    maxy = item.Y;
            }

            foreach (var item in model.switches)
            {
                if (maxy < item.Y)
                    maxy = item.Y;
            }
            return maxy;
        }
        #endregion
    }
}

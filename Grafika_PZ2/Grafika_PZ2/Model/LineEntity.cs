using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Grafika_PZ2.Model
{
    [Serializable]
    [XmlRoot("NetworkModel")]
    public class LineEntity
    {
        private UInt64 id;
        private string name;
        private bool isUnderground;
        private double r;
        private string conductorMaterial;
        private string lineType;
        private int thermalConstantHeat;
        private UInt64 firstEnd;
        private UInt64 secondEnd;
        private List<System.Windows.Point> vertices = new List<System.Windows.Point>();

        public UInt64 Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public bool IsUnderground { get => isUnderground; set => isUnderground = value; }
        public double R { get => r; set => r = value; }
        public string ConductorMaterial { get => conductorMaterial; set => conductorMaterial = value; }
        public string LineType { get => lineType; set => lineType = value; }
        public int ThermalConstantHeat { get => thermalConstantHeat; set => thermalConstantHeat = value; }
        public UInt64 FirstEnd { get => firstEnd; set => firstEnd = value; }
        public UInt64 SecondEnd { get => secondEnd; set => secondEnd = value; }

        [XmlIgnore]
        public List<System.Windows.Point> Vertices { get => vertices; set => vertices = value; }

        public LineEntity()
        {
            this.vertices = new List<System.Windows.Point>();
        }

        public override string ToString()
        {
            return String.Format($"{Id}, {Name}, {ConductorMaterial}, {LineType}");
        }
    }
}

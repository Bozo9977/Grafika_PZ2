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
    public class SubstationEntity
    {
        private ulong id;
        private string name;
        private double x;
        private double y;
        private int row;
        private int column;

        public ulong Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }

        [XmlIgnore]
        public int Row { get => row; set => row = value; }
        [XmlIgnore]
        public int Column { get => column; set => column = value; }

        public SubstationEntity() { }

        public SubstationEntity(ulong id, string name, double x, double y)
        {
            this.id = id;
            this.name = name;
            this.x = x;
            this.y = y;
        }
    }
}

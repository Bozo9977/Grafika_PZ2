using Grafika_PZ2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Grafika_PZ2.Model
{
    [Serializable]
    public class NetworkModel
    {

        public static NetworkModel InitModel(string path)
        {
            NetworkModel networkModel = LoadXMLHelper.Load<NetworkModel>(path);
            PositionHelper.TranslatePositions(networkModel);
            LineRedundancyCheck(networkModel);
            return networkModel;
        }

        
        [XmlArray("Substations")]
        [XmlArrayItem("SubstationEntity", typeof(SubstationEntity))]
        public List<SubstationEntity> substations { get; set; }

        [XmlArray("Nodes")]
        [XmlArrayItem("NodeEntity", typeof(NodeEntity))]
        public List<NodeEntity> nodes { get; set; }

        [XmlArray("Switches")]
        [XmlArrayItem("SwitchEntity", typeof(SwitchEntity))]
        public List<SwitchEntity> switches { get; set; }

        [XmlArray("Lines")]
        [XmlArrayItem("LineEntity", typeof(LineEntity))]
        public List<LineEntity> lines { get; set; }


        public static void LineRedundancyCheck(NetworkModel model)
        {
            List<LineEntity> redundanciesRemoved = new List<LineEntity>();
            List<ulong> ids = new List<ulong>();
            foreach(var item in model.substations)
            {
                ids.Add(item.Id);
            }
            foreach(var item in model.switches)
            {
                ids.Add(item.Id);
            }
            foreach(var item in model.nodes)
            {
                ids.Add(item.Id);
            }

            foreach(var item in model.lines)
            {
                bool alreadyAdded = false;
               if(ids.Contains(item.FirstEnd) && ids.Contains(item.SecondEnd))
               {
                    for(int i = 0; i < redundanciesRemoved.Count; i++)
                    {
                        if((redundanciesRemoved[i].FirstEnd == item.FirstEnd && redundanciesRemoved[i].SecondEnd == item.SecondEnd) || (redundanciesRemoved[i].SecondEnd == item.FirstEnd && redundanciesRemoved[i].FirstEnd == item.SecondEnd))
                        {
                            alreadyAdded = true;
                            break;
                        }
                    }
                    if (!alreadyAdded)
                        redundanciesRemoved.Add(item);
               }
            }
            model.lines = new List<LineEntity>(redundanciesRemoved);

        }
    }
}

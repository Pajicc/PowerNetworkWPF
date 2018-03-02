using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using CIM.IEC61970.Base.Core;
using System.Collections.ObjectModel;
using CIM.IEC61970.Base.Wires;

namespace WPF1._5
{
    [Serializable, XmlRoot("user")]
    public class Database
    {
        public ObservableCollection<ACLineSegment> aclines { get; set; }
        public ObservableCollection<Terminal> terminals { get; set; }

        public ObservableCollection<BaseVoltage> BaseVoltages { get; set; }
        public ObservableCollection<Substation> Substations { get; set; }
        public ObservableCollection<ConnectivityNode> Nodes { get; set; }

        public Database()
        {
            aclines = new ObservableCollection<ACLineSegment>();
            terminals = new ObservableCollection<Terminal>();
            BaseVoltages = new ObservableCollection<BaseVoltage>();
            Nodes = new ObservableCollection<ConnectivityNode>();
            Substations = new ObservableCollection<Substation>();
        }
    }
}

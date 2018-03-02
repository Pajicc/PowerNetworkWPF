using CIM.IEC61970.Base.Core;
using CIM.IEC61970.Base.Wires;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace WPF1._5.EES
{
    public class CloneDeleteObject
    {
        public bool deleted { get; set; }           //ako nije obrisan onda je kloniran
        public bool substation { get; set; }        
        public Canvas UISub { get; set; }
        public Substation sub { get; set; }
        public List<Ellipse> UINodes { get; set; }

        public List<ConnectivityNode> nodes { get; set; }

        public enum Called { Substation, Node, Line }

        public Called call { get; set; }

        public bool onlyOneLine { get; set; }
        public List<Line> UIlines { get; set; }

        public List<ACLineSegment> lines { get; set; }

        public CloneDeleteObject()
        {
            lines = new List<ACLineSegment>();
            UIlines = new List<Line>();
            UINodes = new List<Ellipse>();
            nodes = new List<ConnectivityNode>();
        }
    }
}

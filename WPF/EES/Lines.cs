using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace WPF1._5.EES
{
    [Serializable, XmlRoot("Lines")]
    public class Lines
    {
        public string uid1 { get; set; }        //from - to
        public string uid2 { get; set; }

        public List<Line> linije { get; set; }

        public Lines()
        {
            linije = new List<Line>();
        }
    }
}

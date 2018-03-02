using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPF1._5.EES
{
    public class RectObject
    {

        public Canvas canvas { get; set; }
        public double old_top { get; set; }
        public double old_left { get; set; }

        public double new_top { get; set; }
        public double new_left { get; set; }

        public RectObject()
        {

        }

        public void RectObjectOld(Canvas can, double oldtop, double oldleft)
        {
            canvas = can;
            old_top = oldtop;
            old_left = oldleft;
        }

        public void RectObjectNew(Canvas can, double newtop, double newleft)
        {
            canvas = can;
            new_left = newleft;
            new_top = newtop;
        }
    }
}

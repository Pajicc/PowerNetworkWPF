using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF1._5
{
    public class Context
    {
        private static volatile Context instance;
        public ViewModel ct;
        public Window mw;
        public Window subwin;
        private Context()
        {
            ct = new ViewModel();
            mw = new Window();
            subwin = new Window();
        }

        public static Context getInstance()
        {
            if (instance == null)
            {
                instance = new Context();
            }
            return instance;
        }
    }
}

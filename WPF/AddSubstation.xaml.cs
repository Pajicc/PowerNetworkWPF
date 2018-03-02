using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF1._5
{
    /// <summary>
    /// Interaction logic for AddSubstation.xaml
    /// </summary>
    public partial class AddSubstation : Window
    {
        public AddSubstation()
        {
            InitializeComponent();
            Context wrap = Context.getInstance();
            wrap.subwin = this;
            this.DataContext = wrap.ct;
        }
    }
}

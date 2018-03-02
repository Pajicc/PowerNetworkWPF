using CIM.IEC61970.Base.Core;
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
    /// Interaction logic for AddLine.xaml
    /// </summary>
    public partial class AddLine : Window
    {
        public AddLine()
        {
            InitializeComponent();
            Context wrap = Context.getInstance();
            wrap.subwin = this;
            this.DataContext = wrap.ct;
        }

        private void LeftComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Context wrap = Context.getInstance();
            try
            {
                wrap.ct.pomListLeft.Clear();
                int index = LeftSub.SelectedItem.ToString().IndexOf("(");
                string subsName = LeftSub.SelectedItem.ToString().Substring(0, index);
                subsName = subsName.Trim();
                foreach (Substation sub in wrap.ct.subsList)
                {
                    if (sub.name.CompareTo(subsName) == 0)
                    {
                        foreach (ConnectivityNode node in sub.ConnectivityNodes)
                        {
                            wrap.ct.pomListLeft.Add(node);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Context wrap = Context.getInstance();
            try
            {
                wrap.ct.pomListRight.Clear();
                int index = RightSub.SelectedItem.ToString().IndexOf("(");
                string subsName = RightSub.SelectedItem.ToString().Substring(0, index);
                subsName = subsName.Trim();
                foreach (Substation sub in wrap.ct.subsList)
                {
                    if (sub.name.CompareTo(subsName) == 0)
                    {
                        foreach (ConnectivityNode node in sub.ConnectivityNodes)
                        {
                            wrap.ct.pomListRight.Add(node);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

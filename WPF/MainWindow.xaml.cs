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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using CIM.IEC61970.Base.Core;
using CIM.IEC61970.Base.Wires;
using WPF1._5.EES;

namespace WPF1._5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private bool _isRectDragInProg;
        public RectObject obj;


        public MainWindow()
        {
            InitializeComponent();
            Context wrapper = Context.getInstance();
            wrapper.mw = this;
            this.DataContext = wrapper.ct;

            LoginForm win = new LoginForm();          //otvara prvo login

            Table.DataContext = wrapper.ct;
            TableNode.DataContext = wrapper.ct;
            TableTerminals.DataContext = wrapper.ct;
            TableLines.DataContext = wrapper.ct;

            obj = new RectObject();
            win.ShowDialog();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // ... Get TabControl reference.
                var item = sender as TabControl;
                // ... Set Title to selected tab header.
                var selected = item.SelectedItem as TabItem;
                this.Title = selected.Header.ToString();
            }
            catch
            {

            }
        } //title na vrhu

        public void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e = null)
        {
            Context wrap = Context.getInstance();

            if (e.OriginalSource is Canvas)
            {
                Canvas can = (Canvas) e.OriginalSource;
                var mousePos = e.GetPosition(MainCanvas);
                foreach (Ellipse rect in can.Children)
                {
                    double globalx = Canvas.GetLeft(can);
                    double globaly = Canvas.GetTop(can);
                    double x = Canvas.GetLeft(rect) + globalx;
                    double y = Canvas.GetTop(rect) + globaly;

                    EES.IdentifiedObject io = new EES.IdentifiedObject();
                    io = (EES.IdentifiedObject) rect.Tag;
                    Point pt = new Point(x + 10, y + 10);
                    foreach (Lines ls in wrap.ct.lines)
                    {
                        if (ls.uid1.CompareTo(io.id) == 0)
                        {
                            foreach (Line line in ls.linije)
                            {
                                line.X1 = pt.X;
                                line.Y1 = pt.Y;
                            }
                            //break;
                        }
                        else if (ls.uid2.CompareTo(io.id) == 0)
                        {
                            foreach (Line line in ls.linije)
                            {
                                line.X2 = pt.X;
                                line.Y2 = pt.Y;
                            }
                            //break;
                        }
                    }
                    io.point = pt;
                    rect.Tag = io;
                }

                double left = mousePos.X - (can.ActualWidth/2);
                double top = mousePos.Y - (can.ActualHeight/2);
                obj.RectObjectNew(can, top, left);
                wrap.ct.UndoStack.Push(obj);                 //ubaci pomeranje objekta u undo stack
                _isRectDragInProg = false;
                can.ReleaseMouseCapture();
            }
        }

        private void rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (e.OriginalSource is Canvas)
            {
                Canvas can = (Canvas) e.OriginalSource;
                var mousePos = e.GetPosition(MainCanvas);

                obj = new RectObject();
                double left = mousePos.X - (can.ActualWidth/2);
                double top = mousePos.Y - (can.ActualHeight/2);
                obj.RectObjectOld(can, top, left);
                _isRectDragInProg = true;
                can.CaptureMouse();
            }
        }

       private void rect_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isRectDragInProg) return;
            var mousePos = e.GetPosition(MainCanvas);

            if (e.OriginalSource is Canvas)
            {
                Canvas can = (Canvas) e.OriginalSource;

                double left = mousePos.X - (can.ActualWidth/2);
                double top = mousePos.Y - (can.ActualHeight/2);
                if (left < 0 || top < 0)
                {
                    return;
                }

                if (left + 70 > 530 || top + 70 > 350)
                {
                    return;
                }

                Canvas.SetLeft(can, left);
                Canvas.SetTop(can, top);
            }
        }

        private void RadioSub_Checked(object sender, RoutedEventArgs e)
        {
            Context wrap = Context.getInstance();
            CloneBox.ItemsSource = wrap.ct.subsList;
            wrap.ct.RadioFlag = "substation";
        }

        private void RadioNode_Checked(object sender, RoutedEventArgs e)
        {
            Context wrap = Context.getInstance();
            CloneBox.ItemsSource = wrap.ct.nodeList;
            wrap.ct.RadioFlag = "node";
        }

        private void RadioLine_Checked(object sender, RoutedEventArgs e)
        {
            Context wrap = Context.getInstance();
            CloneBox.ItemsSource = wrap.ct.lineList;
            wrap.ct.RadioFlag = "line";
        }


    }
}

using CIM.IEC61970.Base.Core;
using CIM.IEC61970.Base.Wires;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using WPF1._5.EES;

namespace WPF1._5
{
    public class ViewModel : INotifyPropertyChanged
    {

        #region Main Fields
        public ObservableCollection<Lines> lines { get; set; }
        public ObservableCollection<ConnectivityNode> pomListLeft { get; set; }     //pomocne liste za prebacivanje levo-desno(addSubstation.win)
        public ObservableCollection<ConnectivityNode> pomListRight { get; set; }

        public Stack<object> UndoStack;     //undo i redo
        public Stack<object> RedoStack;
        public ObservableCollection<ACLineSegment> lineList { get; set; }       //Database
        public ObservableCollection<Terminal> terminalList { get; set; }
        public ObservableCollection<ConnectivityNode> nodeList { get; set; }
        public ObservableCollection<BaseVoltage> BaseVolList { get; set; }
        public ObservableCollection<Substation> subsList { get; set; }
        public Collection<UIElement> UIElements { get; set; }

        public string RadioFlag = String.Empty;

        private string username;            //pomocna polja
        private string password;

        private int selectedIndex;
        private string typeItem;

        private string substationName = String.Empty;


        private string nodeName = String.Empty;
        private string bindToSub;
        private int bindToSubIdx;
        private string nodeVol;
        private int nodeVolIdx;

        private string baseVoltage = String.Empty;
        private string baseVoltageName = String.Empty;
        private double xpos = 0;
        private double ypos = 0;

        private Random rand = new Random();

        #endregion

        public ViewModel()  
        {
            lines = new ObservableCollection<Lines>();
            pomListRight = new ObservableCollection<ConnectivityNode>();
            pomListLeft = new ObservableCollection<ConnectivityNode>();
            lineList = new ObservableCollection<ACLineSegment>();
            terminalList = new ObservableCollection<Terminal>();
            BaseVolList = new ObservableCollection<BaseVoltage>();
            nodeList = new ObservableCollection<ConnectivityNode>();
            subsList = new ObservableCollection<Substation>();
            UIElements = new Collection<UIElement>();
            UndoStack = new Stack<object>();
            RedoStack = new Stack<object>();
        }

        #region On Property Change Fields
        public string BaseVoltageName
        {
            get
            {
                return baseVoltageName;
            }

            set
            {
                baseVoltageName = value;
                OnPropertyChanged("BaseVoltageName");
            }
        }

        public string BaseVoltage
        {
            get
            {
                return baseVoltage;
            }

            set
            {
                baseVoltage = value;
                OnPropertyChanged("BaseVoltage");
            }
        }

        public int NodeVolIdx
        {
            get { return nodeVolIdx; }
            set
            {
                nodeVolIdx = value;
                OnPropertyChanged("NodeVolIdx");
            }
        }

        public int BindToSubIdx
        {
            get { return bindToSubIdx; }
            set
            {
                bindToSubIdx = value;
                OnPropertyChanged("BindToSubIdx");
            }
        }

        public string NodeVol
        {
            get
            {
                return nodeVol;
            }

            set
            {
                nodeVol = value;
                OnPropertyChanged("NodeVol");
            }
        }

        public string BindToSub
        {
            get
            {
                return bindToSub;
            }

            set
            {
                bindToSub = value;
                OnPropertyChanged("BindToSub");
            }
        }

        public string NodeName
        {
            get
            {
                return nodeName;
            }

            set
            {
                nodeName = value;
                OnPropertyChanged("NodeName");
            }
        }

        public string SubstationName
        {
            get
            {
                return substationName;
            }

            set
            {
                substationName = value;
                OnPropertyChanged("SubstationName");
            }
        }


        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }

        public string TypeItem
        {
            get { return typeItem; }
            set
            {
                typeItem = value;
                OnPropertyChanged("TypeItem");
            }
        }
        public string Username
        {
            get
            {
                return username;
            }

            set
            {
                username = value;
                OnPropertyChanged("Username");
            }
        }

        public string Password
        {
            get
            {
                return password;
            }

            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion                

        #region Private Commands
        private ICommand close;
        private ICommand login;
        private ICommand choose;
        private ICommand closeWindow;
        private ICommand addSubstation;
        private ICommand addNode;
        private ICommand saveCommand;
        private ICommand loadCommand;
        private ICommand addBaseVoltage;
        private ICommand rightToLeft;
        private ICommand leftToRight;
        private ICommand undoCommand;
        private ICommand redoCommand;
        private ICommand addLine;
        private ICommand cloneDeleteCommand;
        #endregion

        #region Public Commands
        public ICommand CloneDeleteCommand
        {
            get
            {
                return cloneDeleteCommand ?? (cloneDeleteCommand = new RelayCommand(param => CloneDeleteCommandExecution(param)));
            }
        }

        public ICommand AddLine
        {
            get
            {
                return addLine ?? (addLine = new RelayCommand(param => this.AddLineCommandExecution(param)));
            }
        }

        public ICommand RedoCommand
        {
            get
            {
                return redoCommand ?? (redoCommand = new RelayCommand(param => this.RedoCommandExecution(param)));
            }
        }

        public ICommand UndoCommand
        {
            get
            {
                return undoCommand ?? (undoCommand = new RelayCommand(param => this.UndoCommandExecution(param)));
            }
        }

        public ICommand LeftToRight
        {
            get
            {
                return leftToRight ?? (leftToRight = new RelayCommand(param => this.LeftToRightExecution(param)));
            }
        }

        public ICommand RightToLeft
        {
            get
            {
                return rightToLeft ?? (rightToLeft = new RelayCommand(param => this.RightToLeftExecution(param)));
            }
        }

        public ICommand AddBaseVoltage
        {
            get
            {
                return addBaseVoltage ?? (addBaseVoltage = new RelayCommand(param => this.AddBaseVoltageExecution(param)));
            }
        }

        public ICommand LoadCommand
        {
            get
            {
                return loadCommand ?? (loadCommand = new RelayCommand(param => this.LoadFromFile(param)));
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new RelayCommand(param => this.SaveToFile(param)));
            }
        }

        public ICommand AddNode
        {
            get
            {
                return addNode ?? (addNode = new RelayCommand(param => this.AddNodeCommandExecution(param)));
            }
        }

        public ICommand ChooseCommand
        {
            get
            {
                return choose ?? (choose = new RelayCommand(param => this.ChooseCommandExecution(param)));
            }
        }

        public ICommand LoginCommand
        {
            get
            {
                return login ?? (login = new RelayCommand(param => this.LoginCommandExecution(param)));
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return close ?? (close = new RelayCommand(param => this.CloseApplication(param)));
            }
        }

        public ICommand CloseWindow
        {
            get
            {
                return closeWindow ?? (closeWindow = new RelayCommand(param => this.CloseWindowExecution(param)));
            }
        }
        public ICommand AddSubstationCommand
        {
            get
            {
                return addSubstation ?? (addSubstation = new RelayCommand(param => this.AddSubstationCommandExecute(param)));
            }
        }

        #endregion

        #region CommandExecution (Reciever)
        private void CloseApplication(object param)
        {
            Environment.Exit(0);
        }       //zatvaranje aplikacije

        private void CloseWindowExecution(object param)
        {
            Context wrap = Context.getInstance();
            wrap.subwin.Close();
        }

        private void LoginCommandExecution(object param)
        {
            Context wrapper = Context.getInstance();
            string filePass = String.Empty;

            if (String.IsNullOrEmpty(Password) || String.IsNullOrEmpty(Username))
            {
                MessageBox.Show(wrapper.subwin, "Username or Password can not be empty", "Error");
                return;
            }

            User session = UserSession.getInstance();
            session.setUserPass(Username, Password);
            bool registered = false;

            if (!File.Exists(@"..\..\..\" + Username + ".xml"))
            {
                XDocument doc = new XDocument(new XElement("User",
                                                new XElement("Username", Username),
                                                new XElement("Password", Password)));
                doc.Save(@"..\..\..\" + Username + ".xml");
                registered = true;
            }
            else
            {
                using (XmlReader reader = XmlReader.Create(@"..\..\..\" + Username + ".xml"))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "Password":
                                    if (reader.Read())
                                    {
                                        filePass = reader.Value.Trim();
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            if (Password != filePass)
            {
                if (registered)
                {
                    MessageBox.Show("Registered user: " + session.getUser());
                }
                else
                {
                    MessageBox.Show(wrapper.subwin, "Wrong password for selected username", "Error");
                    return;
                }
            }
            wrapper.subwin.Close();
        }       //login usera

        private void ChooseCommandExecution(object param)
        {
            Context wrapper = Context.getInstance();
            if (SelectedIndex > -1)
            {
                if (TypeItem.Contains("Substation"))
                {
                    AddSubstation subWin = new AddSubstation();
                    subWin.SubRight.ItemsSource = BaseVolList;          //prikazi listu svih voltage desno
                    subWin.ShowDialog();
                }

                if (TypeItem.Contains("Connectivity Node"))
                {
                    AddNode subWin = new AddNode();
                    subWin.BindToSub.ItemsSource = subsList;         //prikazi listu svih substationa
                    subWin.ShowDialog();
                }

                if (TypeItem.Contains("Base Voltage"))
                {
                    AddBaseVoltage subwin = new AddBaseVoltage();
                    subwin.ShowDialog();
                }

                if (TypeItem.Contains("AC Line Segment"))
                {
                    AddLine subwin = new AddLine();
                    subwin.ShowDialog();
                }

            }
        }      //biranje objekta za dodavanje

        private void AddBaseVoltageExecution(object param)
        {
            if (String.IsNullOrWhiteSpace(BaseVoltage.Trim().ToString()))
            {
                MessageBox.Show("Base voltage can not be empty");
                return;
            }

            if (String.IsNullOrWhiteSpace(BaseVoltageName.Trim().ToString()))
            {
                MessageBox.Show("Name of the base voltage can not be empty");
                return;
            }

            try
            {
                float bvol = float.Parse(BaseVoltage);
                BaseVoltage voltage = new BaseVoltage();

                voltage.nominalVoltage = bvol;
                voltage.name = BaseVoltageName;
                voltage.mRID = Guid.NewGuid().ToString();

                foreach (BaseVoltage bv in BaseVolList)
                {
                    if (bv.name.CompareTo(BaseVoltageName) == 0)
                    {
                        MessageBox.Show("Base voltage with that name already exists");
                        return;
                    }

                    if (bv.nominalVoltage == bvol)
                    {
                        MessageBox.Show("Base voltage with same value already exists");
                        return;
                    }
                }
                BaseVolList.Add(voltage);
                CloseWindowExecution(null);
            }
            catch
            {
                MessageBox.Show("Base voltage must be a number");
            }
        }       

        private void AddSubstationCommandExecute(object param)
        {
            bool quit = false;
            Context wrap = Context.getInstance();

            if (((AddSubstation)wrap.subwin).SubLeft.Items.IsEmpty)      //da li je prazna lista voltage-a
            {
                MessageBox.Show("Substation must have voltage level");     
                return;
            }

            if (String.IsNullOrWhiteSpace(SubstationName.Trim().ToString()))    
            {
                MessageBox.Show("Substation must have a name");
                return;
            }

            foreach (Substation substation in subsList)
            {
                if (String.Compare(substation.name, SubstationName) == 0)   //da li vec postoji ime
                {
                    quit = true;
                    break;
                }
                else
                {
                    quit = false;
                }
            }

            if (quit)
            {
                MessageBox.Show("Substation with that name already exists");
                return;
            }

            try
            {
                Guid UID = Guid.NewGuid();

                Substation sub = new Substation();  //novi miniCanvas sa tim atributima
                Canvas rect = new Canvas();
                rect.Background = Brushes.DodgerBlue;
                rect.Width = 75;
                rect.Height = 90;
                rect.Uid = UID.ToString();
                rect.Name = SubstationName;
                sub.name = SubstationName;
                sub.mRID = UID.ToString();
                rect.Tag = AddVoltageLevels(sub);
                rect.ToolTip = SubstationName;

                Canvas.SetLeft(rect, 0);    //top left
                Canvas.SetTop(rect, 0);
                ((MainWindow)wrap.mw).MainCanvas.Children.Add(rect);

                UIElements.Add(rect);           //u liste i stackove
                subsList.Add(sub);
                SubstationName = String.Empty;
                UndoStack.Push(sub);
                UndoStack.Push(rect);
                CloseWindowExecution(null);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }

        }

        private string AddVoltageLevels(Substation substation)
        {
            Context wrap = Context.getInstance();
            substation.VoltageLevels = new List<VoltageLevel>();
            foreach (BaseVoltage bv in ((AddSubstation)wrap.subwin).SubLeft.Items)
            {
                VoltageLevel vl = new VoltageLevel();
                vl.BaseVoltage = bv;
                substation.VoltageLevels.Add(vl);
            }

            return substation.VoltageLvls;
        }       //dodavanje voltage levela u substation

        private void AddNodeCommandExecution(object param)
        {
            if (String.IsNullOrWhiteSpace(NodeName.Trim().ToString()))
            {
                MessageBox.Show("Name of the node can not be empty");
                return;
            }

            if (NodeVolIdx == -1)
            {
                MessageBox.Show("Node must have voltage");
                return;
            }

            if (BindToSubIdx == -1)
            {
                MessageBox.Show("Node must belong to substation");
                return;
            }

            try
            {
                int index = BindToSub.IndexOf("(");
                string subsName = BindToSub.Substring(0, index);    //naziv i index substationa
                string subsVol = BindToSub.Substring(index);


                if (!subsVol.Contains(NodeVol))     //provere
                {
                    MessageBox.Show("Selected substation does not support node with this voltage level");
                    return;
                }

                foreach (Substation subs in subsList)
                {
                    if (subs.name.CompareTo(subsName.Trim()) == 0)
                    {
                        foreach (ConnectivityNode node in subs.ConnectivityNodes)
                        {
                            if (node.name.CompareTo(NodeName) == 0)
                            {
                                MessageBox.Show("Node with that name already exists in selected substation");
                                return;
                            }
                        }
                    }
                }

                Context wrap = Context.getInstance();
                var canvas = ((MainWindow)wrap.mw).MainCanvas;

                IEnumerable<Canvas> canvases = canvas.Children.OfType<Canvas>(); //uzmi sve canvase

                Ellipse ell = new Ellipse(); 

                foreach (var cc in canvases) 
                {
                    if (cc.Name.CompareTo(subsName.Trim()) == 0)    //prvo cvor na canvas sub-a
                    {
                        ell.Fill = Brushes.Black;
                        ell.Width = 20;
                        ell.Height = 20;
                        ell.Uid = cc.Uid;
                        ell.Name = NodeName;
                        ell.ToolTip = NodeName;

                        EES.IdentifiedObject io = new EES.IdentifiedObject();
                        Point point = new Point(Canvas.GetLeft(cc) + xpos + 10, Canvas.GetTop(cc) + ypos + 10);
                        io.point = point;
                        io.id = Guid.NewGuid().ToString();
                        ell.Tag = io;
                        Random rnd = new Random();
                        Canvas.SetLeft(ell, xpos);
                        Canvas.SetTop(ell, ypos);
                        xpos = rnd.Next(55);
                        ypos = rnd.Next(70);
                        cc.Children.Add(ell);
                    }
                }

                foreach (Substation subs in subsList)       //pa u sub 
                {
                    if (subs.name.CompareTo(subsName.Trim()) == 0)
                    {
                        Context wrapp = Context.getInstance();
                        ConnectivityNode node = new ConnectivityNode();
                        node.name = NodeName;
                        node.mRID = Guid.NewGuid().ToString();
                        string bv = ((AddNode)wrap.subwin).BindToBaseVol.SelectedItem.ToString();
                        float bvv = float.Parse(bv, CultureInfo.InvariantCulture.NumberFormat);
                        foreach (BaseVoltage bvol in BaseVolList)
                        {
                            if (bvol.nominalVoltage == bvv)
                            {
                                node.m_BaseVoltage = bvol;
                                break;
                            }
                        }
                        subs.ConnectivityNodes.Add(node);
                        nodeList.Add(node);
                        UndoStack.Push(node);
                        break;
                    }
                }
                UndoStack.Push(ell);
                CloseWindowExecution(null);
                return;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                return;
            }
        }

        private void AddLineCommandExecution(object param)
        {
            Context wrap = Context.getInstance();


            string subsNameLeft = String.Empty;
            string subsNameRight = String.Empty;
            string nodeNameLeft = String.Empty;
            string nodeNameRight = String.Empty;

            if (param == null)
            {
                ACLineSegment acline = new ACLineSegment();
                AddLine win = ((AddLine)wrap.subwin);
                if (win.LeftSub.SelectedIndex == -1)
                {
                    MessageBox.Show("Select substation");
                    return;
                }

                if (win.RightSub.SelectedIndex == -1)
                {
                    MessageBox.Show("Select substation");
                    return;
                }

                if (win.LeftNode.SelectedIndex == -1)
                {
                    MessageBox.Show("Select node");
                    return;
                }

                if (win.RightNode.SelectedIndex == -1)
                {
                    MessageBox.Show("Select node");
                    return;
                }

                if (win.LeftSub.SelectedItem.ToString().CompareTo(win.RightSub.SelectedItem.ToString()) == 0)
                {
                    MessageBox.Show("Nodes can not be wired inside the same substaion");
                    return;
                }

                int index = win.LeftSub.SelectedItem.ToString().IndexOf("(");                   //levi sub
                subsNameLeft = win.LeftSub.SelectedItem.ToString().Substring(0, index);
                subsNameLeft = subsNameLeft.Trim();

                index = win.RightSub.SelectedItem.ToString().IndexOf("(");
                subsNameRight = win.RightSub.SelectedItem.ToString().Substring(0, index);        //desni sub
                subsNameRight = subsNameRight.Trim();

                index = win.LeftNode.SelectedItem.ToString().IndexOf(" ");
                nodeNameLeft = win.LeftNode.SelectedItem.ToString().Substring(0, index);
                string nodevalueLeft = win.LeftNode.SelectedItem.ToString().Substring(index + 1);    //levi cvor
                nodeNameLeft = nodeNameLeft.Trim();
                double leftnode = 0;
                Double.TryParse(nodevalueLeft, out leftnode);

                index = win.RightNode.SelectedItem.ToString().IndexOf(" ");
                nodeNameRight = win.RightNode.SelectedItem.ToString().Substring(0, index);
                string nodevalueRight = win.RightNode.SelectedItem.ToString().Substring(index + 1);  //desni cvor
                double rightnode = 0;
                Double.TryParse(nodevalueRight, out rightnode);
                nodeNameRight = nodeNameRight.Trim();

                if (rightnode != leftnode)
                {
                    MessageBox.Show("Nodes with different base voltage can not be connected");
                    return;
                }

                Terminal terLeft = new Terminal();
                Terminal terRight = new Terminal();     //vod ima 2 terminala

                bool alreadyAddedL = false;
                bool alreadyAddedR = false;

                foreach (Substation sub in subsList)
                {
                    if (sub.name.CompareTo(subsNameLeft) == 0)
                    {
                        foreach (ConnectivityNode node in sub.ConnectivityNodes)    //levi cvor
                        {
                            if (node.name.CompareTo(nodeNameLeft) == 0)
                            {
                                if (node.Terminals.Count > 0)
                                {
                                    terLeft = (Terminal)node.Terminals[0].Clone();  //ako ima, kloniraj samo
                                    alreadyAddedL = true;
                                    break;
                                }
                                terLeft.mRID = node.mRID;                       //inace, napravi novi, ID-node.id
                                terLeft.connected = true;
                                terLeft.name = "Terminal" + node.name;      
                                terLeft.ConductingEquipment = acline;
                                terLeft.ACline = acline;
                                node.Terminals.Add(terLeft);
                                acline.Terminals.Add(terLeft);
                                break;
                            }
                        }
                    }
                    else if (sub.name.CompareTo(subsNameRight) == 0)        //isto i za desni
                    {
                        foreach (ConnectivityNode node in sub.ConnectivityNodes)
                        {
                            if (node.name.CompareTo(nodeNameRight) == 0)
                            {
                                if (node.Terminals.Count > 0)
                                {
                                    terRight = (Terminal)node.Terminals[0].Clone();
                                    alreadyAddedR = true;
                                    break;
                                }

                                terRight.mRID = node.mRID;
                                terRight.connected = true;
                                terRight.name = "Terminal" + node.name;
                                terRight.ConductingEquipment = acline;
                                terRight.ACline = acline;
                                node.Terminals.Add(terRight);
                                acline.Terminals.Add(terRight);
                                break;
                            }
                        }
                    }
                }

                if (!alreadyAddedL)
                {
                    terminalList.Add(terLeft);
                }

                if (!alreadyAddedR)
                {
                    terminalList.Add(terRight);
                }

                var canvas = ((MainWindow)wrap.mw).MainCanvas;
                IEnumerable<Canvas> canvases = canvas.Children.OfType<Canvas>(); //svi canvasi

                Line linija = new Line();               //nova linija
                linija.Stroke = Brushes.Maroon;
                linija.StrokeThickness = 2;
                Lines Lines = new Lines();              

                foreach (Canvas cc in canvases)
                {
                    if (cc.Name.CompareTo(subsNameLeft) == 0)
                    {
                        foreach (Ellipse rect in cc.Children)
                        {
                            if (rect.Name.CompareTo(nodeNameLeft) == 0)     //krece od koordinata levog cvora
                            {
                                EES.IdentifiedObject io = (EES.IdentifiedObject)rect.Tag;
                                Point pt = io.point;
                                Lines.uid1 = io.id;
                                linija.X1 = pt.X;
                                linija.Y1 = pt.Y;
                                acline.StartNode_X = pt.X;  
                                acline.StartNode_Y = pt.Y;
                                linija.Uid = io.id;
                                acline.mRID = io.id;
                                acline.name = rect.Name;
                                linija.Name = rect.Name;
                                break;
                            }
                        }
                    }
                    else if (cc.Name.CompareTo(subsNameRight) == 0)     //do desnog
                    {
                        foreach (Ellipse rect in cc.Children)
                        {
                            if (rect.Name.CompareTo(nodeNameRight) == 0)
                            {
                                EES.IdentifiedObject io = (EES.IdentifiedObject)rect.Tag;
                                Point pt = io.point;
                                Lines.uid2 = io.id;
                                linija.X2 = pt.X;
                                linija.Y2 = pt.Y;
                                acline.EndNode_X = pt.X;
                                acline.EndNode_Y = pt.Y;
                                linija.Uid += io.id;
                                acline.mRID += " " + io.id;
                                acline.name += " " + rect.Name;
                                linija.Name += "_" + rect.Name;
                                break;
                            }
                        }
                    }
                }
                linija.ToolTip = linija.Name;

                ((MainWindow)wrap.mw).MainCanvas.Children.Add(linija);
                Lines.linije.Add(linija);
                lines.Add(Lines);

                lineList.Add(acline);

                CloseWindowExecution(null);
            }
            else            //ako vec postoje terminali, parametar nece biti null
            {
                List<Terminal> paramTerm = (List<Terminal>)param;   //lista terminala
                var canvas = ((MainWindow)wrap.mw).MainCanvas;
                IEnumerable<Canvas> canvases = canvas.Children.OfType<Canvas>(); //svi kanvasi

                string terid1 = String.Empty;
                string terid2 = String.Empty;
                string tername1 = String.Empty;
                string tername2 = String.Empty;
                bool found = false;

                for (int i = 0; i < paramTerm.Count - 1; i++)
                {
                    for (int j = i + 1; j < paramTerm.Count; j++)
                    {
                        string[] ids1 = paramTerm[i].ACline.mRID.Split(' ');    //posto je GUID - ID cvora
                        string[] ids2 = paramTerm[j].ACline.mRID.Split(' ');

                        if (paramTerm[i].ACline.mRID.CompareTo((paramTerm[j].ACline.mRID)) == 0 ||
                            ids1[0].CompareTo(ids2[0]) == 0 ||
                            ids1[0].CompareTo(ids2[1]) == 0 ||
                            ids1[1].CompareTo(ids2[0]) == 0 ||
                            ids1[1].CompareTo(ids2[1]) == 0)     //razdvoj po " " pa check
                        {
                            terid1 = paramTerm[i].mRID;
                            terid2 = paramTerm[j].mRID;
                            tername1 = paramTerm[i].name;
                            tername2 = paramTerm[j].name;           //onda ih samo nadji
                            found = true;
                        }

                        if (found)
                        {
                            ACLineSegment acline = new ACLineSegment();     //i dodaj liniju
                            Line linija = new Line();
                            linija.Stroke = Brushes.Maroon;
                            linija.StrokeThickness = 2;
                            Lines Lines = new Lines();
                            found = false;
                            foreach (Canvas cc in canvases)
                            {
                                foreach (Ellipse ell in cc.Children)
                                {
                                    if (tername1.Contains(ell.Name))
                                    {
                                        EES.IdentifiedObject io = (EES.IdentifiedObject)ell.Tag;
                                        Point pt = io.point;
                                        Lines.uid1 = io.id;
                                        linija.X1 = pt.X;
                                        linija.Y1 = pt.Y;
                                        acline.StartNode_X = pt.X;
                                        acline.StartNode_Y = pt.Y;
                                        linija.Uid = io.id;
                                        acline.mRID = io.id;
                                        acline.name = ell.Name;
                                        linija.Name = ell.Name;
                                        break;
                                    }
                                    else if (tername2.Contains(ell.Name))
                                    {
                                        EES.IdentifiedObject io = (EES.IdentifiedObject)ell.Tag;
                                        Point pt = io.point;
                                        Lines.uid2 = io.id;
                                        linija.X2 = pt.X;
                                        linija.Y2 = pt.Y;
                                        acline.EndNode_X = pt.X;
                                        acline.EndNode_Y = pt.Y;
                                        linija.Uid += io.id;
                                        acline.mRID += " " + io.id;
                                        acline.name += " " + ell.Name;
                                        linija.Name += "_" + ell.Name;
                                        break;
                                    }
                                }
                            }

                            ((MainWindow)wrap.mw).MainCanvas.Children.Add(linija);
                            Lines.linije.Add(linija);
                            lines.Add(Lines);
                            lineList.Add(acline);
                        }
                    }
                }

                //CloseWindowExecution(null);
            }
        }       

        private void UndoCommandExecution(object param)
        {
            Context wrapper = Context.getInstance();
            Canvas mainCanvas = ((MainWindow)wrapper.mw).MainCanvas;
            if (UndoStack.Count > 0)
            {
                if (UndoStack.Peek() is Canvas)                             //undo substationa
                {
                    Canvas canvas = (Canvas)UndoStack.Pop();
                    mainCanvas.Children.Remove(canvas);
                    Substation sub = (Substation)UndoStack.Pop();
                    subsList.Remove(sub);
                    RedoStack.Push(sub);
                    RedoStack.Push(canvas);
                }
                else if (UndoStack.Peek() is Ellipse)
                {
                    IEnumerable<Canvas> canvases = mainCanvas.Children.OfType<Canvas>(); //undo cvora
                    Ellipse rect = (Ellipse)UndoStack.Pop();
                    foreach (Canvas cc in canvases)
                    {
                        if (cc.Children.Contains(rect))
                        {
                            cc.Children.Remove(rect);
                            break;
                        }
                    }
                    ConnectivityNode node = (ConnectivityNode)UndoStack.Pop();
                    nodeList.Remove(node);
                    RedoStack.Push(node);
                    RedoStack.Push(rect);
                }
                else if (UndoStack.Peek() is RectObject)                //undo pomeranja
                {
                    RectObject obj = (RectObject)UndoStack.Pop();
                    Canvas.SetLeft(obj.canvas, obj.old_left);
                    Canvas.SetTop(obj.canvas, obj.old_top);

                    RedoStack.Push(obj);

                }
                else if (UndoStack.Peek() is CloneDeleteObject)                               //undo kloniranog objekta
                {
                    CloneDeleteObject obj = (CloneDeleteObject)UndoStack.Pop();
                    if (!obj.deleted) //ako je kloniran
                    {
                        if (obj.sub != null) //substation
                        {
                            ((MainWindow)wrapper.mw).MainCanvas.Children.Remove(obj.UISub);
                            subsList.Remove(obj.sub);
                            foreach (ConnectivityNode node in obj.nodes)
                            {
                                nodeList.Remove(node);
                            }
                            RedoStack.Push(obj);
                        }
                        else        //cvor
                        {
                            obj.UISub.Children.Remove(obj.UINodes[0]);
                            nodeList.Remove(obj.nodes[0]);
                            RedoStack.Push(obj);
                        }
                    }
                    else //ako je obrisan                              undo obrisanog objekta
                    {
                        //substation
                        if (obj.call == CloneDeleteObject.Called.Substation)
                        {
                            ((MainWindow)wrapper.mw).MainCanvas.Children.Add(obj.UISub);
                            subsList.Add(obj.sub);
                            foreach (ConnectivityNode node in obj.nodes)
                            {
                                nodeList.Add(node);
                                int getCount = obj.nodes.Count();
                                for (int i = 0; i < getCount; i++)
                                {
                                    foreach (Terminal ter in obj.nodes[i].Terminals.ToList())
                                    {
                                        terminalList.Add(ter);
                                    }
                                }
                            }

                            foreach (Line line in obj.UIlines)
                            {
                                mainCanvas.Children.Add(line);
                            }

                            foreach (ACLineSegment acline in obj.lines)
                            {
                                lineList.Add(acline);
                            }

                            RedoStack.Push(obj);

                        }
                        // cvor
                        else if (obj.call == CloneDeleteObject.Called.Node)
                        {
                            obj.UISub.Children.Add(obj.UINodes[0]);
                            nodeList.Add(obj.nodes[0]);
                            foreach (Terminal ter in obj.nodes[0].Terminals)
                            {
                                terminalList.Add(ter);
                            }

                            foreach (Line line in obj.UIlines)
                            {
                                mainCanvas.Children.Add(line);
                            }

                            foreach (ACLineSegment acline in obj.lines)
                            {
                                lineList.Add(acline);
                            }

                            RedoStack.Push(obj);
                        }
                        else //linija
                        {
                            if (obj.onlyOneLine)
                            {
                                ((MainWindow)wrapper.mw).MainCanvas.Children.Add(obj.UIlines[0]);
                                lineList.Add(obj.lines[0]);
                                RedoStack.Push(obj);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No more actions to undo");
            }
        }       //UNDO

        private void RedoCommandExecution(object param)
        {
            Context wrapper = Context.getInstance();
            Canvas mainCanvas = ((MainWindow)wrapper.mw).MainCanvas;

            if (RedoStack.Count > 0)
            {
                if (RedoStack.Peek() is Canvas)             //redo substationa
                {
                    Canvas canvas = (Canvas)RedoStack.Pop();
                    mainCanvas.Children.Add(canvas);
                    Substation sub = (Substation)RedoStack.Pop();
                    subsList.Add(sub);
                    UndoStack.Push(sub);
                    UndoStack.Push(canvas);
                }
                else if (RedoStack.Peek() is Ellipse)           // cvora
                {
                    IEnumerable<Canvas> canvases = mainCanvas.Children.OfType<Canvas>(); //get all canvases
                    Ellipse rect = (Ellipse)RedoStack.Pop();
                    foreach (Canvas cc in canvases)
                    {
                        if (cc.Uid.CompareTo(rect.Uid) == 0)
                        {
                            cc.Children.Add(rect);
                            break;
                        }
                    }
                    ConnectivityNode node = (ConnectivityNode)RedoStack.Pop();
                    nodeList.Add(node);
                    UndoStack.Push(node);
                    UndoStack.Push(rect);
                }
                else if (RedoStack.Peek() is RectObject)        //redo pomeranja
                {
                    RectObject obj = (RectObject)RedoStack.Pop();
                    Canvas.SetLeft(obj.canvas, obj.new_left);
                    Canvas.SetTop(obj.canvas, obj.new_top);
                    UndoStack.Push(obj);
                }
                else if (RedoStack.Peek() is CloneDeleteObject)     //kloniranog objekta
                {
                    CloneDeleteObject obj = (CloneDeleteObject)RedoStack.Pop();
                    if (!obj.deleted) //cloned
                    {
                        if (obj.sub != null) //substation
                        {
                            ((MainWindow)wrapper.mw).MainCanvas.Children.Add(obj.UISub);

                            foreach (ConnectivityNode node in obj.nodes)
                            {
                                nodeList.Add(node);
                            }

                            subsList.Add(obj.sub);
                            UndoStack.Push(obj);
                        }
                        else //cvor
                        {
                            obj.UISub.Children.Add(obj.UINodes[0]);
                            nodeList.Add(obj.nodes[0]);
                            UndoStack.Push(obj);
                        }
                    }
                    else //delete redo
                    {
                        if (obj.call == CloneDeleteObject.Called.Substation)
                        {
                            ((MainWindow)wrapper.mw).MainCanvas.Children.Remove(obj.UISub);
                            subsList.Remove(obj.sub);
                            foreach (ConnectivityNode node in nodeList.ToList())
                            {
                                foreach (ConnectivityNode nod in obj.nodes.ToList())
                                {
                                    if (nod.name.CompareTo(node.name) == 0)
                                    {
                                        nodeList.Remove(node);
                                        foreach (Terminal ter in terminalList.ToList())
                                        {
                                            if (ter.name.EndsWith(nod.name))
                                            {
                                                terminalList.Remove(ter);
                                            }
                                        }
                                    }
                                }
                            }

                            foreach (Line line in obj.UIlines)
                            {
                                mainCanvas.Children.Remove(line);
                            }

                            foreach (ACLineSegment acline in obj.lines)
                            {
                                lineList.Remove(acline);
                            }

                            UndoStack.Push(obj);
                        }
                        else if (obj.call == CloneDeleteObject.Called.Node) //cvor
                        {
                            obj.UISub.Children.Remove(obj.UINodes[0]);
                            nodeList.Remove(obj.nodes[0]);
                            foreach (Terminal ter in terminalList.ToList())
                            {
                                if (ter.name.EndsWith(obj.nodes[0].name))
                                {
                                    terminalList.Remove(ter);
                                }
                            }

                            foreach (Line line in obj.UIlines)
                            {
                                mainCanvas.Children.Remove(line);
                            }

                            foreach (ACLineSegment acline in obj.lines)
                            {
                                lineList.Remove(acline);
                            }

                            UndoStack.Push(obj);
                        }
                        else //linija
                        {
                            if (obj.onlyOneLine)
                            {
                                ((MainWindow)wrapper.mw).MainCanvas.Children.Remove(obj.UIlines[0]);
                                lineList.Remove(obj.lines[0]);
                                UndoStack.Push(obj);
                            }
                        }
                    }
                }

            }
            else
            {
                MessageBox.Show("No more actions to redo");
            }
        }       //REDO

        private void CloneDeleteCommandExecution(object param)      //biranje objekta za kloniranje ili brisanje
        {
            if (param == null)
            {
                switch (RadioFlag)
                {
                    case "substation":
                        CloneSubstation();
                        break;

                    case "node":
                        CloneNode();
                        break;

                    case "":
                        MessageBox.Show("Select option to be cloned");
                        return;
                }
            }
            else
            {
                switch (RadioFlag)
                {
                    case "substation":
                        DeleteSubstation();
                        break;

                    case "node":
                        DeleteNode();
                        break;

                    case "line":
                        DeleteLine();
                        break;
                    case "":
                        MessageBox.Show("Select option to be cloned");
                        return;
                }
            }

        }

        private void CloneSubstation()
        {
            CloneDeleteObject CloneObj = new CloneDeleteObject();

            Context wrap = Context.getInstance();
            ComboBox cloneBox = ((MainWindow)wrap.mw).CloneBox;
            string subName = String.Empty;
            try
            {
                int index = cloneBox.SelectedItem.ToString().IndexOf("(");
                subName = cloneBox.SelectedItem.ToString().Substring(0, index);
            }
            catch
            {
                MessageBox.Show("Selection can not be empty");
                return;
            }

            subName = subName.Trim();
            Substation newSub = null;
            foreach (Substation sub in subsList)
            {
                if (sub.name.CompareTo(subName) == 0)
                {
                    newSub = (Substation)sub.Clone();   //prototip pattern
                }
            }

            subsList.Add(newSub);
            CloneObj.sub = newSub;

            Canvas subs = new Canvas();
            subs.Background = Brushes.Aqua;
            subs.Width = 75;
            subs.Height = 90;
            subs.Uid = newSub.mRID;
            subs.Name = newSub.name;
            subs.Tag = newSub.VoltageLvls;
            subs.ToolTip = newSub.name;

            Canvas.SetLeft(subs, 0);
            Canvas.SetTop(subs, 0);
            ((MainWindow)wrap.mw).MainCanvas.Children.Add(subs);
            CloneObj.UISub = subs;

            foreach (ConnectivityNode node in newSub.ConnectivityNodes)
            {
                Ellipse ell = new Ellipse();
                ell.Fill = Brushes.DarkRed;
                ell.Uid = subs.Uid;
                ell.Width = 20;
                ell.Height = 20;
                ell.Name = node.name;
                ell.ToolTip = node.name;

                EES.IdentifiedObject io = new EES.IdentifiedObject();
                Point point = new Point(Canvas.GetLeft(subs) + xpos + 10, Canvas.GetTop(subs) + ypos + 10);
                io.point = point;
                io.id = Guid.NewGuid().ToString();
                ell.Tag = io;
                Random rnd = new Random();
                Canvas.SetLeft(ell, xpos);
                Canvas.SetTop(ell, ypos);
                xpos = rnd.Next(55);
                ypos = rnd.Next(70);
                subs.Children.Add(ell);

                CloneObj.UINodes.Add(ell);

                nodeList.Add(node);
                CloneObj.nodes.Add(node);
            }
            CloneObj.deleted = false;
            UndoStack.Push(CloneObj);
            MessageBox.Show("Substation cloned");
        }

        private void CloneNode()
        {
            Context wrap = Context.getInstance();
            CloneDeleteObject clone = new CloneDeleteObject();
            ComboBox cloneBox = ((MainWindow)wrap.mw).CloneBox;

            string nodeName = String.Empty;
            try
            {
                int index = cloneBox.SelectedItem.ToString().IndexOf(" ");
                nodeName = cloneBox.SelectedItem.ToString().Substring(0, index);
                nodeName = nodeName.Trim();
            }
            catch
            {
                MessageBox.Show("Selection can not be empty");
            }

            ConnectivityNode newNode = null;

            string getSubUid = String.Empty;
            Ellipse ell = new Ellipse();
            foreach (Substation sub in subsList)
            {
                bool found = false;
                foreach (ConnectivityNode node in sub.ConnectivityNodes)
                {
                    if (node.name.CompareTo(nodeName) == 0)
                    {
                        newNode = (ConnectivityNode)node.Clone();       //kloniranje
                        getSubUid = sub.mRID;

                        ell.Fill = Brushes.DarkRed;
                        ell.Uid = sub.mRID;
                        ell.Width = 20;
                        ell.Height = 20;
                        ell.Name = newNode.name;
                        ell.ToolTip = newNode.name;

                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    sub.ConnectivityNodes.Add(newNode);
                    break;
                }
            }
            var canvas = ((MainWindow)wrap.mw).MainCanvas;
            IEnumerable<Canvas> canvases = canvas.Children.OfType<Canvas>(); //svi canvasi
            foreach (Canvas cc in canvases)
            {
                if (cc.Uid.CompareTo(getSubUid) == 0)
                {
                    EES.IdentifiedObject io = new EES.IdentifiedObject();
                    Point point = new Point(Canvas.GetLeft(cc) + xpos + 10, Canvas.GetTop(cc) + ypos + 10);
                    io.point = point;
                    io.id = rand.Next(100).ToString();
                    ell.Tag = io;
                    Random rnd = new Random();
                    Canvas.SetLeft(ell, xpos);
                    Canvas.SetTop(ell, ypos);
                    xpos = rnd.Next(55);
                    ypos = rnd.Next(70);
                    cc.Children.Add(ell);
                    clone.UISub = cc;
                }
            }

            nodeList.Add(newNode);
            MessageBox.Show("Node cloned");

            clone.deleted = false;
            clone.sub = null;
            clone.nodes.Add(newNode);   //dodaj u klonirane nodove
            clone.UINodes.Add(ell);
            UndoStack.Push(clone);

        }

        private void DeleteSubstation()
        {
            CloneDeleteObject delete = new CloneDeleteObject();
            Context wrap = Context.getInstance();
            ComboBox cloneBox = ((MainWindow)wrap.mw).CloneBox;

            string subName = String.Empty;
            try
            {
                int index = cloneBox.SelectedItem.ToString().IndexOf("(");
                subName = cloneBox.SelectedItem.ToString().Substring(0, index);
            }
            catch
            {
                MessageBox.Show("Selection can not be empty");
                return;
            }

            subName = subName.Trim();

            var MCcanvas = ((MainWindow)wrap.mw).MainCanvas;
            IEnumerable<Line> MClines = MCcanvas.Children.OfType<Line>(); //sve linije

            foreach (Substation sub in subsList)
            {
                if (sub.name.CompareTo(subName) == 0)
                {
                    foreach (ConnectivityNode node in sub.ConnectivityNodes)
                    {
                        delete.nodes.Add(node);
                        foreach (Terminal ter in terminalList.ToList())
                        {
                            if (ter.name.EndsWith(node.name))
                            {
                                terminalList.Remove(ter);
                            }
                        }

                        foreach (Line line in MClines.ToList())
                        {
                            bool found = false;
                            string[] nodes = line.Name.Split('_');
                            if (nodes[0].CompareTo(node.name) == 0) //za prvi cvor
                            {
                                MCcanvas.Children.Remove(line);
                                delete.UIlines.Add(line);
                                found = true;
                            }
                            else if (nodes[1].CompareTo(node.name) == 0)
                            {
                                MCcanvas.Children.Remove(line);
                                delete.UIlines.Add(line);
                                found = true;
                            }
                            if (found)
                            {
                                found = false;
                                foreach (ACLineSegment acline in lineList.ToList())
                                {
                                    string lname = nodes[0] + " " + nodes[1];
                                    if (acline.name.CompareTo(lname) == 0)
                                    {
                                        lineList.Remove(acline);
                                        delete.lines.Add(acline);
                                    }
                                }
                            }
                        }
                    }

                    subsList.Remove(sub);
                    delete.sub = sub;
                    break;
                }
            }


            foreach (ConnectivityNode node in nodeList.ToList())
            {
                foreach (ConnectivityNode delnod in delete.nodes.ToList())
                {
                    if (delnod.name.CompareTo(node.name) == 0)
                    {
                        nodeList.Remove(node);
                    }
                }
            }

            var canvas = ((MainWindow)wrap.mw).MainCanvas;
            IEnumerable<Canvas> canvases = canvas.Children.OfType<Canvas>(); //svi canvasi

            foreach (Canvas can in canvases)
            {
                if (can.Name.CompareTo(subName) == 0)
                {
                    foreach (Ellipse ell in can.Children)
                    {
                        delete.UINodes.Add(ell);
                    }

                    canvas.Children.Remove(can);
                    delete.UISub = can;
                    break;
                }
            }

            delete.deleted = true;      //setuj na deleted
            delete.substation = true;
            delete.call = CloneDeleteObject.Called.Substation;
            UndoStack.Push(delete);
            MessageBox.Show("Substation deleted");

        }

        private void DeleteNode()
        {
            CloneDeleteObject delete = new CloneDeleteObject();
            delete.deleted = true;
            delete.call = CloneDeleteObject.Called.Node;

            Context wrap = Context.getInstance();
            CloneDeleteObject clone = new CloneDeleteObject();
            ComboBox cloneBox = ((MainWindow)wrap.mw).CloneBox;
            string nodeName = String.Empty;

            try
            {
                int index = cloneBox.SelectedItem.ToString().IndexOf(" ");
                nodeName = cloneBox.SelectedItem.ToString().Substring(0, index);
                nodeName = nodeName.Trim();
            }
            catch
            {
                MessageBox.Show("Selection can not be empty");
                return;
            }

            string getSubUid = String.Empty;

            foreach (Substation sub in subsList)
            {
                foreach (ConnectivityNode node in sub.ConnectivityNodes)
                {
                    if (node.name.CompareTo(nodeName) == 0)
                    {
                        getSubUid = sub.mRID;
                        sub.ConnectivityNodes.Remove(node);
                        delete.nodes.Add(node);
                        delete.sub = sub;
                        foreach (Terminal ter in terminalList.ToList())
                        {
                            if (ter.name.EndsWith(node.name))
                            {
                                terminalList.Remove(ter);
                            }
                        }
                        break;
                    }
                }
            }

            foreach (ConnectivityNode node in nodeList)
            {
                if (node.name.CompareTo(nodeName) == 0)
                {
                    nodeList.Remove(node);
                    break;
                }
            }

            var canvas = ((MainWindow)wrap.mw).MainCanvas;
            IEnumerable<Canvas> canvases = canvas.Children.OfType<Canvas>(); //svi canvasi


            IEnumerable<Line> MClines = canvas.Children.OfType<Line>().ToList();

            foreach (Canvas can in canvases)
            {
                if (can.Uid.CompareTo(getSubUid) == 0)
                {
                    foreach (Ellipse ell in can.Children)
                    {
                        if (ell.Name.CompareTo(nodeName) == 0)
                        {
                            can.Children.Remove(ell);
                            delete.UINodes.Add(ell);
                            delete.UISub = can;
                            break;
                        }
                    }
                }
            }

            foreach (Line line in MClines.ToList()) //i sve linije obrisi
            {
                bool found = false;
                string[] nodes = line.Name.Split('_');
                if (nodes[0].CompareTo(nodeName) == 0)
                {
                    canvas.Children.Remove(line);
                    found = true;
                    delete.UIlines.Add(line);
                }
                else if (nodes[1].CompareTo(nodeName) == 0)
                {
                    canvas.Children.Remove(line);
                    found = true;
                    delete.UIlines.Add(line);
                }

                if (found)
                {
                    found = false;
                    foreach (ACLineSegment acline in lineList.ToList())
                    {
                        string lname = nodes[0] + " " + nodes[1];
                        if (acline.name.CompareTo(lname) == 0)
                        {
                            lineList.Remove(acline);
                            delete.lines.Add(acline);
                        }
                    }
                }
            }

            UndoStack.Push(delete);
            MessageBox.Show("Node deleted");

        }

        private void DeleteLine()
        {
            CloneDeleteObject delete = new CloneDeleteObject();
            delete.deleted = true;
            delete.call = CloneDeleteObject.Called.Line;
            Context wrap = Context.getInstance();
            CloneDeleteObject clone = new CloneDeleteObject();
            ComboBox cloneBox = ((MainWindow)wrap.mw).CloneBox;

            try
            {
                string lineName = cloneBox.SelectedItem.ToString();
                lineName = lineName.Trim();
                string[] nodes = lineName.Split(' ');
                lineName = nodes[0] + "_" + nodes[1];           //zbog naziva
                var MC = ((MainWindow)wrap.mw).MainCanvas;
                IEnumerable<Line> MClines = MC.Children.OfType<Line>().ToList(); 

                foreach (Line line in MClines.ToList())
                {
                    if (line.Name.CompareTo(lineName) == 0)
                    {
                        MC.Children.Remove(line);
                        delete.UIlines.Add(line);
                        delete.deleted = true;
                        delete.onlyOneLine = true;
                        break;
                    }
                }

                foreach (ACLineSegment acline in lineList.ToList())
                {
                    string[] lns = lineName.Split('_');
                    lineName = lns[0] + " " + lns[1];
                    if (acline.name.CompareTo(lineName) == 0)
                    {
                        lineList.Remove(acline);
                        delete.lines.Add(acline);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Selection can not be empty");
                return;
            }

            UndoStack.Push(delete);

            MessageBox.Show("Line deleted");
        }

        private void SaveToFile(object param)
        {
            Context wrap = Context.getInstance();
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "UIElement File"; // Default file name
            dlg.DefaultExt = ".xaml"; // Default file extension
            dlg.Filter = "Xaml File (.xaml)|*.xaml"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                SerializeToXML((MainWindow)wrap.mw, ((MainWindow)wrap.mw).MainCanvas, 96, dlg.FileName, lines.ToList());

                string filename = dlg.FileName;
                int ind = filename.IndexOf('.');
                filename = filename.Substring(0, ind);
                ind = filename.LastIndexOf('\\');
                filename = filename.Substring(ind + 1);
                try
                {

                    XmlSerializer serializer = new XmlSerializer(typeof(Database));
                    using (TextWriter tw = new StreamWriter(filename, false))
                    {
                        Database data = new Database();
                        data.Substations = subsList;
                        data.Nodes = nodeList;
                        data.BaseVoltages = BaseVolList;
                        serializer.Serialize(tw, data);     //celu bazu serijalizuj, preskocice ono sto sam stavio ignore
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }       //SAVE 

        private void LoadFromFile(object param)
        {
            Context wrap = Context.getInstance();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".xaml"; // Default file extension
            dlg.Filter = "Xaml File (.xaml)|*.xaml"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                string filename = dlg.FileName;
                int ind = filename.IndexOf('.');
                filename = filename.Substring(0, ind);
                ind = filename.LastIndexOf('\\');
                filename = filename.Substring(ind + 1);
                Canvas canvas = DeSerializeXAML(dlg.FileName) as Canvas;

                Database dataLoad = new Database();
                XmlSerializer serializer = new XmlSerializer(typeof(Database));
                using (TextReader tr = new StreamReader(filename))
                {
                    object ob = serializer.Deserialize(tr);
                    dataLoad = ob as Database;
                }

                ((MainWindow)wrap.mw).MainCanvas.Children.Clear();  //praznjenje database-a
                subsList.Clear();
                nodeList.Clear();
                terminalList.Clear();
                lineList.Clear();
                UndoStack.Clear();
                RedoStack.Clear();

                List<Terminal> paramTerm = new List<Terminal>();

                foreach (Substation sub in dataLoad.Substations)    //dodavanje 
                {
                    subsList.Add(sub);
                    foreach (ConnectivityNode node in sub.ConnectivityNodes)
                    {
                        if (node.Terminals.Count > 0)
                        {
                            foreach (Terminal ter in node.Terminals)
                            {
                                terminalList.Add(ter);
                                paramTerm.Add(ter);
                                if (ter.ConductingEquipment != null) //ignorisali smo ovo pa nece biti
                                {
                                    lineList.Add((ACLineSegment)ter.ConductingEquipment);
                                }
                            }
                        }
                    }
                }

                foreach (ConnectivityNode node in dataLoad.Nodes)
                {
                    nodeList.Add(node);
                }

                foreach (BaseVoltage bv in dataLoad.BaseVoltages)
                {
                    BaseVolList.Add(bv);
                }

                // Dodavanje svih canvasa, ellipsa i linija na canvas
                while (canvas.Children.Count > 0)
                {
                    UIElement obj = canvas.Children[0]; // Get next child
                    canvas.Children.Remove(obj); // Have to disconnect it from result before we can add it
                    ((MainWindow)wrap.mw).MainCanvas.Children.Add(obj); // Add to canvas
                    if (obj is Ellipse)
                    {
                        UIElements.Add(obj);
                    }

                    if (obj is Canvas)
                    {
                        Canvas can = obj as Canvas;
                        Substation sub = new Substation();
                        sub.mRID = can.Uid;
                        sub.name = can.Name;
                        string volLvl = (String)can.Tag;
                        //serijalizuj listu sa substationima i proveri po mRIDu koje volt lvl imaju
                        try
                        {

                            foreach (Substation subs in dataLoad.Substations)
                            {
                                if (sub.mRID == subs.mRID)
                                {
                                    foreach (VoltageLevel vl in subs.VoltageLevels)
                                    {
                                        sub.VoltageLevels.Add(vl);
                                    }
                                    break;
                                }
                            }

                        }
                        catch (IOException)
                        {
                        }

                        //subsList.Add(sub);                        
                    }
                }
                var MC = ((MainWindow)wrap.mw).MainCanvas;
                IEnumerable<Line> MClines = MC.Children.OfType<Line>().ToList(); //sve linije
                foreach (Line ln in MClines)
                {
                    MC.Children.Remove(ln);
                }

                AddLineCommandExecution(paramTerm);
            }

        }       //LOAD

        private void RightToLeftExecution(object param)
        {
            Context wrap = Context.getInstance();
            AddSubstation window = ((AddSubstation)wrap.subwin);
            if (window.SubRight.SelectedIndex > -1)
            {
                if (!window.SubLeft.Items.Contains(window.SubRight.SelectedItem))
                {
                    window.SubLeft.Items.Add(window.SubRight.SelectedItem);
                    window.SubLeft.Items.Refresh();
                    window.SubRight.SelectedIndex = -1;
                }
            }
        }       //za dodavanje voltage levela u substation

        private void LeftToRightExecution(object param)
        {
            Context wrap = Context.getInstance();
            AddSubstation window = ((AddSubstation)wrap.subwin);
            if (window.SubLeft.SelectedIndex > -1)
            {
                window.SubLeft.Items.Remove(window.SubLeft.SelectedItem);
                window.SubLeft.Items.Refresh();
                window.SubLeft.SelectedIndex = -1;
            }
        }

        public static void SerializeToXML(MainWindow window, Canvas canvas, int dpi, string filename, object obj = null)
        {
            string mystrXAML = XamlWriter.Save(canvas);
            FileStream filestream = File.Create(filename);
            StreamWriter streamwriter = new StreamWriter(filestream);
            streamwriter.Write(mystrXAML);
            streamwriter.Close();
            filestream.Close();
        }

        public static UIElement DeSerializeXAML(string filename)
        {
            // Load XAML from file. Use 'using' so objects are disposed of properly.
            using (System.IO.FileStream fs = System.IO.File.Open(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                return System.Windows.Markup.XamlReader.Load(fs) as UIElement;
            }
        }

        #endregion
    }
}

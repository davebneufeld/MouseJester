using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows;
using System.Windows.Media;

namespace Orbiter
{
    public class GestureManager
    {
        private static GestureManager _Instance = null;
        public static GestureManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new GestureManager();
                }
                return _Instance;
            }
        }

        private List<Gesture> GestureCollection;
        private string _GestureFileName;
        public string GestureFileName
        {
            get
            {
                return _GestureFileName;
            }
            set
            {
                _GestureFileName = value;
            }
        }

        private HotKey _hkey = null;
        public HotKey hkey
        {
            get
            {
                return _hkey;
            }
            set
            {
                if (_hkey != null)
                {
                    _hkey.Dispose();
                }
                _hkey = value;
            }
        }

        private GestureManager()
        {
            GestureCollection = new List<Gesture>();
            GestureFileName = "gestures";
        }

        public void Save()
        {
            XmlWriter xmlWriter = XmlWriter.Create(GestureFileName);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement(Constants.GESTURES_TAG);

            foreach (Gesture g in GestureCollection)
            {
                xmlWriter.WriteStartElement(Constants.GESTURE_TAG);
                {
                    xmlWriter.WriteStartElement(Constants.NAME_TAG);
                    xmlWriter.WriteString(g.Name);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement(Constants.DATA_TAG);
                    foreach (Point p in g.PointVector)
                    {
                        xmlWriter.WriteBase64(BitConverter.GetBytes(p.X), 0, 8);
                        xmlWriter.WriteBase64(BitConverter.GetBytes(p.Y), 0, 8);
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement(Constants.ACTIONS_TAG);
                    xmlWriter.WriteString("404");
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        private void LoadGesture(XmlReader reader)
        {
            string Name = "";
            List<Point> gesturePoints = new List<Point>();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == Constants.NAME_TAG)
                    {
                        reader.Read(); //read value node
                        Name = reader.Value;
                        reader.Read(); //read end tag
                    }
                    else if (reader.Name == Constants.DATA_TAG)
                    {
                        //space for two doubles: x, y
                        byte[] dataBuffer = new byte[16];
                        double x, y;

                        reader.Read();
                        while (reader.ReadContentAsBase64(dataBuffer, 0, 16) != 0)
                        {
                            x = BitConverter.ToDouble(dataBuffer, 0);
                            y = BitConverter.ToDouble(dataBuffer, 8);
                            gesturePoints.Add(new Point(x, y));
                        }
                    }
                    else if (reader.Name == Constants.ACTIONS_TAG)
                    {
                        reader.Read(); //read value node
                        reader.Read(); //read end tag
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    new Gesture(gesturePoints).Register(Name);
                    break;
                }
            }
        }

        public void Load()
        {
            Clear();
            using (XmlReader reader = XmlReader.Create(GestureFileName))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == Constants.GESTURE_TAG)
                        {
                            LoadGesture(reader);
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                }
            }
        }

        public void Clear()
        {
            GestureCollection.Clear();
        }

        public void Add(Gesture g)
        {
            GestureCollection.Add(g);
        }

        public void HotKeyHandler(Object sender, HotKeyEventArgs e)
        {
            if (e.id == Constants.GESTURE_INPUT_ID)
            {
                GestureCanvas gDrawer = InputGesture(true);
                if (gDrawer != null)
                {
                    Gesture gMatched = gDrawer.matchedGesture;
                    if (gMatched != null)
                    {
                        gMatched.Execute();
                    }
                }
            }
        }

        public GestureCanvas InputGesture(bool isMatching /*as opposed to defining*/)
        {
            GestureCanvas drawer = new GestureCanvas(Brushes.LightSteelBlue, Brushes.Black, true, isMatching, true);
            if (drawer.DialogResult == true)
            {
                return drawer;
            }
            else
            {
                return null;
            }
        }

        public KeyValuePair<double, Gesture> Recognize(Gesture g)
        {
            return new KeyValuePair<double, Gesture>(1, g);
        }
    }
}

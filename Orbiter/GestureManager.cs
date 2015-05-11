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

        public void SaveGestures()
        {
            XmlWriter xmlWriter = XmlWriter.Create(GestureFileName);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("GESTURE");

            foreach (Gesture g in GestureCollection)
            {
                xmlWriter.WriteStartElement("GESTURE");
                {
                    xmlWriter.WriteStartElement("NAME");
                    xmlWriter.WriteString(g.Name);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("DATA");
                    foreach (Point p in g.PointVector)
                    {
                        xmlWriter.WriteBase64(BitConverter.GetBytes(p.X), 0, 8);
                        xmlWriter.WriteBase64(BitConverter.GetBytes(p.Y), 0, 8);
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("ACTIONS");
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
                    if (reader.Name == "NAME")
                    {
                        reader.Read(); //read value node
                        Name = reader.Value;
                        reader.Read(); //read end tag
                    }
                    else if (reader.Name == "DATA")
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
                    else if (reader.Name == "ACTIONS")
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

        public void LoadGestures()
        {
            Clear();

            using (XmlReader reader = XmlReader.Create(GestureFileName))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "GESTURE")
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
                GestureDrawer gDrawer = inputGesture(true);
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

        public GestureDrawer inputGesture(bool isMatching /*as opposed to defining*/)
        {
            GestureDrawer drawer = new GestureDrawer(Brushes.LightSteelBlue, Brushes.Black, true, isMatching, true);
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

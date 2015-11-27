using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows;
using System.Windows.Media;
using System.IO;
using System.Collections.ObjectModel;
using Qaovxtazypdl.GlobalHotKeys;

namespace MouseJester
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

        private HotKey _hkey = null;
        public HotKey hkey
        {
            get
            {
                return _hkey;
            }
            private set
            {
                if (_hkey != null)
                {
                    _hkey.Dispose();
                }
                _hkey = value;
            }
        }

        internal uint InitiateGestureKey = 0;
        internal uint InitiateGestureModifiers = 0;

        internal void UpdateHotKey()
        {
            if (Qaovxtazypdl.GlobalHotKeys.Constants.vkeyMap.ContainsKey(InitiateGestureKey) &&
                InitiateGestureModifiers != 0 &&
                HotKey.isHotKeyAvilable(InitiateGestureModifiers, InitiateGestureKey))
            {
                this.hkey = new HotKey(
                    Constants.GESTURE_INPUT_ID, 
                    InitiateGestureModifiers, 
                    InitiateGestureKey, 
                    GestureManager.Instance.HotKeyHandler
                );
            }
            MainWindow.Instance.SetHotKeyString(HotKey.GetKeyComboString(InitiateGestureModifiers, InitiateGestureKey));
        }

        private void RemoveUnusedGestureImages()
        {
            // remove all png files in the directory that are not in the paths.
            List<String> pngFilesInDirectory = new List<String>(Directory.GetFiles(Directory.GetCurrentDirectory(), "*-*-*-*-*.png"));

            List<String> currentImagePaths = new List<String>();
            foreach (Gesture g in MainWindow.Instance.GestureCollection)
            {
                currentImagePaths.Add(g.ImageFullPath);
            }

            IEnumerable<String> unusedPngFiles = pngFilesInDirectory.Except(currentImagePaths);
            foreach (String unusedFile in unusedPngFiles)
            {
                try
                {
                    File.Delete(unusedFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public void Save()
        {
            Save(Constants.GESTURE_FILE_NAME, MainWindow.Instance.GestureCollection);
            UpdateHotKey();
            RemoveUnusedGestureImages();
        }

        public void Save(string fileName, ObservableCollection<Gesture> gestures)
        {
            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;
            XmlWriter xmlWriter = XmlWriter.Create(fileName);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement(Constants.GESTURES_TAG);

            xmlWriter.WriteStartElement(Constants.GESTURE_HKEY_TAG);
            xmlWriter.WriteStartElement(Constants.MODIFIERS_TAG);
            {
                xmlWriter.WriteString("" + InitiateGestureModifiers);
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement(Constants.KEY_TAG);
            {
                xmlWriter.WriteString("" + InitiateGestureKey);
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();

            foreach (Gesture g in gestures)
            {
                xmlWriter.WriteStartElement(Constants.GESTURE_TAG);
                {
                    if (!String.IsNullOrEmpty(g.Description))
                    {
                        xmlWriter.WriteStartElement(Constants.NAME_TAG);
                        xmlWriter.WriteString(g.Description);
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteStartElement(Constants.DATA_TAG);
                    foreach (Point p in g.PointVector)
                    {
                        xmlWriter.WriteBase64(BitConverter.GetBytes(p.X), 0, 8);
                        xmlWriter.WriteBase64(BitConverter.GetBytes(p.Y), 0, 8);
                    }
                    xmlWriter.WriteEndElement();

                    if (!String.IsNullOrEmpty(g.Action.Path) ||
                        !String.IsNullOrEmpty(g.Action.Arguments) ||
                        !String.IsNullOrEmpty(g.Action.StartIn))
                    {
                        xmlWriter.WriteStartElement(Constants.ACTIONS_TAG);
                        if (!String.IsNullOrEmpty(g.Action.Path))
                        {
                            xmlWriter.WriteStartElement(Constants.ACTION_PATH_TAG);
                            xmlWriter.WriteString(g.Action.Path);
                            xmlWriter.WriteEndElement();
                        }

                        if (!String.IsNullOrEmpty(g.Action.Arguments))
                        {
                            xmlWriter.WriteStartElement(Constants.ACTION_ARGS_TAG);
                            xmlWriter.WriteString(g.Action.Arguments);
                            xmlWriter.WriteEndElement();
                        }

                        if (!String.IsNullOrEmpty(g.Action.StartIn))
                        {
                            xmlWriter.WriteStartElement(Constants.ACTION_START_IN_TAG);
                            xmlWriter.WriteString(g.Action.StartIn);
                            xmlWriter.WriteEndElement();
                        }
                        xmlWriter.WriteEndElement();
                    }


                    xmlWriter.WriteStartElement(Constants.IMAGE_PATH_TAG);
                    xmlWriter.WriteString(g.ImagePath);
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
            string ActionPath = "";
            string ActionArgs = "";
            string ActionStartIn = "";
            string ImagePath = "";

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
                    else if (reader.Name == Constants.IMAGE_PATH_TAG)
                    {
                        reader.Read(); //read value node
                        ImagePath = reader.Value;
                        reader.Read(); //read end tag
                    }
                    else if (reader.Name == Constants.ACTIONS_TAG)
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                if (reader.Name == Constants.ACTION_PATH_TAG)
                                {
                                    reader.Read(); //read value node
                                    ActionPath = reader.Value;
                                    reader.Read(); //read end tag
                                }
                                else if (reader.Name == Constants.ACTION_ARGS_TAG)
                                {
                                    reader.Read(); //read value node
                                    ActionArgs = reader.Value;
                                    reader.Read(); //read end tag
                                }
                                else if (reader.Name == Constants.ACTION_START_IN_TAG)
                                {
                                    reader.Read(); //read value node
                                    ActionStartIn = reader.Value;
                                    reader.Read(); //read end tag
                                }
                            }
                            else if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                break;
                            }
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    Gesture g = new Gesture(gesturePoints);
                    g.Register(Name);
                    g.Action = new GestureAction(ActionPath, ActionArgs, ActionStartIn);
                    g.ImagePath = ImagePath;
                    break;
                }
            }
        }

        public void Load()
        {
            List<String> currentImagePaths = new List<String>();

            MainWindow.Instance.GestureCollection.Clear();
            Load(Constants.GESTURE_FILE_NAME, currentImagePaths);
            UpdateHotKey();
        }

        public void Load(string fileName, List<String> currentImagePaths)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(fileName))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == Constants.GESTURE_TAG)
                            {
                                LoadGesture(reader);
                            }
                            else if (reader.Name == Constants.GESTURE_HKEY_TAG)
                            {
                                while (reader.Read())
                                {
                                    if (reader.Name == Constants.MODIFIERS_TAG)
                                    {
                                        reader.Read();
                                        InitiateGestureModifiers = (uint)Int32.Parse(reader.Value);
                                        reader.Read();
                                    }
                                    else if (reader.Name == Constants.KEY_TAG)
                                    {
                                        reader.Read();
                                        InitiateGestureKey = (uint)Int32.Parse(reader.Value);
                                        reader.Read();
                                    }
                                    else if (reader.NodeType == XmlNodeType.EndElement)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement)
                        {
                            break;
                        }
                    }
                }
            }
            catch (FileNotFoundException) {
                //fail silently
            }
        }

        public void Clear()
        {
            MainWindow.Instance.GestureCollection.Clear();
        }

        public void Add(Gesture g)
        {
            MainWindow.Instance.GestureCollection.Add(g);
        }

        public Gesture Remove(int internalGestureID)
        {
            Gesture item = MainWindow.Instance.GestureCollection.Single(x => x.InternalID == internalGestureID);
            MainWindow.Instance.GestureCollection.Remove(item);

            return item;
        }

        public int Count()
        {
            return MainWindow.Instance.GestureCollection.Count();
        }

        public void HotKeyHandler(Object sender, HotKeyEventArgs e)
        {
            HotKeyProcesser(e.id);
        }

        private void MatchAndExecuteGesture()
        {
            GestureCanvas gDrawer = InputGesture(true);
            if (gDrawer != null)
            {
                Gesture gMatched = gDrawer.matchedGesture;
                if (gMatched != null)
                {
                    gMatched.Action.Execute();
                }
            }
        }

        internal void DefineNewGesture()
        {
            GestureCanvas gDrawer = InputGesture(false);
            if (gDrawer != null && !gDrawer.defineTooSimilar)
            {
                Gesture gDrawn = gDrawer.drawnGesture;
                this.Add(gDrawn);
            }
            else if (gDrawer.defineTooSimilar)
            {
                MessageBox.Show("Gesture too similar to a previously defined gesture.");
            }
        }

        internal void RedefineGesture(int internalGestureID)
        {
            GestureCanvas gDrawer = InputGesture(false, internalGestureID);
            if (gDrawer != null && !gDrawer.defineTooSimilar)
            {
                Gesture gDrawn = gDrawer.drawnGesture;

                foreach (Gesture g in MainWindow.Instance.GestureCollection)
                {
                    if (g.InternalID == internalGestureID)
                    {
                        int index = MainWindow.Instance.GestureCollection.IndexOf(g);
                        g.copyActionAndDescriptionTo(gDrawn);
                        MainWindow.Instance.GestureCollection[index] = gDrawn;
                        break;
                    }
                }
            }
            else if (gDrawer.defineTooSimilar)
            {
                MessageBox.Show("Gesture too similar to a previously defined gesture.");
            }
        }

        private void HotKeyProcesser(int keyID)
        {
            if (keyID == Constants.GESTURE_INPUT_ID)
            {
                this.MatchAndExecuteGesture();
            }
        }

        public GestureCanvas InputGesture(bool isMatching /*as opposed to defining*/, int excludeIdFromMatch = -1)
        {
            GestureCanvas drawer = new GestureCanvas(false, isMatching, true, excludeIdFromMatch);
            if (drawer.DialogResult == true)
            {
                return drawer;
            }
            else
            {
                return null;
            }
        }

        public KeyValuePair<double, Gesture> Recognize(Gesture input, int excludeIdFromMatch)
        {
            KeyValuePair<double, Gesture> bestMatch = new KeyValuePair<double, Gesture>(1, null);
            foreach(Gesture definedGesture in MainWindow.Instance.GestureCollection)
            {
                if (definedGesture.InternalID == excludeIdFromMatch)
                {
                    continue;
                }

                double matchError = PerformMatch(input, definedGesture);
                if (matchError < bestMatch.Key)
                {
                    bestMatch = new KeyValuePair<double, Gesture>(matchError, definedGesture);
                }
            }
            return bestMatch;
        }

        //simple least squares error returns value in [0,1]
        private double PerformMatch(Gesture input, Gesture definedGesture)
        {
            double weight = 1.0 / (Constants.GESTURE_POINTS - 1);
            double matchError = 0;
            for (int i = 0; i < Constants.GESTURE_POINTS - 1; i++)
            {
                double distance = Math.Abs(input.Directions[i] - definedGesture.Directions[i]);

                //account for the periodic nature of angles
                distance = distance > 1 ? 2 - distance : distance;
                matchError += weight * Math.Pow(distance, 2);
            }

            return matchError;
        }
    }
}

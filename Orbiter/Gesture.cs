using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Orbiter
{
    public class Gesture
    {
        public static List<Gesture> currGestures = new List<Gesture>();
        public static HotKey _hkey = null;
        public static HotKey hkey
        {
            get
            {
                return _hkey;
            }
            set
            {
                if(_hkey != null)
                {
                    _hkey.Dispose();
                }
                _hkey = value;
            }
        }

        public static void HotKeyHandler(Object sender, HotKeyEventArgs e)
        {
            if (e.id == Constants.GESTURE_INPUT_ID)
            {
                GestureDrawer gDrawer = inputGesture(true);
                if(gDrawer != null)
                {
                    Gesture gMatched = gDrawer.matchedGesture;
                    if (gMatched != null)
                    {
                        gMatched.Execute();
                    }
                }
            }
        }

        public static GestureDrawer inputGesture(bool isMatching /*as opposed to defining*/)
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

        public static void SaveGestures()
        {

        }

        public static void LoadGestures()
        {

        }

        public static KeyValuePair<double, Gesture> Recognize(Gesture g)
        {
            return new KeyValuePair<double,Gesture>(1, g);
        }

        //instanced stuff
        private List<Point> _PointVector;
        public List<Point> PointVector
        {
            get
            {
                return _PointVector;
            }
        }

        public Gesture(Gesture other)
        {
            this._PointVector = new List<Point>(other.PointVector);
        }

        public Gesture(List<Point> rawPoints)
        {
            _PointVector = new List<Point>();
            double minX = Double.MaxValue, minY = Double.MaxValue, maxX = Double.MinValue, maxY = Double.MinValue;
            foreach (Point p in rawPoints)
            {
                minX = Math.Min(minX, p.X);
                minY = Math.Min(minY, p.Y);
                maxX = Math.Max(maxX, p.X);
                maxY = Math.Max(maxY, p.Y);
            }
            scaleAndCenterGesture(rawPoints, minX, minY, maxX, maxY);
        }

        public Gesture(List<Point> rawPoints, double minX, double minY, double maxX, double maxY)
        {
            _PointVector = new List<Point>();
            scaleAndCenterGesture(rawPoints, minX, minY, maxX, maxY);
        }

        public void scaleAndCenterGesture(List<Point> rawPoints, double minX, double minY, double maxX, double maxY)
        {
            if (rawPoints.Count < 2)
            {
                for (int i = 0; i < Constants.GESTURE_POINTS; i++)
                {
                    _PointVector.Add(new Point(0,0));
                }
            }
            else
            {
                //double sourceDims = Math.Max(maxX - minX, maxY - minY); TODO:
                double scaleFactorX = Constants.GESTURE_WIDTH / (maxX - minX);
                double scaleFactorY = Constants.GESTURE_HEIGHT / (maxY - minY);
                double indexScaleFactor = (double)(rawPoints.Count - 1) / Constants.GESTURE_POINTS;
                _PointVector.Clear();
                for (int i = 0; i < Constants.GESTURE_POINTS; i++)
                {
                    double scaledIndex = indexScaleFactor * i;
                    double interpolationFactor = scaledIndex - (int)scaledIndex;
                    Point lowerPoint = rawPoints[(int)scaledIndex];
                    Point upperPoint = rawPoints[(int)scaledIndex + 1];
                    Point scaledPoint = new Point(lowerPoint.X + interpolationFactor * (upperPoint.X - lowerPoint.X), lowerPoint.Y + interpolationFactor * (upperPoint.Y - lowerPoint.Y));
                    _PointVector.Add(new Point(scaleFactorX * (scaledPoint.X - minX), scaleFactorY * (scaledPoint.Y - minY)));
                }
            }
        }

        public void Execute()
        {

        }

        public void SetAction()
        {

        }
    }
}

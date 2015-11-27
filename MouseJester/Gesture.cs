using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


namespace MouseJester
{
    public class Gesture
    {
        private static int InternalGestureID = 0;

        private static int getNextInternalID()
        {
            return InternalGestureID++;
        }

        private int _InternalID = -1;
        public int InternalID
        {
            get
            {
                if (_InternalID == -1)
                {
                    _InternalID = Gesture.getNextInternalID();
                }
                return _InternalID;
            }
        }
        private string _Description;
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }

        private List<Point> _PointVector;
        public List<Point> PointVector
        {
            get
            {
                return _PointVector;
            }
        }

        private List<double> _Directions;
        public List<double> Directions
        {
            get
            {
                return _Directions;
            }
        }

        private String _ImagePath;
        public String ImagePath
        {
            get
            {
                return "C:\\Users\\qaovxtazypdl\\Source\\Repos\\MouseJester\\MouseJester\\bin\\Debug\\test.jpg";
            }
            set
            {
                _ImagePath = value;
            }
        }

        public GestureAction Action { get; set; }

        public Gesture(Gesture other)
        {
            this._Description = null;
            this._PointVector = new List<Point>(other.PointVector);
            this._Directions = new List<double>(other.Directions);
            this.Action = new GestureAction(other.Action);
        }

        public Gesture(List<Point> rawPoints)
        {
            double minX = Double.MaxValue, minY = Double.MaxValue, maxX = Double.MinValue, maxY = Double.MinValue;
            foreach (Point p in rawPoints)
            {
                minX = Math.Min(minX, p.X);
                minY = Math.Min(minY, p.Y);
                maxX = Math.Max(maxX, p.X);
                maxY = Math.Max(maxY, p.Y);
            }

            double sourceDims = 0;
            double originalDeltaX = maxX - minX;
            double originalDeltaY = maxY - minY;

            if (originalDeltaX >= originalDeltaY)
            {
                sourceDims = originalDeltaX;
                double deltaDiff = originalDeltaX - originalDeltaY;
                minY -= deltaDiff / 2;
                maxY += deltaDiff / 2;
            }
            else
            {
                sourceDims = originalDeltaY;
                double deltaDiff = originalDeltaY - originalDeltaX;
                minX -= deltaDiff / 2;
                maxX += deltaDiff / 2;
            }

            this.Action = new GestureAction();
            Initialize(rawPoints, minX, minY, maxX, maxY);
        }

        public Gesture(List<Point> rawPoints, double minX, double minY, double maxX, double maxY)
        {
            Initialize(rawPoints, minX, minY, maxX, maxY);
        }

        private void Initialize(List<Point> rawPoints, double minX, double minY, double maxX, double maxY)
        {
            this._Description = null;
            this._PointVector = new List<Point>();
            this._Directions = new List<double>();
            this.scaleAndCenterGesture(rawPoints, minX, minY, maxX, maxY);
            this.Action = new GestureAction();
        }

        private void scaleAndCenterGesture(List<Point> rawPoints, double minX, double minY, double maxX, double maxY)
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
            calcDirections();
        }

        private void calcDirections()
        {
            if (PointVector.Count == 0) return;

            Point prevPoint = PointVector[0];
            for (int i = 1; i < PointVector.Count; i++)
            {
                Point curPoint = PointVector[i];
                //normalize resulting direction angle to [0,2]
                _Directions.Add(Math.Atan2(curPoint.Y - prevPoint.Y, curPoint.X - prevPoint.X) / (Math.PI) + 1);
            }
        }

        public void Register(string Name)
        {
            this._Description = Name;
            GestureManager.Instance.Add(this);
        }
    }
}

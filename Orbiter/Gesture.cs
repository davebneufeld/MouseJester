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
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
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

        public Gesture(Gesture other)
        {
            this._Name = null;
            this._PointVector = new List<Point>(other.PointVector);
            this._Directions = new List<double>(other.Directions);
        }

        public Gesture(List<Point> rawPoints)
        {
            this._Name = null;
            this._PointVector = new List<Point>();
            this._Directions = new List<double>();
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

            this.scaleAndCenterGesture(rawPoints, minX, minY, maxX, maxY);
        }

        public Gesture(List<Point> rawPoints, double minX, double minY, double maxX, double maxY)
        {
            this._Name = null;
            this._PointVector = new List<Point>();
            this._Directions = new List<double>();
            this.scaleAndCenterGesture(rawPoints, minX, minY, maxX, maxY);
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

        public void calcDirections()
        {
            if (PointVector.Count == 0) return;

            Point prevPoint = PointVector[0];
            for (int i = 1; i < PointVector.Count; i++)
            {
                Point curPoint = PointVector[i];
                //normalize resulting direction angle to [0,1]
                _Directions.Add(Math.Atan2(curPoint.Y - prevPoint.Y, curPoint.X - prevPoint.X) / (2 * Math.PI) + 1);
            }
        }

        public void Register(string Name)
        {
            this._Name = Name;
            GestureManager.Instance.Add(this);
        }

        public void SetAction()
        {

        }

        public void ExecuteAction()
        {

        }

    }
}

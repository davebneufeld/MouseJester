using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Orbiter
{
    public partial class GestureDrawer : Window
    {
        private bool mouseDown = false;
        private Point prevPos = new Point(-1, -1);
        private double minX, minY, maxX, maxY;
        private List<Point> rawPoints = new List<Point>();
        private Brush confirmColor = Brushes.Red;

        private bool isMatching;
        private bool displayPreview;
        private bool drawOutline;
        private Brush drawColor;
        private Brush outlineColor;

        public bool defineTooSimilar = false;
        public Gesture drawnGesture = null;
        public Gesture matchedGesture = null;

        public GestureDrawer(Brush drawColor, Brush outlineColor, bool drawOutline, bool isMatching /*as opposed to defining a gesture*/, bool displayPreview)
            : base()
        {
            InitializeComponent();
            this.isMatching = isMatching;
            this.drawColor = drawColor;
            this.drawOutline = drawOutline;
            this.outlineColor = outlineColor;
            this.displayPreview = displayPreview;
            ShowDialog();
        }

        private void DrawLine(Point prevPos, Point pos, Brush brushColor, Brush outlineColor, bool drawOutline)
        {
            if (drawOutline)
            {
                Line outline = new Line();
                outline.Stroke = outlineColor;
                outline.X1 = prevPos.X;
                outline.X2 = pos.X;
                outline.Y1 = prevPos.Y;
                outline.Y2 = pos.Y;
                outline.StrokeThickness = 5;
                drawingGrid.Children.Add(outline);
            }

            Line myLine = new Line();
            myLine.Stroke = brushColor;
            myLine.X1 = prevPos.X;
            myLine.X2 = pos.X;
            myLine.Y1 = prevPos.Y;
            myLine.Y2 = pos.Y;
            myLine.StrokeThickness = drawOutline ? 3 : 5;
            drawingGrid.Children.Add(myLine);
        }

        private void DrawGesture(Gesture g, Brush drawColor, Brush outlineColor, bool drawOutline)
        {
            DrawVector(g.PointVector, drawColor, outlineColor, drawOutline);
        }

        private void DrawVector(List<Point> points, Brush drawColor, Brush outlineColor, bool drawOutline)
        {
            Point prevPos = points[0];
            foreach (Point pos in points)
            {
                DrawLine(prevPos, pos, drawColor, outlineColor, drawOutline);
                prevPos = pos;
            }
        }

        private void MouseMoveEventHandler(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Point pos = e.GetPosition(this);
                List<Point> normalizedLenPoints = new List<Point>();
                double dist = Math.Sqrt(Math.Pow(pos.X - prevPos.X, 2) + Math.Pow(pos.Y - prevPos.Y, 2));
                double segs = dist / Constants.MIN_LINE_LEN;
                Point unit = new Point((pos.X - prevPos.X) / segs, (pos.Y - prevPos.Y) / segs);
                for (int i = 0; i < (int)segs; i++)
                {
                    Point curPos = new Point(prevPos.X + unit.X, prevPos.Y + unit.Y);
                    rawPoints.Add(curPos);
                    DrawLine(prevPos, curPos, drawColor, outlineColor, drawOutline);
                    prevPos = curPos;

                    minX = Math.Min(minX, pos.X);
                    minY = Math.Min(minY, pos.Y);
                    maxX = Math.Max(maxX, pos.X);
                    maxY = Math.Max(maxY, pos.Y);
                }
            }
        }

        private void MouseDownEventHandler(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            Point startPos = e.GetPosition(this);
            rawPoints.Add(startPos);
            prevPos = startPos;

            minX = maxX = startPos.X;
            minY = maxY = startPos.Y;
        }

        private void MouseUpEventHandler(object sender, MouseEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += processGesture;
            worker.RunWorkerCompleted += processingCompleted;
            worker.RunWorkerAsync();
        }

        private void processGesture(object sender, DoWorkEventArgs e)
        {
            mouseDown = false;
            drawnGesture = new Gesture(rawPoints, minX, minY, maxX, maxY);
            if (drawnGesture != null)
            {
                KeyValuePair<double, Gesture> matched = Gesture.Recognize(drawnGesture);
                double matchAccuracy = matched.Key;
                Gesture closestMatch = matchAccuracy > Constants.MATCH_THRESHOLD? matched.Value : null;

                if (displayPreview && closestMatch != null)
                {
                    if (isMatching) //matching. display the matched gesture.
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            drawingGrid.Children.Clear();
                            DrawGesture(closestMatch, confirmColor, outlineColor, true);
                        }));
                    }
                    else //defining. if too similar, display similar gesture.
                    {
                        defineTooSimilar = true;
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            drawingGrid.Children.Clear();
                            DrawGesture(closestMatch, confirmColor, outlineColor, true);
                            MessageBox.Show(this, "Gesture too similar to a previously defined gesture.");
                        }));
                    }
                    Thread.Sleep(500);
                }
            }
        }

        private void processingCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DialogResult = drawnGesture != null;
        }
    }
}

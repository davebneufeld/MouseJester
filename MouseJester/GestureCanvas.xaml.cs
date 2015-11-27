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
using System.IO;

namespace MouseJester
{
    public partial class GestureCanvas : Window
    {
        private bool mouseDown;
        private Point prevPos;
        private double minX, minY, maxX, maxY;
        private List<Point> rawPoints;
        private Brush confirmColor;

        private bool isMatching;
        private bool displayPreview;
        private bool drawOutline;
        private bool finished;
        private Brush drawColor;
        private Brush outlineColor;
        private Canvas drawingCanvas;

        public bool defineTooSimilar;
        public Gesture drawnGesture;
        public Gesture matchedGesture;

        public GestureCanvas(bool drawOutline, bool isMatching /*as opposed to defining a gesture*/, bool displayPreview)
            : base()
        {
            InitializeComponent();
            this.isMatching = isMatching;
            this.drawColor = null;
            this.drawOutline = drawOutline;
            this.outlineColor = Brushes.Black;
            this.confirmColor = Brushes.LightSteelBlue;
            this.displayPreview = displayPreview;
            this.rawPoints = new List<Point>();
            this.mouseDown = false;
            this.finished = false;
            this.defineTooSimilar = false;
            this.drawnGesture = null;
            this.matchedGesture = null;
            GestureManager.Instance.hkey.Disabled = true;

            drawingCanvas = new Canvas();
            drawingCanvas.MouseDown += MouseDownEventHandler;
            drawingCanvas.MouseUp += MouseUpEventHandler;
            drawingCanvas.MouseMove += MouseMoveEventHandler;
            drawingGrid.Children.Add(drawingCanvas);

            ShowDialog();
            this.Activate();
            this.Focus();  
        }

        private void DrawLine(Point prevPos, Point pos, Brush brushColor, Brush outlineColor, bool drawOutline)
        {
            if (drawOutline)
            {
                Line outline = new Line();
                outline.MouseDown += MouseDownEventHandler;
                outline.MouseUp += MouseUpEventHandler;
                outline.MouseMove += MouseMoveEventHandler;

                outline.Stroke = outlineColor;
                outline.X1 = prevPos.X;
                outline.X2 = pos.X;
                outline.Y1 = prevPos.Y;
                outline.Y2 = pos.Y;
                outline.StrokeThickness = 13;
                drawingCanvas.Children.Add(outline);
            }

            Line lineSegment = new Line();
            lineSegment.MouseDown += MouseDownEventHandler;
            lineSegment.MouseUp += MouseUpEventHandler;
            lineSegment.MouseMove += MouseMoveEventHandler;

            double redFraction = ((pos.X - prevPos.X) / Constants.MIN_LINE_LEN + 1) / 2;
            double greenFraction = ((pos.Y - prevPos.Y) / Constants.MIN_LINE_LEN + 1) / 2;

            if (brushColor == null)
            {
                lineSegment.Stroke = new SolidColorBrush(Color.FromArgb(255, (byte)(255 * redFraction), (byte)(255 *(1 - greenFraction)), (byte)(255 * (1 - redFraction))));
            }
            else
            {
                lineSegment.Stroke = brushColor;
            }
            lineSegment.X1 = prevPos.X;
            lineSegment.X2 = pos.X;
            lineSegment.Y1 = prevPos.Y;
            lineSegment.Y2 = pos.Y;
            lineSegment.StrokeThickness = drawOutline ? 10 : 13;
            drawingCanvas.Children.Add(lineSegment);
        }

        private void DrawEllipse(Point pos, int width, int height, Brush brushColor, Brush outlineColor, bool drawOutline)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.MouseDown += MouseDownEventHandler;
            ellipse.MouseUp += MouseUpEventHandler;
            ellipse.MouseMove += MouseMoveEventHandler;

            ellipse.Stroke = outlineColor;
            ellipse.Fill = brushColor;
            ellipse.Width = width;
            ellipse.Height = height;
            ellipse.StrokeThickness = drawOutline ? 15 : 0;
            drawingCanvas.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, pos.X - 6);
            Canvas.SetTop(ellipse, pos.Y - 6);
        }

        private void DrawGesture(Gesture g, Brush drawColor, Brush outlineColor, bool drawOutline)
        {
            List<Point> scaledPoints = new List<Point>();

            double scaleFactorX = (maxX - minX) / Constants.GESTURE_WIDTH;
            double scaleFactorY = (maxY - minY) / Constants.GESTURE_HEIGHT;
            foreach(Point p in g.PointVector)
            {
                scaledPoints.Add(new Point(minX + scaleFactorX * p.X, minY + scaleFactorY * p.Y));
            }
            DrawVector(scaledPoints, drawColor, outlineColor, drawOutline);
        }

        private void DrawVector(List<Point> points, Brush drawColor, Brush outlineColor, bool drawOutline)
        {
            if (points.Count == 0) return;

            Point prevPos = points[0];
            DrawEllipse(prevPos, 25, 25, drawColor, outlineColor, drawOutline);
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
            if (!finished)
            {
                mouseDown = true;
                Point startPos = e.GetPosition(this);
                rawPoints.Add(startPos);
                prevPos = startPos;

                minX = maxX = startPos.X;
                minY = maxY = startPos.Y;

                DrawEllipse(startPos, 12, 12, drawColor, outlineColor, drawOutline);
            }
        }

        private void MouseUpEventHandler(object sender, MouseEventArgs e)
        {
            finished = true;
            mouseDown = false;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += processGesture;
            worker.RunWorkerCompleted += processingCompleted;
            worker.RunWorkerAsync();
        }

        private void processGesture(object sender, DoWorkEventArgs e)
        {
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

            drawnGesture = new Gesture(rawPoints, minX, minY, maxX, maxY);
            if (drawnGesture != null)
            {
                KeyValuePair<double, Gesture> matched = GestureManager.Instance.Recognize(drawnGesture);
                double matchError = matched.Key;
                double threshold = isMatching ? Constants.MATCH_THRESHOLD : Constants.DEFINE_NEW_THRESHOLD;
                Gesture closestMatch = matchError < threshold ? matched.Value : null;

                if (displayPreview && closestMatch != null)
                {
                    this.matchedGesture = closestMatch;
                    if (isMatching) //matching. display the matched gesture.
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            drawingCanvas.Children.Clear();
                            DrawGesture(matchedGesture, confirmColor, outlineColor, true);
                        }));
                    }
                    else //defining. if too similar, display similar gesture.
                    {
                        defineTooSimilar = true;
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            drawingCanvas.Children.Clear();
                            DrawGesture(matchedGesture, confirmColor, outlineColor, true);
                        }));
                    }
                    Thread.Sleep(500);
                }
                else if (!isMatching)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        String gestureImageName = this.saveBitmap();
                        drawnGesture.ImagePath = gestureImageName;
                    }));
                }
            }
        }

        private String saveBitmap()
        {
            double dpi = 96d;
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)drawingCanvas.RenderSize.Width,
                (int)drawingCanvas.RenderSize.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);
            rtb.Render(drawingCanvas);

            int minXAdjusted = (int)minX - 7;
            int minYAdjusted = (int)minY - 7;
            int maxXAdjusted = (int)maxX + 7;
            int maxYAdjusted = (int)maxY + 7;

            minXAdjusted = (int)((minXAdjusted >= 0) ? minXAdjusted : 0);
            minYAdjusted = (int)((minYAdjusted >= 0) ? minYAdjusted : 0);
            maxXAdjusted = (int)((maxXAdjusted <= drawingCanvas.RenderSize.Width) ? maxXAdjusted : drawingCanvas.RenderSize.Width);
            maxYAdjusted = (int)((maxYAdjusted <= drawingCanvas.RenderSize.Height) ? maxYAdjusted : drawingCanvas.RenderSize.Height);
            CroppedBitmap crop = new CroppedBitmap(rtb, new Int32Rect(minXAdjusted, minYAdjusted, maxXAdjusted - minXAdjusted, maxYAdjusted - minYAdjusted));

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(crop));

            String gestureImageName = Guid.NewGuid().ToString() + ".png";
            String gestureImagePath = Directory.GetCurrentDirectory() + "\\" + gestureImageName;
            using (var fs = System.IO.File.OpenWrite(gestureImagePath))
            {
                pngEncoder.Save(fs);
            }

            return gestureImageName;
        }

        private void ExitCanvas()
        {
            if(DialogResult == null)
            {
                GestureManager.Instance.hkey.Disabled = false;
                DialogResult = drawnGesture != null;
            }
        }

        private void processingCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ExitCanvas();
        }

        private void KeyDownEventHandler(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                ExitCanvas();
            }
        }
    }
}

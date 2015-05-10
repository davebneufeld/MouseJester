﻿using System;
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

namespace Orbiter
{
    public partial class GestureDrawer : Window
    {
        private bool mouseDown = false;
        private Point prevPos = new Point(-1, -1);
        private double minX = Double.MaxValue, minY = Double.MaxValue, maxX = Double.MinValue, maxY = Double.MinValue;
        private List<Point> rawPoints = new List<Point>();
        private Brush confirmColor = Brushes.Red;

        private bool isMatching;
        private bool drawOutline;
        private Brush drawColor;
        private Brush outlineColor;

        public bool defineTooSimilar = false;
        public Gesture drawnGesture = null;
        public Gesture matchedGesture = null;

        public GestureDrawer(Brush drawColor, Brush outlineColor, bool drawOutline, bool isMatching /*as opposed to defining a gesture*/)
            : base()
        {
            InitializeComponent();
            this.isMatching = isMatching;
            this.drawColor = drawColor;
            this.drawOutline = drawOutline;
            this.outlineColor = outlineColor;
            ShowDialog();
        }

        private void DrawLine(Point prevPos, Point pos, Brush brushColor, Brush outlineColor, bool drawOutline)
        {
            if (prevPos.X == -1 || prevPos.Y == -1) return;
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
            Point prevPos = new Point(-1, -1);
            foreach (Point pos in points)
            {
                Console.WriteLine("klka2 " + pos.X + " , " + pos.Y);
                DrawLine(prevPos, pos, drawColor, outlineColor, drawOutline);
            }
        }

        private void MouseMoveEventHandler(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Point pos = e.GetPosition(this);
                rawPoints.Add(pos);

                DrawLine(prevPos, pos, drawColor, outlineColor, drawOutline);
                prevPos = pos;

                minX = Math.Min(minX, pos.X);
                minY = Math.Min(minY, pos.Y);
                maxX = Math.Max(maxX, pos.X);
                maxY = Math.Max(maxY, pos.Y);
            }
        }

        private void MouseDownEventHandler(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            prevPos = e.GetPosition(this);
        }

        private void MouseUpEventHandler(object sender, MouseEventArgs e)
        {
            Point prevPos = rawPoints[0];
            List<Point> normalizedLenPoints = new List<Point>();
            foreach (Point pos in rawPoints)
            {

                double d = Math.Sqrt(Math.Pow(pos.X - prevPos.X, 2) + Math.Pow(pos.Y - prevPos.Y, 2));
                double segs = (int)(d / Constants.MIN_LINE_LEN);
                Point unit = new Point((pos.X - prevPos.X) * Constants.MIN_LINE_LEN / d, (pos.Y - prevPos.Y) * Constants.MIN_LINE_LEN / d);
                for (int i = 0; i < (int)segs; i++)
                {
                    Point curPos = new Point(prevPos.X + unit.X, prevPos.Y + unit.Y);
                    normalizedLenPoints.Add(curPos);
                    prevPos = curPos;
                }
            }

            mouseDown = false;
            drawnGesture = new Gesture(normalizedLenPoints, minX, minY, maxX, maxY);
            if (drawnGesture != null)
            {
                KeyValuePair<double, Gesture> matched = Gesture.Recognize(drawnGesture);
                Gesture closestMatch = matched.Value;
                double matchAccuracy = matched.Key;

                drawingGrid.Children.Clear();
                DrawGesture(drawnGesture, drawColor, outlineColor, true);

                if (closestMatch != null)
                {
                    if (isMatching) //matching. display the matched gesture.
                    {
                        if (matchAccuracy > Constants.MATCH_THRESHOLD)
                        {
                            DrawGesture(closestMatch, confirmColor, outlineColor, true);
                        }
                    }
                    else //defining. if too similar, display similar gesture.
                    {
                        if (matchAccuracy > Constants.MATCH_THRESHOLD)
                        {
                            defineTooSimilar = true;
                            DrawGesture(closestMatch, confirmColor, outlineColor, true);
                            MessageBox.Show(this, "Gesture too similar to previously defined gesture.");
                        }
                    }
                }
                Thread.Sleep(2500);
            }

            DialogResult = drawnGesture != null;
            Close();
        }
    }
}

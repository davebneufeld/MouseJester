using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private Point prevPos;
        private List<Point> rawPoints = new List<Point>();
        public Gesture drawnGesture = null;

        public GestureDrawer() : base()
        {
            InitializeComponent();
            ShowDialog();
        }

        private void MouseMoveEventHandler(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Point pos = e.GetPosition(this);
                rawPoints.Add(pos);

                //draw line from prevpos to pos
            }
        }

        private void MouseDownEventHandler(object sender, MouseEventArgs e)
        {
            mouseDown = true;
        }

        private void MouseUpEventHandler(object sender, MouseEventArgs e)
        {
            mouseDown = false;

            drawnGesture = new Gesture(rawPoints);
            DialogResult = drawnGesture != null;
            Close();
        }
    }
}

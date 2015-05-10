using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Orbiter
{
    public class Gesture
    {
        public static List<Gesture> currGestures = new List<Gesture>();
        public static HotKey hkey = null;

        public static void HotKeyHandler(Object sender, HotKeyEventArgs e)
        {
            MessageBox.Show(HotKeyWindow.Instance, "Hello World. " + e.id);
            if (e.id == Constants.GESTURE_INPUT_ID)
            {
                Gesture g = inputGesture();
                if (g != null)
                {
                    Gesture closestMatch = Recognize(g);
                    if(closestMatch != null)
                    {
                        closestMatch.Execute();
                    }
                }
            }
        }

        public static Gesture inputGesture()
        {
            GestureDrawer drawer = new GestureDrawer();

            if (drawer.DialogResult == true)
            {
                return drawer.drawnGesture;
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

        private static Gesture Recognize(Gesture g)
        {
            return null;
        }

        //instanced stuff
        private List<Point> PointVector = new List<Point>();
        public Gesture(List<Point> rawPoints)
        {
        }
        public void Execute()
        {

        }

        public void SetAction()
        {

        }
    }
}

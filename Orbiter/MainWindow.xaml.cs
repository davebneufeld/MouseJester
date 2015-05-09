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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Orbiter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int GESTURE_INPUT_ID = 0;

        public MainWindow()
        {
            InitializeComponent();
            HotKeyWindow.Instance.AddHotKeyHandler(HotkeyHandler);
            HotKey initGestureInput = new HotKey(GESTURE_INPUT_ID, (uint) HotKey.Modifiers.CTRL, (uint) VirtualKey.B);
        }

        private void HotkeyHandler(Object sender, HotKeyEventArgs e)
        {
            int id = e.id;
            MessageBox.Show("Hello World. " + id);
        }
    }
}

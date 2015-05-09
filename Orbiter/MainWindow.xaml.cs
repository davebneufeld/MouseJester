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
        const int GESTURE_INPUT_ID = 1;
        List<HotKey> DefinedHotKeys;

        public MainWindow()
        {
            DefinedHotKeys = new List<HotKey>();
            InitializeComponent();
            HotKeyWindow.Instance.AddHotKeyHandler(HotkeyHandler);
            DefinedHotKeys.Add(new HotKey(GESTURE_INPUT_ID, (uint) KeyModifier.Ctrl, (uint) VirtualKey.B));
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            foreach (HotKey hkey in DefinedHotKeys)
            {
                hkey.Dispose();
            }
            HotKeyWindow.Instance.Close();
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            //minimize to taskbar.
        }

        private void HotkeyHandler(Object sender, HotKeyEventArgs e)
        {
            int id = e.id;
            MessageBox.Show("Hello World. " + id);
        }
    }
}

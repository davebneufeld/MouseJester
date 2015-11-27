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
using System.Threading;
using System.Collections.ObjectModel;
using Qaovxtazypdl.GlobalHotKeys;

namespace MouseJester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow _Instance = null;
        public static MainWindow Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new MainWindow();
                    GestureManager.Instance.Load();
                }
                return _Instance;
            }
        }

        public ObservableCollection<Gesture> GestureCollection;
        public event EventHandler ShowEvent;
        public event EventHandler CloseEvent;

        private MainWindow() : base()
        {
            GestureCollection = new ObservableCollection<Gesture>();
            InitializeComponent();
            ShowEvent += ShowEventHandler;
            CloseEvent += CloseEventHandler;
            gestureGrid.ItemsSource = GestureCollection;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        public void ExecuteShowEvent()
        {
            EventHandler handler = ShowEvent;
            if (handler != null)
            {
                handler(null, new EventArgs());
            }
        }

        public void ExecuteCloseEvent()
        {
            EventHandler handler = CloseEvent;
            if (handler != null)
            {
                handler(null, new EventArgs());
            }
        }

        private void ShowEventHandler(object sender, EventArgs e)
        {
            this.Show();
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void CloseEventHandler(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            GestureManager.Instance.Save();
            base.OnClosed(e);
        }

        private void NewGestureButtonClick(object sender, RoutedEventArgs e)
        {
            GestureManager.Instance.DefineNewGesture();
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            GestureManager.Instance.Save();
        }

        private void ReloadButtonClick(object sender, RoutedEventArgs e)
        {
            GestureManager.Instance.Load();
        }

        private void RemoveGestureClick(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                int internalGestureID = (int)(sender as Button).Tag;
                GestureManager.Instance.Remove(internalGestureID);
            }
        }

        private void RedefineGestureClick(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                int internalGestureID = (int)(sender as Button).Tag;
                GestureManager.Instance.RedefineGesture(internalGestureID);
            }
        }

        private void WindowCapTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SetHotKeyString("");
                GestureManager.Instance.InitiateGestureKey = 0;
                GestureManager.Instance.InitiateGestureModifiers = 0;
                return;
            }
            uint vkey = (uint)KeyInterop.VirtualKeyFromKey(e.Key);
            uint modifiers = (uint)e.KeyboardDevice.Modifiers;
            if (Qaovxtazypdl.GlobalHotKeys.Constants.vkeyMap.ContainsKey(vkey) &&
                modifiers != 0 &&
                HotKey.isHotKeyAvilable(modifiers, vkey))
            {
                SetHotKeyString(HotKey.GetKeyComboString(modifiers, (uint)KeyInterop.VirtualKeyFromKey(e.Key)));
                GestureManager.Instance.InitiateGestureKey = vkey;
                GestureManager.Instance.InitiateGestureModifiers = modifiers;
            }
        }

        internal void SetHotKeyString(String HotKeyString)
        {
            HotKeyTextBox.Text = HotKeyString;
        }
    }
}

﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Messaging;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Orbiter
{ 
    public partial class HotKeyWindow : Window
    {
        private static HotKeyWindow _Instance = null;
        private static List<HotKey> registeredKeys = new List<HotKey>();

        public static HotKeyWindow Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new HotKeyWindow();
                }
                return _Instance;
            }
        }

        private HotKeyWindow() : base()
        {
            //make this window invisible.
            InitializeComponent();
            Show();
        }

        const int WM_HOTKEY = 0x0312;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_HOTKEY:
                    //check hotkey register ID
                    int id = wParam.ToInt32();

                    //raise the event with the hotkey ID
                    foreach (HotKey hkey in registeredKeys)
                    {
                        if (hkey.id == id)
                        {
                            hkey.RaiseHotKeyEvent();
                        }
                    }

                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        internal static void RegisterHotKey(HotKey hkey)
        {
            registeredKeys.Add(hkey);
        }
    }
}
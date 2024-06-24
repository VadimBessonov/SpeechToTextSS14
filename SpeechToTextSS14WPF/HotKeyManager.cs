using System;
using System.Windows;
using System.Windows.Interop;

namespace SpeechToTextSS14WPF
{
    internal class HotKeyManager
    {
        private const int HOTKEY_ID = 9000;
        private WindowInteropHelper _helper;
        private HwndSource _source;
        public delegate void MethodContainer();
        public event MethodContainer onHotKeyPresed;

        public HotKeyManager(Window window) 
        {
            _helper = new WindowInteropHelper(window);
            _source = HwndSource.FromHwnd(_helper.Handle);
            _source.AddHook(HwndHook);
        }

        public void OnClose()
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();
        }

        public void RegisterHotKey(uint newHotKey)
        {
            if (!Dlls.RegisterHotKey(_helper.Handle, HOTKEY_ID, 0, newHotKey))
            {
                throw new Exception("Hotkey registration error");
            }
        }

        public void UnregisterHotKey()
        {
            Dlls.UnregisterHotKey(_helper.Handle, HOTKEY_ID);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            onHotKeyPresed();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }
    }
}

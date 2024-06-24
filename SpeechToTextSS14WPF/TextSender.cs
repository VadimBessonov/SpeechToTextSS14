using static SpeechToTextSS14WPF.Dlls;
using System.Windows;
using System;

namespace SpeechToTextSS14WPF
{
    internal static class TextSender
    {
        public static void Enter(string text)
        {
            Clipboard.SetText(text);
            SetWindowActive();
            Send();
        }

        private static void SetWindowActive()
        {
            string className = Dictionary.AppClasses[1];
            IntPtr handle = Find(className);
            SetForegroundWindow(handle);
        }

        private static IntPtr Find(string name)
        {
            IntPtr handle = FindWindow(name, null);
            if (handle == IntPtr.Zero)
            {
                throw new Exception("Can not find window");
                return IntPtr.Zero;
            }
            Console.WriteLine(handle);
            return (handle);
        }

        private static void Send() // sending text by emulating a keyboard
        {
            int keyUp = (int)Dictionary.Keys.KeyUp;

            keybd_event(0x54, 0, 0, 0); // T key
            keybd_event(0x54, 0, keyUp, 0); // T key release
            keybd_event(0x11, 0, 0, 0); // key ctrl
            keybd_event(0x56, 0, 0, 0); // key V
            keybd_event(0x11, 0, keyUp, 0); // key ctrl release
            keybd_event(0x56, 0, keyUp, 0); // key V release
            //Thread.Sleep(10);
            //keybd_event(0xD, 0, 0, 0); // key enter
            //keybd_event(0xD, 0, KeyUp, 0); // key enter release
        }
    }
}

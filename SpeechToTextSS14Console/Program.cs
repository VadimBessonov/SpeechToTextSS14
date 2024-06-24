using System;
using System.Diagnostics;
using System.Windows.Input;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using TextCopy;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using GlobalHotKey;
//using System.Windows.Forms;

namespace SpeechToTextSS14
{
    internal class Program
    {

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName,string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte vk, byte scan, int flags, int extrainfo);

        

        const int KeyUp = 0x2;

        static void Main(string[] args)
        {
            HotKeyManager hotKeyManager = new HotKeyManager();
            IntPtr handle = Find("Qt51513QWindowIcon"); //Find("GLFW30");
            if (handle == IntPtr.Zero) { Console.WriteLine("Programm not finded!!"); return; }
            SetWindowAtcive(handle);
            Console.WriteLine("programm Finded!");

            EnterText("FUARK");


            Console.ReadLine();

        }



        private static void SetWindowAtcive(IntPtr handle)
        {
            SetForegroundWindow(handle);
            
            Process[] procList = Process.GetProcesses();
            foreach (Process a in procList)
            {
                if (a.MainWindowHandle == handle || handle == new IntPtr(a.Id))
                {
                    Console.WriteLine(a.MainWindowTitle);
                    //ShowWindow(a.MainWindowHandle, 3);
                    SetForegroundWindow(a.MainWindowHandle);
                }
            }
            //Thread.Sleep(10);
        }

        private static void EnterText(string text)
        {
            ClipboardService.SetText(text);
            SendText();
        }

        private static void SendText() // sending text by emulating a keyboard
        {
            keybd_event(0x6F, 0, 0, 0); // '/' key
            keybd_event(0x6F, 0, KeyUp, 0); // '/' key release
            keybd_event(0x11, 0, 0, 0); // key ctrl
            keybd_event(0x56, 0, 0, 0); // key V
            keybd_event(0x11, 0, KeyUp, 0); // key ctrl release
            keybd_event(0x56, 0, KeyUp, 0); // key V release

            keybd_event(0xD, 0, 0, 0); // key enter
            keybd_event(0xD, 0, KeyUp, 0); // key enter release
        }

        private static IntPtr Find(string name)
        {
            IntPtr handle = FindWindow(name, null);
            if (handle == IntPtr.Zero)
            {
                Console.WriteLine("bruh");
                return IntPtr.Zero;
            }
            Console.WriteLine(handle);
            return (handle);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Automation;
using System.Windows.Interop;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static SpeechToTextSS14WPF.Dlls;
using System.Security.Policy;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace SpeechToTextSS14WPF // это кошмар, я не хочу это рефакторить
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int KeyUp = 0x2;
        private int _bindedKey = 0xDC; // '\\' key
        private HwndSource _source;
        private const int HOTKEY_ID = 9000;
        private Process _process;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Send(object o, EventArgs e)
        {
            OnHotKeyPressed();
        }

        private void SetBind(object o, EventArgs e)
        {
            
            _bindedKey = '\\';
            if (BindButtonBox.Text != "")
            {
                _bindedKey = BindButtonBox.Text.ToUpper().ElementAt(0);
            }
            MessageBox.Show(Convert.ToString(_bindedKey, 16));
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

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey();
            _process = StartProcess(); // needs to minimize input latency

        }

        private Process StartProcess()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("py");
            Process process = new Process();

            string directory = @"C:\Users\Vadim\Desktop\Программирование\C#\C# important\SpeechToTextSS14\PythonSTT\";
            string script = "PythonSTT.py";

            startInfo.WorkingDirectory = directory;
            startInfo.Arguments = script;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;

            process.StartInfo = startInfo;

            process.Start();
            return process;
        }

        private async void MainMethod()
        {
            _process.StandardInput.WriteLine("start");
            //await Task.Delay(5000);
            string result = await _process.StandardOutput.ReadToEndAsync();
            //_process.Close();

            //MessageBox.Show(result);
            EnterText(result);
            _process = StartProcess();
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();
            _process.Kill();
            base.OnClosed(e);
        }

        private void RegisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            const uint VK_F10 = 0x79;
            const uint MOD_CTRL = 0x0002;

            if (!Dlls.RegisterHotKey(helper.Handle, HOTKEY_ID, 0, 0xDC))
            {
                // handle error
            }
        }



        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            Dlls.UnregisterHotKey(helper.Handle, HOTKEY_ID);
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
                            OnHotKeyPressed();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void OnHotKeyPressed()
        {
            IntPtr handle = Find("GLFW30"); //Find("Qt51513QWindowIcon"); 

            if (handle == IntPtr.Zero) { MessageBox.Show("Programm not finded!!"); return; }

            SetWindowAtcive(handle);
            MainMethod();
            //EnterText("FUARK");
        }
        private static void EnterText(string text)
        {
            Clipboard.SetText(text);
            SendText();
        }

        private static void SendText() // sending text by emulating a keyboard
        {
            keybd_event(0x54, 0, 0, 0); // T key
            keybd_event(0x54, 0, KeyUp, 0); // T key release
            keybd_event(0x11, 0, 0, 0); // key ctrl
            keybd_event(0x56, 0, 0, 0); // key V
            keybd_event(0x11, 0, KeyUp, 0); // key ctrl release
            keybd_event(0x56, 0, KeyUp, 0); // key V release
            //Thread.Sleep(10);
            //keybd_event(0xD, 0, 0, 0); // key enter
            //keybd_event(0xD, 0, KeyUp, 0); // key enter release
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

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string text = BindButtonBox.Text;
            if (text.Length > 1)
            {
                text = text.Remove(1);
                BindButtonBox.Text = text;
            }
        }
    }
}

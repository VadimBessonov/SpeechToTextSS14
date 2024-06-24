using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SpeechToTextSS14WPF
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private int _bindedKey = 0xDC; // '\\' key
        //private HwndSource _source;
        private const int HOTKEY_ID = 9000;
        Translater translater;
        HotKeyManager keyManager;

        public MainWindow()
        {
            Window window = this;
            InitializeComponent();

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

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            keyManager = new HotKeyManager(this);
            
            keyManager.RegisterHotKey((uint)Dictionary.Keys.VK_OEM_5);
            translater = new Translater(); // needs to minimize input latency

            keyManager.onHotKeyPresed += translater.MainMethod;

        }

        protected override void OnClosed(EventArgs e)
        {
            keyManager.OnClose();
            translater.CloseProcess();
            base.OnClosed(e);
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

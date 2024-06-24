using System.Diagnostics;

namespace SpeechToTextSS14WPF
{
    internal class Translater
    {
        private Process _process;


        public Translater()
        {
            _process = StartProcess();
        }

        public async void MainMethod()
        {
            
            _process.StandardInput.WriteLine("start");
            string result = await _process.StandardOutput.ReadToEndAsync();
            TextSender.Enter(result);
            _process = StartProcess();
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

        public void CloseProcess()
        {
            _process.Kill();
        }

    }
}

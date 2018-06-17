using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client;


namespace Client
{
    public class ProgramStart : IDisposable
    {

        private Process p;      //Новый процесс
        private Byte[] buffer;  //Буфер
         
        public ProgramStart(string path)
        {
            p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            //p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; //Видимость окна приложения
            try
            {
                Console.WriteLine("Zapusk progi");
                p.Start();
                Console.WriteLine("Proga zapushena.");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message, "Oshibka pri sozdanii");                
                p.Close();

            }

            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
            p.ErrorDataReceived += new DataReceivedEventHandler(p_ErrorDataReceived);
            p.Exited += p_Exited;

        }
        //Статус завершения
        void p_Exited(object sender, EventArgs e)
        {

        }


        //Бработка и отправка Error 
        private static void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (Program.s != null)
                if (e.Data != "")
                {
                    {
                        Console.WriteLine("Perehvatili vivod: " + e.Data);
                        Program.send(e.Data);
                        //byte[] b = Encoding.GetEncoding(1251).GetBytes(e.Data);
                        //writeOutput(Encoding.GetEncoding(866).GetString(b));
                    }
                }
        }
        
        
        // Бработка и отправка OutputData
        private static void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (Program.s != null)
                if (e.Data != "")
                {
                    {
                        Console.WriteLine("Perehvatili vivod: " + e.Data);
                        Program.send(e.Data);
                        //byte[] b = Encoding.GetEncoding(1251).GetBytes(e.Data);
                        //writeOutput(Encoding.GetEncoding(866).GetString(b));
                    }
                }
            //Thread.Sleep(5000);
        }
        

        //Запись в консоль
        public void sendCommandfunc(string command)
        {
            p.StandardInput.WriteLine(command);
        }


        //Закрытие запущеной програмы
        public void Dispose()
        {
            if (p != null)
            {
                try
                {
                    p.Kill();
                }
                catch { }
            }
        }
    }
}

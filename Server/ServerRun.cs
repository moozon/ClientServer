using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;


namespace Server
{
    public partial class ServerRun
    {
        

            private Socket listenSocket; //Сокет для входящих подключений
            private Socket listener;     //Второй Сокет для входящих подключений
            private SocketAsyncEventArgs sava;
            private IAsyncResult ar1;    //Информация о новом сокете
            //private string formname;
            public List<FormClient> listOfForms2;//Коллекция форм клиентов
            public bool _isaccept = false;       //Флаг статуса сервера


            //Конструктор ServerRun2
            public ServerRun()
            {
                //MessageBox.Show("ServerRun");
                listOfForms2 = new List<FormClient>();
                listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listenSocket.Bind(new IPEndPoint(IPAddress.Any, 2200));
                //listenSocket.Listen(1); 
                //if (_isaccept)
                //listenSocket.BeginAccept(new AsyncCallback(ReceiveCallback), listenSocket);
                //listenSocket.Blocking = true;
            }


            //Старт Сервер
            public void startServer()
            {
                listenSocket.Listen(10);
                _isaccept = true;
                listenSocket.BeginAccept(ReceiveCallback, listenSocket);

                //listenSocket.BeginAccept(new AsyncCallback(ReceiveCallback), listenSocket);
                //listenSocket.Blocking = false;
                //if (listenSocket.Blocking != true)
                //{
                //    listenSocket.Listen(1);
                //    listenSocket.BeginAccept(new AsyncCallback(ReceiveCallback), listenSocket);
                //    MessageBox.Show("Запустили сокет");
                //}
                //else
                //{
                //    listenSocket.Blocking = false;
                //    MessageBox.Show("Возобновили прием");
                //}
            }


            // Стоп Сервер
            public void stopServer()
            {
                _isaccept = false;
                //listenSocket.
                //listenSocket.Listen(0);
                //IAsyncResult ar = (IAsyncResult)Object;
                //listenSocket.EndAccept(ar);
                //listenSocket.Blocking = true;
                //MessageBox.Show("Остановили прием");
                //listenSocket.EndAccept()
            }


            void sava_Completed(object sender, SocketAsyncEventArgs e)
            {
                e.AcceptSocket.Close();
                //new Form2(e.AcceptSocket);

            }



            private void connect1()
            {
                if (listenSocket.AcceptAsync(sava))
                {
                    //new Form2(sava.AcceptSocket);


                }
                Thread.Sleep(1000);
            }


            // Ассинхронный прием
            public void ReceiveCallback(IAsyncResult ar)
            {
                ar1 = ar;
                Thread th = new Thread(form2Create);
                th.Start();
                if (_isaccept == true)
                    // После того как завершили соединение, ждем новое
                    listenSocket.BeginAccept(ReceiveCallback, listenSocket);
                Thread.Sleep(500);
            }



            //Создание окна для нового клиента
            private void form2Create()
            {

                FormClient myForm2 = new FormClient((Socket)ar1.AsyncState, ar1); //Создаем новое окно
                //myForm2.Name = ((Socket)ar1.AsyncState).RemoteEndPoint.ToString();
                listOfForms2.Add(myForm2); // Добавляем в массив

            }



        }

    
}

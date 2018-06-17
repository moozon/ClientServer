using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Server
{
    public partial class FormServer : Form          //Главная форма и логика
    {
        
        public static FormClient formClient;        //Форма клиента 
        public static ServerRun sRun;               //Сервер
        public delegate void AddRows(Socket socket);//Делегат для добавления записей о новых клиентах в датагрид
        public AddRows myDelegate;                  //Переменная делегата для добавления записей о новых клиентах в датагрид  
        public delegate void DellRows(string s);    //Делегат для удаления записей о новых клиентах из датагрид
        public List<Socket> sockets;                //Коллекци клиентов
        
        public FormServer()
        {
            
            InitializeComponent();
            links.EventHandler = new links.MyEvent(addRows);
            links.EventHandler1 = new links.MyEvent1(dellRows);
            myDelegate = new AddRows(addRowsMethod);           
            sRun = new ServerRun();            
           

        }
        
        
        
        //Удаление клиентов из датагрид
        private void dellRows(string s)
        {
            this.Invoke(new DellRows(dellRowsMethod),s);
        }

        
        // Метод используемый делегатом для удаления клиента из датагрид
        private void dellRowsMethod(string s)
        {
            ////MessageBox.Show(s);
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                try
                {
                    if (s == dr.Cells[1].Value.ToString())
                        dataGridView1.Rows.Remove(dr);
                }
                catch (Exception        e)
                {
                    MessageBox.Show(e.Message);
                }
            }      
        }
        

        //Выход из программы
        private void button1_Click(object sender, EventArgs e)
        {
                       
            Application.Exit();

        }
        

        // Добавляем нового клиента в датагрид
        public void addRows(Socket socket)
        {
            this.Invoke(this.myDelegate, socket); //Вызов Invoke Для выполнения метода в своем потоке 
        }
       

        // Метод используемый делегатом для добавления нового клиента в датагрид
        public void addRowsMethod(Socket socket)
        {
            try
            {
                DataGridViewRow dr = new DataGridViewRow();
                dr.CreateCells(dataGridView1);
                dr.SetValues(new object[] { socket.ProtocolType.ToString(), socket.RemoteEndPoint.ToString() });
                dataGridView1.Rows.AddRange(dr);
                dr.Dispose();
            }
            catch { }
            
        }
        


        //Класс Сервера
        public partial class ServerRun2
        {
            
            private Socket listenSocket; //Сокет для входящих подключений
            private Socket listener;     //Второй Сокет для входящих подключений
            private SocketAsyncEventArgs sava;
            private IAsyncResult ar1;    //Информация о новом сокете
            //private string formname;
            public List<FormClient> listOfForms2;//Коллекция форм клиентов
            public bool _isaccept = false;       //Флаг статуса сервера


            //Конструктор ServerRun2
            public ServerRun2()
            {
                //MessageBox.Show("FormServer");
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
                if (_isaccept==true)
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
       
        
        //Открыте окна клиента
        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                string str = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
            }
            catch
            {
                MessageBox.Show("Выберете подключенного клиента", "Внимание !!!");
            }

                foreach (FormClient form in sRun.listOfForms2)
                {
                    try
                    {
                        if (form.Text == dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString() || form.Text == dataGridView1[e.ColumnIndex + 1, e.RowIndex].Value.ToString())
                        //if (form.Text == dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString())
                            form.Show();
                    }
                    catch {  }
                }
                
            
        }


        //Обновдение информации в Датагрид
        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Refresh();
        }

 
        //StartService
        private void buttonStartService_Click(object sender, EventArgs e)
        {
                ((Button)sender).Enabled = false;
                buttonStopService.Enabled = true;                            
                sRun.startServer();            
        }
       
        
        //StopService
        private void buttonStopService_Click(object sender, EventArgs e)
        {
                ((Button)sender).Enabled = false;
                buttonStartService.Enabled = true;                
                sRun.stopServer();            
        }
    }
}

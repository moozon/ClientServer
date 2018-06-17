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
using System.IO;
using System.Data.SqlClient;


namespace Server
{
   
    public partial class FormClient : Form
    {
        private string strCmd;      //Строка команды
        public int countRecive;     //Размер буфера
        //public string Name;
        public Socket s;            //Сокет 
        public byte[] buffer;       //Буффер
        private SqlConnection con;  //Sql соединение        
        private SqlCommand com;     //Sql команд
        //int recived = 0;
        public SocketAsyncEventArgs socketAsyncEventArgs;
        public delegate void AddListItem();//Делегат дял записи в текстбокс
        public AddListItem myDelegate;     //Экземпляр Делегата дял записи в текстбокс


        public FormClient(Socket socket, IAsyncResult ar)
        {
            
            myDelegate = new AddListItem(addTextMethod);
            buffer = new byte[1024];            
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Thread th = new Thread(recive);             //Поток для приема даннх
            th.Start();
            Thread th2 = new Thread(connectionFlag);    //Поток для проверки доступности клиента
            th2.Start();

            try
            {
                s = socket.EndAccept(ar);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }




            if (this.IsHandleCreated)
            {
                this.Text = s.RemoteEndPoint.ToString();
                insertDb(s.RemoteEndPoint.ToString(), s.RemoteEndPoint.ToString(), DateTime.Now.ToString());
            }
            else
            {
                Thread.Sleep(500);
                try
                {
                    this.Text = s.RemoteEndPoint.ToString();
                    insertDb(s.RemoteEndPoint.ToString(), s.RemoteEndPoint.ToString(), DateTime.Now.ToString());
                }
                catch (Exception e)
                {
                    //MessageBox.Show(e.Message);
                }
            }
            links.EventHandler(s);

            
            InitializeComponent();            
        }


        //Запись в MS Sql Дб
        private void insertDb(string ip, string name, string date)
        {
            string stringCmd = string.Format("INSERT INTO clients VALUES('{0}','{1}','{2}')", ip, name, date);
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            csb.DataSource = "localhost";
            csb.InitialCatalog = "mydb";
            csb.UserID = "admin";
            csb.Password = "admin";
            csb.ConnectTimeout = 1;

            con = new SqlConnection(csb.ConnectionString);
            try
            {
                con.Open();        
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }

            com = new SqlCommand(stringCmd, con);

            try
            {
                com.ExecuteNonQuery();                

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }


        //Флаг подключения клиента
        private void connectionFlag()
        {
            while (true)
            {
                try
                {
                    s.Send(new byte[1024]);
                }
                catch
                {
                    s.Close();
                    //MessageBox.Show("Sockeck zakrit tak kak bolshe ne dostupen");
                    break;                    
                }
                Thread.Sleep(1000);
            }
        }


        // Метод для делегата записи входящих данных в текстбокс
        private void addTextMethod()
        {
            textBox1.Text += Encoding.GetEncoding(1251).GetString(buffer) + Environment.NewLine;            
        }

        void socketAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    //ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    //ProcessSend(e);
                    break;
            }


        }


        
        //Запись входящих данных в текстбокс
        private void processReceive(SocketAsyncEventArgs e)
        {
            
            textBox1.Text += Encoding.ASCII.GetString(e.Buffer, 0, e.Buffer.Length) + Environment.NewLine;
            
            //s.ReceiveAsync(socketAsyncEventArgs);
        }


        //Прием данных
        private void recive()
        {
            while(true)
            {
                if (s != null)
                {
                    try
                    {

                        s.Receive(buffer);
                        try
                        {
                            if (this.IsHandleCreated)
                                this.Invoke(myDelegate);
                        }
                        catch (Exception e1)
                        {
                            MessageBox.Show(e1.Message);
                        }

                    }
                    catch (ObjectDisposedException )
                    {
                        //MessageBox.Show(e.Message, "");
                        break;
                        
                        
                    }
                    catch (SocketException )
                    {
                        //MessageBox.Show(e.Message);
                    }
                    //MessageBox.Show(e.Message);

                }
                else
                    break;
            Thread.Sleep(3000);
            }
        }

        
        //Асинхронный Прием данных
        private void recive(IAsyncResult ar)
        {
            //Array.Clear(buffer,buffer[0],buffer.Length);
            //buffer.

            try
            {
                countRecive = s.EndReceive(ar);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                try
                {
                    if (this.IsHandleCreated)
                        this.Invoke(myDelegate);
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }


            }
            //MessageBox.Show(Encoding.GetEncoding(1251).GetString(buffer));
            //byte[] buffer1 = new System.Text.ASCIIEncoding().GetBytes(ar.ToString());
            if (this.IsHandleCreated)
                Invoke(myDelegate);

            Thread.Sleep(2000);
            if (s != null)
            {
                try
                {
                    s.BeginReceive(buffer, buffer[0], countRecive, SocketFlags.None, new AsyncCallback(recive), s);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "");
                }
            }
        }

       
        //Скрыть форму
        private void button2_Click(object sender, EventArgs e)
        {
            
            this.Hide();
        }
       
        
        //Отправить каманду
        private void button1_Click(object sender, EventArgs e) //Кнопка оправляет команду
        {
            strCmd = textBox2.Text;
            if (strCmd != "")
            {
                if (send(strCmd))
                {
                    if (strCmd == "cmd")
                    {
                        var th = new Thread(programStart);
                        labelProgramName.Text = strCmd;
                    }
                    textBox2.Text = "";
                }
            }

        }

        private void programStart(object obj)
        {
            throw new NotImplementedException();
        }

        
        //Закрытие формы
        private void button3_Click(object sender, EventArgs e)
        {
            if (s != null)
            {
                s.Close();
            }
            links.EventHandler1(this.Text);
            this.Dispose();
            this.Close();
        }


        //Отправка команды по нажатию клавиши Enter
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            //MessageBox.Show(Keys.Enter.ToString());
            //MessageBox.Show(e.KeyChar);
            if (e.KeyChar == (char)Keys.Enter)
            {
                try
                {
                    s.Send(new System.Text.ASCIIEncoding().GetBytes(textBox2.Text));
                    textBox2.Text = "";
                }
                catch
                {
                    MessageBox.Show("Клиент отключен.");
                }
            }

        }


        //Отправка файла
        private void button4_Click(object sender, EventArgs e)  
        {
            try
            {
                s.Send(new System.Text.ASCIIEncoding().GetBytes("filerecive"));
            }
            catch
            {

            }
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                long filesize = new FileInfo(openFileDialog1.FileName).Length;
                s.Send(new System.Text.ASCIIEncoding().GetBytes(filesize.ToString()));
                s.SendFile(openFileDialog1.FileName);
            }
        }
           
       
        //Метод Отправки команды
        private bool send(string st)
        {

        try
            {
                s.Send(new System.Text.ASCIIEncoding().GetBytes(st));
                return true;
            }
        catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

        }
        
        
        //Закрытие формы при закрытие крестиком
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {

            if (s != null)
            {
                s.Close();
            }
            links.EventHandler1(this.Text);
            this.Dispose();
            this.Close();
        }
                      

    }
}

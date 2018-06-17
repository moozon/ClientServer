using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace Server
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormServer());            
        }
    }

    // Класс делегатов для доступа к другим классам
    public static class links
    {
       
       //public string[] formearrey;
       //public string[] socketarrey;
       public delegate void MyEvent(Socket socket);
       public static MyEvent EventHandler;
       public delegate void MyEvent1(string s);
       public static MyEvent1 EventHandler1;

    }



}

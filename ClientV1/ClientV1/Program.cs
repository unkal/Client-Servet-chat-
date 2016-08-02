using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientV1
{
    public static class StatClass
    {
        //Данная переменная статического класса будет доступна откуда угодно в пределах проекта
        public static String nik_info;
        public static String ip_info;
        public static String port_info;
        public static string pm_name;
    }
    public static class SendClass
    {
        public delegate void MyEvent(string data);
        public static MyEvent EventHandler;
    }
    public static class RecvClass
    {
        public delegate void MyEvent(string data);
        public static MyEvent EventHandler;
    }
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
            Application.Run(new Form1());
        }
    }
}

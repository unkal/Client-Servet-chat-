using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace SocketServer
{
    public static class StatClass
    {
        public static String nik_info;
    }  
    class ThreadedServer
    {
        private Socket serverSocket;
        private int _port;
        public ThreadedServer(int port) { _port = port; }
        private class ConnectionInfo
        {
            public Socket Socket;
            public Thread Thread;
            public string Nik;
        }
        private Thread acceptThread;
        private List<ConnectionInfo> connections = new List<ConnectionInfo>();
        public void Start()
        {
            // Получаем информацию о локальном компьютере
            IPAddress ipAddr = IPAddress.Parse("192.168.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, _port);
            // Создаем сокет, привязываем его к адресу
            // и начинаем прослушивание
            serverSocket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(10);
            acceptThread = new Thread(AcceptConnections);
            acceptThread.IsBackground = true;
            acceptThread.Start();
        }
        private void AcceptConnections()
        {
            while (true)
            {
                // Принимаем соединение
                Socket socket = serverSocket.Accept();
                ConnectionInfo connection = new ConnectionInfo();
                connection.Socket = socket;


                string data = "";
                byte[] buffer = new byte[100];
                int bytesRead = connection.Socket.Receive(buffer);
                data += Encoding.UTF8.GetString(buffer, 0, bytesRead);
                connection.Nik = data;

                // Создаем поток для получения данных
                connection.Thread = new Thread(ProcessConnection);
                connection.Thread.IsBackground = true;
                connection.Thread.Start(connection);
                // Сохраняем сокет
                lock (connections) connections.Add(connection);
            }
        }
        private void ProcessConnection(object state)
        {
            ConnectionInfo connection = (ConnectionInfo)state;
            byte[] buffer = new byte[800];
            try
            {
                while (true)
                {
                    int bytesRead = connection.Socket.Receive(buffer);
                    string data="",name="&",time="",nik="";             
                    DateTime dat=DateTime.Now;
                    time = dat.ToLongTimeString();
                    data += Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    int s = data.IndexOf(":");
                    int d = data.Length;
                    nik = data.Substring(0,s);
                    lock (connections)
                    {
                        foreach (ConnectionInfo conn in connections)
                        {
                            name = name + conn.Nik+"&";
                        }
                        name =name + "<endnik>";
                    }

                    data = data.Substring(s+1,d-s-1);
                    data = nik + "\t \t \t" + time + "\n" + data + name;
                    name = "&";
                    byte[] msg = Encoding.UTF8.GetBytes(data);
                    Console.Write("Полученный текст: " + data + "\n");

                    if (bytesRead > 0)
                    {
                        lock (connections)
                        {
                            foreach (ConnectionInfo conn in connections)
                            {
                                    conn.Socket.Send(msg);
                            }
                          
                        }
                    }
                    else if (bytesRead == 0) return;
                    
                }
            }
            catch (SocketException exc)
            {
                Console.WriteLine("Socket exception: " + exc.SocketErrorCode);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception: " + exc);
            }
            finally
            {
                connection.Socket.Close();
                lock (connections) connections.Remove(connection);
            }
        }
    }
    class Program
    {
         static void Main(string[] args)
        {
            ThreadedServer ts = new ThreadedServer(11000);
            ts.Start();
            Console.ReadLine();
        }
    }
}
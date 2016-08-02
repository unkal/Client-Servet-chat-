using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace ClientV1
{
    public partial class Form1 : Form
    {
        static private Socket Client;
        private IPAddress ip = null;
        private int port = 0;
        private Thread th;
        Form3 form;
        public Form1()
        {
            InitializeComponent();
            button1.Enabled = false;
            try
            {
                var sr = new StreamReader(@"Options/Options.txt");
                string buffer = sr.ReadToEnd();
                sr.Close();
                int ip_lenght = buffer.IndexOf(":");
                int port_lenght = buffer.IndexOf("/");
                int lengh = buffer.Length;
                StatClass.ip_info = buffer.Substring(0, ip_lenght);
                StatClass.port_info = buffer.Substring(ip_lenght + 1, port_lenght - ip_lenght - 1);
                StatClass.nik_info = buffer.Substring(port_lenght + 1, lengh - port_lenght - 3);

                ip = IPAddress.Parse(StatClass.ip_info);
                port = int.Parse(StatClass.port_info);

                label4.ForeColor = Color.Green;
                label4.Text = "Options: \n IP server:" + StatClass.ip_info + "\n Port:" + StatClass.port_info + "\n Ваш ник:" + StatClass.nik_info;
                SendClass.EventHandler = new SendClass.MyEvent(SendMessage);               
            }
            catch(Exception ex)
            {
                label4.ForeColor = Color.Red;
                label4.Text = "Options error"+ex.ToString();
                Form2 fr = new Form2();
                fr.Show();
            }

        }
private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 fr = new Form2();
            fr.Show();
        }
void SendMessage(string message)
   {
    if (message!="" && message!=" ")
       {
                byte[] buffer = new byte[1024];
                buffer = Encoding.UTF8.GetBytes(message);
                Client.Send(buffer);
            }
        }
void RecvMessage()
{
  byte[] buffer = new byte[1024];
    for (int i=0;i<buffer.Length;i++)
    {
        buffer[i] = 0;
    }
    for (; ;)
    {
        try
        {
            Client.Receive(buffer);
            string message = Encoding.UTF8.GetString(buffer);
            string ClientsConnect = "";
            string PM_Message=message.Substring(0,2);
            string PM_PM = message;
            int count = message.IndexOf("<ENDMSG>");
            if (count == -1)
            {
                continue;
            }
            ClientsConnect = message.Substring(message.IndexOf("<ENDMSG>&")+9);
            ClientsConnect = ClientsConnect.Substring(0,ClientsConnect.IndexOf("&<endnik>"));
            string[] words = ClientsConnect.Split(new char[] { '&' });
            string Clear_message = "";           
            Clear_message = message.Substring(0,count);
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = 0;
            }           
            this.Invoke((MethodInvoker)delegate()
            {
                listBox1.Items.Clear();
                foreach(string asb in words)
                {
                    listBox1.Items.Add(asb);
                }               
                if (PM_Message=="PM")
                {
                string PM_NIK = PM_PM.Substring(PM_PM.IndexOf("PM+") + 3, PM_PM.IndexOf("+PM") - 3);
                string[] wo = PM_NIK.Split(new char[] { '[' });
                if (wo[0] == StatClass.nik_info || wo[1] == StatClass.nik_info)
                        { 
                PM_PM = PM_PM.Substring(PM_PM.IndexOf("+PM") + 3, PM_PM.IndexOf("<ENDMSG>")-PM_PM.IndexOf("+PM")-3);
               
                     if (form ==null || form.IsDisposed)
                         {
                          StatClass.pm_name = PM_NIK;
                          form = new Form3();
                          form.Show();
                         }                
                RecvClass.EventHandler(PM_PM);
                            }
                }
                else
                {
                    richTextBox1.AppendText(Clear_message);
                }
            });
        }
        catch(Exception ex)
        {
        }
    }
}
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (label4.Text != "Options error")
                {
                    button1.Enabled = true;
                    richTextBox2.Enabled = true;
                    Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    if (ip != null)
                    {
                        Client.Connect(ip, port);
                        string message = StatClass.nik_info;
                        byte[] buffer = new byte[1024];
                        buffer = Encoding.UTF8.GetBytes(message);
                        Client.Send(buffer);
                        th = new Thread(delegate() { RecvMessage(); });
                        th.Start();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show( "Server Shutdown \n"+ex.ToString(),"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button1.Enabled = false;
                richTextBox2.Enabled = false;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (richTextBox2.Text!="")
            {
            SendMessage("\n" + StatClass.nik_info + ":" + richTextBox2.Text + "<ENDMSG>");
            richTextBox2.Clear();
            }
        }
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (th!=null) th.Abort();
            if (Client !=null)
            {
                Client.Close();
            }
            Application.Exit();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (th != null) th.Abort();
            if (Client != null)
            {
                Client.Close();
            }
            Application.Exit();
        }
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if(listBox1.SelectedItem!=null)
            {
            StatClass.pm_name = listBox1.SelectedItem.ToString();
            form = new Form3();
            form.Show();
            }
        }
    }
}

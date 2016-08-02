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

namespace ClientV1
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        RecvClass.EventHandler = new RecvClass.MyEvent(func);
        }
        void func(string param)
        {
            richTextBox1.AppendText(param);
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            this.Text = StatClass.pm_name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (richTextBox2.Text != "")
            {
            SendClass.EventHandler("PM+"+StatClass.pm_name+"["+StatClass.nik_info+"+PM"+"\n"+ StatClass.nik_info + ":" + richTextBox2.Text + "<ENDMSG>");
                richTextBox2.Clear();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ClientV1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            textBox1.Text = StatClass.ip_info;
            textBox2.Text = StatClass.port_info;
            textBox3.Text = StatClass.nik_info;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text!="" && textBox2.Text!="" && textBox1.Text!=" " && textBox2.Text!=" ")
            {
                try
                {
                    DirectoryInfo data = new DirectoryInfo("Options");
                    data.Create();
                    var sw = new StreamWriter("Options/Options.txt");
                    sw.WriteLine(textBox1.Text + ":" + textBox2.Text+"/"+textBox3.Text);
                    sw.Close();
                    this.Hide();
                    Application.Restart();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error: "+ex.Message);
                }
            }
        }
    }
}

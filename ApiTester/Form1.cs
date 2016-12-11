using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Pipes;
using System.IO;
using RetroShareApi.Connection;

namespace RSApiTester
{
	public partial class Form1 : Form
	{
		HTTPConnection client;

		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (textBox1.Text != "" && client == null)
			{
				client = new HTTPConnection( new Uri(textBox1.Text));
				button1.Enabled = false;
				button2.Enabled = true;
			}
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			client = null;
		}

		string sendRequest(string sector, string function, string data)
		{
            return client.sendRequest(new Request(sector, function, data));
		}

        private void button2_Click(object sender, EventArgs e)
        {
            try {
                string txt = sendRequest(textBox2.Text, textBox3.Text, textBox5.Text);
                textBox4.Text = txt;
            } catch(Exception ex)
            {
                textBox4.Text = ex.ToString();
            }
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			StreamReader file = null;
			try
			{
				file = new StreamReader("defaultconnection.txt");
				textBox1.Text = file.ReadToEnd();
			}
			catch
			{
			}
			finally
			{
				if (file != null)
					file.Dispose();
			}
		}
	}
}

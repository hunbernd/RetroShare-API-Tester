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

namespace RSApiTester
{
	public partial class Form1 : Form
	{
		NamedPipeClientStream client;


		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (textBox1.Text != "" && client == null)
			{
				client = new NamedPipeClientStream(textBox1.Text);
				client.Connect();
				button1.Enabled = false;
				button2.Enabled = true;
			}
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			if(client != null) client.Dispose();
			client = null;
		}

		string sendRequest(string sector, string function)
		{
			if(client == null) return "";

			string url = sector;
			if (!String.IsNullOrWhiteSpace(function))
			{
				url += "/" + function;
			}
			byte[] buffer = ASCIIEncoding.UTF8.GetBytes(url);
			client.Write(buffer, 0, buffer.Length);

			client.Write(buffer, 0, buffer.Length);

			MemoryStream concat = new MemoryStream();
			byte[] readbuf = new byte[128];
			int rc;
			do
			{
				rc = client.Read(readbuf, 0, readbuf.Length);
				concat.Write(readbuf, 0, rc);
			} while (rc > 0 && readbuf[rc - 1] != 0);

			return ASCIIEncoding.UTF8.GetString(concat.GetBuffer(), 0, (int)concat.Length - 1);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			string txt = sendRequest(textBox2.Text, textBox3.Text);
			textBox4.Text = txt;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			StreamReader file = null;
			try
			{
				file = new StreamReader("defaultpipe.txt");
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

using KynetLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KynetServer
{
    public partial class Form1 : Form
    {
        public static Form1 form;

        public Form1()
        {
            InitializeComponent();
            form = this;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Server server = new Server();
            server.Open();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Server.ConnectedClients.Where(f => f.Username == Form1.form.listBox1.SelectedItem.ToString()).FirstOrDefault().callback.Message(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Server.ConnectedClients[0].callback.DownloadAsync(textBox2.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            foreach (FileTransfer file in Server.FileTransfers)
            {
                string username = Server.ConnectedClients.Where(f => f.Fingerprint == file.Fingerprint).FirstOrDefault().Username;
                dataGridView1.Rows.Add(file.ID, file.FileName, file.FileSize / 1024 / 1024, username, file.transferType.ToString(), file.Progress, file.Completed, file.Failed, file.Failed ? file.Error : "");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}

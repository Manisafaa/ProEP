using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using Shat.ShatService;

namespace Shat
{
    public partial class Form1 : Form, IShatCallback
    {
        public ShatClient proxy;
        string username;

        public Form1()
        {
            InstanceContext context = new InstanceContext(this);
            proxy = new ShatClient(context);

            InitializeComponent();

            listBox1.Hide();
            textBox1.Hide();
            button1.Enabled = false;
            button1.Hide();
            button2.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            username = textBox2.Text;
            button2.Hide();
            textBox2.Hide();
            listBox1.Show();
            textBox1.Show();
            button1.Show();

            ShatMessage[] lastMessages = proxy.GetLastMessages();

            for (int i = 0; i < lastMessages.Count(); ++i)
                listBox1.Items.Add(lastMessages[i].username + ": " + lastMessages[i].text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
                button2.Enabled = true;
            else
                button2.Enabled = false;
        }

        public void BroadcastMessage(ShatMessage message)
        {
            listBox1.Items.Add(message.username + ": " + message.text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShatMessage message = new ShatMessage();
            message.username = username;
            message.text = textBox1.Text;
            textBox1.Text = "";

            proxy.SendMessage(message);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                button1.Enabled = false;
            else
                button1.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            proxy.Unsubscribe();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace project_P2P_test
{
    public partial class ChatForm : Form
    {
        delegate void AddMessage(string message);

        string userName;

        const int port = 54545;
        const string broadcastAddress = "255.255.255.255";
        UdpClient receivingClient;
        UdpClient sendingClient;

        Thread receivingThread;

        public ChatForm()
        {
            InitializeComponent();
            
            //this.Load += new EventHandler(ChatForm_Load); // it is already add
            // btnSend.Click += new EventHandler(btnSend_Click); // already present
        }

        void ChatForm_Load(object sender, EventArgs e)
        {
            this.Hide();

            using (LoginForm loginForm = new LoginForm())
            {
                loginForm.ShowDialog(this);

                if (loginForm.UserName == "")
                {
                    this.Close();
                }
                    
                else
                {
                    userName = loginForm.UserName;
                    this.Show();
                }
            }

            tbSend.Focus();
            
            InitializeSender();
            InitializeReceiver();
        }

        private void InitializeSender()
        {
            sendingClient = new UdpClient(broadcastAddress, port);
            sendingClient.EnableBroadcast = true;
            Console.WriteLine("UDP port sending : " + ((IPEndPoint)sendingClient.Client.LocalEndPoint).Port.ToString());
        }

        private void InitializeReceiver()
        {
            receivingClient = new UdpClient(port); // could be 0 to let the system find the first free one automaticly
            //0 as the constructor parameter set the app to automatically find free udp port. ((IPEndPoint)udpClient.Client.LocalEndPoint)).Port.ToString() is used to find the port number.

            Console.WriteLine("UDP port receiver : " + ((IPEndPoint)receivingClient.Client.LocalEndPoint).Port.ToString());

            ThreadStart start = new ThreadStart(Receiver);
            receivingThread = new Thread(start);
            receivingThread.IsBackground = true;
            receivingThread.Start();
        }

        void btnSend_Click(object sender, EventArgs e)
        {
            tbSend.Text = tbSend.Text.TrimEnd();

            if (!string.IsNullOrEmpty(tbSend.Text))
            {
                string toSend = userName + ":\n" + tbSend.Text;
                byte[] data = Encoding.ASCII.GetBytes(toSend);
                sendingClient.Send(data, data.Length);
                tbSend.Text = "";
            }

            tbSend.Focus();
        }

        private void Receiver()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            AddMessage messageDelegate = MessageReceived;

            while (true)
            {
                byte[] data = receivingClient.Receive(ref endPoint);
                string message = Encoding.ASCII.GetString(data);
                Console.WriteLine("message recue : {0}",message);
                Invoke(messageDelegate, message);
            }
        }

        private void MessageReceived(string message)
        {
            rtbChat.Text += message + "\n";
        }

    }
}


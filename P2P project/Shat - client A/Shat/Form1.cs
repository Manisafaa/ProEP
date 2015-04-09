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

using System.Net.PeerToPeer;
using System.Net;

using System.Net.Sockets;
using System.Threading;



namespace Shat
{
    public partial class Form1 : Form, IShatCallback
    {
        public ShatClient proxy;
        string username; //Username of user

        string p2pUsername; //UserName of other user to connect to

        PeerName myPeer, mySecurePeer;
        PeerNameRegistration registration, registeration2;

        



        //Added for P2P connectivity
        
        delegate void AddMessage(string message);

        

        const int port = 3030;
        const string broadcastAddress = "255.255.255.255";

        UdpClient receivingClient;
        UdpClient sendingClient;

        Thread receivingThread;

        string receivingAddress;
        int receivingPort;

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

            p2pUserNameBox.Hide();
            getIpBtn.Hide();

        }

        void RegisterToP2P(){

            //myPeer = new PeerName("MyUnsecurePeer", PeerNameType.Unsecured);
            mySecurePeer = new PeerName(username, PeerNameType.Secured);
            //registration = new PeerNameRegistration(myPeer, 3030);
            registeration2 = new PeerNameRegistration(mySecurePeer, 3031);
            //registration.Start();
            registeration2.Start();

            //Console.WriteLine("Registeration of Peer {0} comeplete with the hostname {1}.", myPeer.ToString(), myPeer.PeerHostName);
            Console.WriteLine("Registeration of Peer {0} comeplete with the hostname {1}.", mySecurePeer.ToString(), mySecurePeer.PeerHostName);

            //string ipaddress = ipFromPeer(mySecurePeer);

            //Console.WriteLine("Press any key to continue...");
            //Console.ReadKey();

            // registeration.Stop();
            // registeration2.Stop();
        }

        void Resolve()
        {
            if(registration!=null){
                registration.Stop();
                //registeration2.Stop();
            }
            PeerName myPeerB = new PeerName(p2pUsername, PeerNameType.Secured);

            receivingAddress = ipFromPeer(myPeerB);
            receivingPort = 3030;
            InitializeReceiver(receivingAddress, receivingPort);
            
        }

        public string ipFromPeer(PeerName peername) {
            PeerNameResolver resolver = new PeerNameResolver();

            PeerNameRecordCollection results = resolver.Resolve(peername);

            Console.WriteLine("{0} Peers Found:", results.Count.ToString());
            //listBox1.Items.Add(String.Format("{0} Peers Found:", results.Count.ToString()));
            int i = 1;
            string ipAddress = "";

            //foreach (PeerNameRecord peer in results)
            //{
            if (results.Count>0){
            PeerNameRecord peer = results.FirstOrDefault();
                String logA = String.Format("{0} Peer:{1}", i++, peer.PeerName.ToString());
                Console.WriteLine(logA);
                //listBox1.Items.Add(logA);

                foreach (IPEndPoint ip in peer.EndPointCollection)
                {
                    String logB = "ipAddress:" + ip.ToString();
                    Console.WriteLine(logB);
                    ipAddress = ip.Address.ToString();
                }
            }

            return ipAddress;
        }


        private void InitializeSender()
        {
            sendingClient = new UdpClient(broadcastAddress, port);
            sendingClient.EnableBroadcast = true;
        }

        private void InitializeReceiver(string address, int port)
        {
            receivingClient = new UdpClient(port);

            ThreadStart start = new ThreadStart(Receiver);
            receivingThread = new Thread(start);
            receivingThread.IsBackground = true;
            receivingThread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            username = textBox2.Text;
            button2.Hide();
            textBox2.Hide();
            listBox1.Show();
            textBox1.Show();
            button1.Show();

            p2pUserNameBox.Show();
            getIpBtn.Show();

            RegisterToP2P();

            InitializeSender();
            //InitializeReceiver();

            /*
            if (discoveryMode)
            {
                Resolve();
            }*/

            /*
            ShatMessage[] lastMessages = proxy.GetLastMessages();

            for (int i = 0; i < lastMessages.Count(); ++i)
                listBox1.Items.Add(lastMessages[i].username + ": " + lastMessages[i].text);*/
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

       
        private void Receiver()
        {
            Console.WriteLine("receivingAddress:"+receivingAddress);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(receivingAddress), receivingPort);
            AddMessage messageDelegate = MessageReceived;

            while (true)
            {
                byte[] data = receivingClient.Receive(ref endPoint);
                string message = Encoding.ASCII.GetString(data);
                Console.WriteLine("message:{0}", message);
                Invoke(messageDelegate, message);
            }
        }

        private void MessageReceived(string message)
        {
            Console.WriteLine("message:{0}", message);
        }

        private void getIpBtn_Click(object sender, EventArgs e)
        {
            p2pUsername = p2pUserNameBox.Text;
            Resolve();

            string toSend = "hello world";
            byte[] data = Encoding.ASCII.GetBytes(toSend);
            sendingClient.Send(data, data.Length);

        }
    }
}

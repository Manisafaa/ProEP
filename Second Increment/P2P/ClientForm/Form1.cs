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
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Lidgren.Network;
using NetworkConnect;


namespace ClientForm
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public partial class Chat : Form, IPrivateChat
    {
        List<IPAddress> IPs = new List<IPAddress>();
        int basePort = 14242;

        User self;
        int selected_conversation = -1;
        List<User> private_list = new List<User>();
        List<List<string>> conversations = new List<List<string>>();

        ServiceHost selfHost;

        List<ServiceHost> hosts = new List<ServiceHost>();
        List<ChannelFactory<IPrivateChat>> channelFactories = new List<ChannelFactory<IPrivateChat>>();
        List<IPrivateChat> channels = new List<IPrivateChat>();

        //messages
        public NetPeer Peer { get; set; }
        public NetPeerConfiguration Config { get; set; }
        public Thread NetThread { get; set; }
        public MyNetWorker NetWorker { get; set; }


        public Chat()
        {
            InitializeComponent();

            tabControl.Hide();
            loginButton.Enabled = false;
            sendButtonPublic.Enabled = false;
            sendButtonPrivate.Enabled = false;
            listPrivateConversation.Hide();
            textTypingPrivate.Hide();
            backButtonPrivate.Hide();
            sendButtonPrivate.Hide();

            self = new User();

            IPs = NetworkConnect.Address.GetAllIPs();

            if (IPs.Count == 0)
                this.Close();
        }


        private void loginButton_Click(object sender, EventArgs e)
        {
            self.id = new Guid();
            self.username =  textUsername.Text;

            label1.Hide();
            textUsername.Hide();
            loginButton.Hide();
            tabControl.Show();

            // Opening own Inbox
            foreach (var ip in IPs)
            {
                Type contract = typeof(IPrivateChat);
                NetPeerTcpBinding binding = new NetPeerTcpBinding("BindingUnsecure");
                Uri address = new Uri("net.p2p://" + ip + ":" + basePort + "/inbox");

                selfHost = new ServiceHost(this);
                selfHost.AddServiceEndpoint(contract, binding, address);
                selfHost.Open();

                Config = new NetPeerConfiguration("Chat");
                Config.BroadcastAddress = NetworkConnect.Address.GetBroadcastAddress(ip, NetworkConnect.Address.GetSubnetMask(ip));
                Config.Port = basePort;
                Config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
                Config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
                Peer = new NetPeer(Config);
                Peer.Start();
                Peer.DiscoverLocalPeers(Config.Port);

                // Start routine
                NetWorker = new MyNetWorker(Peer, this);
                NetThread = new Thread(NetWorker.ProcessNet);
                NetThread.Start();

                basePort++;
            }
        }


        private void textUsername_TextChanged(object sender, EventArgs e)
        {
            if (textUsername.Text == "")
                loginButton.Enabled = false;
            else
                loginButton.Enabled = true;
        }


        public void callPeer(IPEndPoint endpoint)
        {
            foreach (var ip in IPs)
            {
                if (endpoint.Address.ToString() == ip.ToString())
                    return;
            }

            Type contract = typeof(IPrivateChat);
            NetPeerTcpBinding binding = new NetPeerTcpBinding("BindingUnsecure");
            Uri address = new Uri("net.p2p://" + endpoint.Address.ToString() + ":" + endpoint.Port + "/inbox");

            ServiceHost testHost = new ServiceHost(this);
            testHost.AddServiceEndpoint(contract, binding, address);

            ChannelFactory<IPrivateChat> testChannelFactory = new ChannelFactory<IPrivateChat>(testHost.Description.Endpoints[0]);
            IPrivateChat testChannel = testChannelFactory.CreateChannel();

            int IPIndex = NetworkConnect.Address.GetCorrectIPIndex(IPs, endpoint.Address);
            if (IPIndex >= 0)
            {
                self.IP = IPs[IPIndex];
                self.Port = basePort + IPIndex;
            }
            else
            {
                return;
            }

            testChannel.answerPeer(self);
            testChannelFactory.Close();
        }


        public void answerPeer(User user)
        {
            for (int i = 0; i < private_list.Count; ++i)
            {
                if (user.id == private_list[i].id)
                    return;
            }

            int index = private_list.Count;
            private_list.Add(user);
            

            // Estabilishing connection
            Type contract = typeof(IPrivateChat);
            NetPeerTcpBinding binding = new NetPeerTcpBinding("BindingUnsecure");
            Uri address = new Uri("net.p2p://" + user.IP + ":" + user.Port + "/inbox");

            // Creating Channel
            hosts.Add(new ServiceHost(this));
            hosts[index].AddServiceEndpoint(contract, binding, address);
            channelFactories.Add(new ChannelFactory<IPrivateChat>(hosts[index].Description.Endpoints[0]));
            channels.Add(channelFactories[index].CreateChannel());

            listOfConversations.Items.Add(user.username);
            conversations.Add(new List<string>());

            int IPIndex = NetworkConnect.Address.GetCorrectIPIndex(IPs, user.IP);
            if (IPIndex >= 0)
            {
                self.IP = IPs[IPIndex];
                self.Port = basePort + IPIndex;
            }
            else
            {
                return;
            }

            channels[index].answerPeer(self);
        }


        private void sendButtonPublic_Click(object sender, EventArgs e)
        {
            listChatroom.Items.Add("You: " + textTypingPublic.Text);

            for (int i = 0; i < channels.Count; ++i)
                channels[i].SendPublicMessage(self.id, textTypingPublic.Text);

            sendButtonPublic.Enabled = false;
            textTypingPublic.Clear();
        }


        private void textTypingPublic_TextChanged(object sender, EventArgs e)
        {
            if (textTypingPublic.Text != "")
                sendButtonPublic.Enabled = true;
            else
                sendButtonPublic.Enabled = false;
        }


        public void SendPublicMessage(Guid id, string message)
        {
            User user = new User();

            for (int i = 0; i < private_list.Count; ++i)
            {
                if (id == private_list[i].id)
                {
                    user = private_list[i];
                    break;
                }
            }

            if (user != null)
                listChatroom.Items.Add(user.username + ": " + message);
        }


        private void listOfConversations_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int selectedIndex = listOfConversations.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < private_list.Count)
            {
                selected_conversation = selectedIndex;
                listPrivateConversation.Items.Clear();
                for (int i = 0; i < conversations[selected_conversation].Count; ++i)
                    listPrivateConversation.Items.Add(conversations[selected_conversation][i]);

                listOfConversations.Hide();
                listPrivateConversation.Show();
                textTypingPrivate.Show();
                backButtonPrivate.Show();
                sendButtonPrivate.Show();
            }
        }


        private void sendButtonPrivate_Click(object sender, EventArgs e)
        {
            string new_message = "You: " + textTypingPrivate.Text;
            conversations[selected_conversation].Add(new_message);
            listPrivateConversation.Items.Add(new_message);

            channels[selected_conversation].SendPrivateMessage(self.id, textTypingPrivate.Text);

            sendButtonPrivate.Enabled = false;
            textTypingPrivate.Clear();
        }


        private void textTypingPrivate_TextChanged(object sender, EventArgs e)
        {
            if (textTypingPrivate.Text != "")
                sendButtonPrivate.Enabled = true;
            else
                sendButtonPrivate.Enabled = false;
        }


        public void SendPrivateMessage(Guid id, string message)
        {
            int index = -1;
            User user = new User();

            for (int i = 0; i < private_list.Count; ++i)
            {
                if (id == private_list[i].id)
                {
                    index = i;
                    user = private_list[i];
                    break;
                }
            }

            if (user != null && index != -1)
            {
                string new_message = user.username + ": " + message;
                conversations[index].Add(new_message);
                listPrivateConversation.Items.Add(new_message);
            }
        }

        private void backButtonPrivate_Click(object sender, EventArgs e)
        {
            selected_conversation = -1;

            listOfConversations.Show();
            listPrivateConversation.Hide();
            textTypingPrivate.Hide();
            backButtonPrivate.Hide();
            sendButtonPrivate.Hide();
        }


        private void Chat_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (NetThread != null)
                NetThread.Abort();
        }


        public class MyNetWorker
        {
            NetPeer peer = null;
            Chat netManager = null;

            public MyNetWorker(NetPeer inPeer, Chat inNetManager)
            {
                peer = inPeer;
                netManager = inNetManager;
            }

            public void ProcessNet()
            {
                while (true)
                {
                    Thread.Sleep(1);
                    if (peer == null)
                        continue;
                    NetIncomingMessage msg;
                    while ((msg = peer.ReadMessage()) != null)
                    {
                        switch (msg.MessageType)
                        {
                            case NetIncomingMessageType.DiscoveryRequest:
                                peer.SendDiscoveryResponse(null, msg.SenderEndPoint);
                                netManager.callPeer(msg.SenderEndPoint);
                                break;
                            case NetIncomingMessageType.DiscoveryResponse:
                                netManager.callPeer(msg.SenderEndPoint);
                                break;
                        }
                    }
                }
            }
        }
    }
}

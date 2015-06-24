using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        List<Message> chatroom = new List<Message>();
        List<List<string>> conversations = new List<List<string>>();

        List<Thread> NetThread = new List<Thread>();
        Thread updater;
        List<IPrivateChat> channels = new List<IPrivateChat>();

        public Chat()
        {
            InitializeComponent();

            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

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
            self.id = Guid.NewGuid();
            self.username =  textUsername.Text;

            label1.Hide();
            textUsername.Hide();
            loginButton.Hide();
            tabControl.Show();

            int port = basePort;

            // Opening own Inboxes
            foreach (var ip in IPs)
            {
                Type contract = typeof(IPrivateChat);
                NetPeerTcpBinding binding = new NetPeerTcpBinding("BindingUnsecure");
                Uri address = new Uri("net.p2p://" + ip + ":" + port + "/inbox");

                ServiceHost selfHost = new ServiceHost(this);
                selfHost.AddServiceEndpoint(contract, binding, address);

                bool tryAgain, failToConnect = false;
                int attempts = 0;
                do
                {
                    tryAgain = false;
                    try { selfHost.Open(); }
                    catch { tryAgain = true; attempts++; }

                    if (attempts > 100)
                    {
                        MessageBox.Show("One of your IPs (" + ip.ToString() + ") failed to estabilish a connection with other users.");
                        failToConnect = true;
                    }
                } while (tryAgain && attempts <= 100);

                if (failToConnect)
                    continue;

                NetPeerConfiguration Config = new NetPeerConfiguration("Chat");
                Config.BroadcastAddress = NetworkConnect.Address.GetBroadcastAddress(ip, NetworkConnect.Address.GetSubnetMask(ip));
                Config.Port = port;
                Config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
                Config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

                NetPeer Peer = new NetPeer(Config);
                Peer.Start();

                MyNetWorker NetWorker = new MyNetWorker(Peer, this);
                NetThread.Add(new Thread(NetWorker.ProcessNet));
                NetThread.Last().Start();

                Peer.DiscoverLocalPeers(Config.Port);
                
                port++;
            }

            updater = new Thread(CheckUsers);
            updater.Start();
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

            // Defining Connection
            Type contract = typeof(IPrivateChat);
            NetPeerTcpBinding binding = new NetPeerTcpBinding("BindingUnsecure");
            Uri address = new Uri("net.p2p://" + endpoint.Address.ToString() + ":" + endpoint.Port + "/inbox");

            // Creating Channel
            ServiceHost Host = new ServiceHost(this);
            Host.AddServiceEndpoint(contract, binding, address);
            ChannelFactory<IPrivateChat> channelFactory = new ChannelFactory<IPrivateChat>(Host.Description.Endpoints[0]);
            IPrivateChat testChannel = channelFactory.CreateChannel();

            // Completing User
            int IPIndex = NetworkConnect.Address.GetCorrectIPIndex(IPs, endpoint.Address);
            if (IPIndex >= 0)
            {
                self.IP = IPs[IPIndex];
                self.Port = basePort + IPIndex;
                self.lastStamp = DateTime.Now;
            }
            else
            {
                return;
            }

            bool tryAgain;
            int attempts = 0;
            do {
                tryAgain = false;
                try { testChannel.answerPeer(self); }
                catch { tryAgain = true; attempts++; }
            } while (tryAgain && attempts < 10);

            attempts = 0;
            do
            {
                tryAgain = false;
                try { channelFactory.Close(); }
                catch { tryAgain = true; attempts++; }
            } while (tryAgain && attempts < 10);
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

            // Defining Connection
            Type contract = typeof(IPrivateChat);
            NetPeerTcpBinding binding = new NetPeerTcpBinding("BindingUnsecure");
            Uri address = new Uri("net.p2p://" + user.IP + ":" + user.Port + "/inbox");

            // Creating Channel
            ServiceHost host = new ServiceHost(this);
            host.AddServiceEndpoint(contract, binding, address);
            ChannelFactory<IPrivateChat> channelFactory = new ChannelFactory<IPrivateChat>(host.Description.Endpoints[0]);
            channels.Add(channelFactory.CreateChannel());

            listOfConversations.Items.Add(user.username + " (online)");
            conversations.Add(new List<string>());

            // Completing User
            int IPIndex = NetworkConnect.Address.GetCorrectIPIndex(IPs, user.IP);
            if (IPIndex >= 0)
            {
                self.IP = IPs[IPIndex];
                self.Port = basePort + IPIndex;
                self.lastStamp = DateTime.Now;
            }
            else
            {
                return;
            }

            bool tryAgain;
            int attempts = 0;
            do
            {
                tryAgain = false;
                try { channels[index].answerPeer(self); }
                catch { tryAgain = true; attempts++; }
            } while (tryAgain && attempts < 10);
        }


        public void UpdateSelf(Guid id, DateTime lastUpdate)
        {
            foreach (var user in private_list)
            {
                if (user.id == id)
                {
                    user.online = true;
                    user.lastStamp = lastUpdate;
                }
            }
        }


        private void sendButtonPublic_Click(object sender, EventArgs e)
        {
            Message msg = new Message(Guid.NewGuid(), self.id, textTypingPublic.Text);

            chatroom.Add(msg);
            listChatroom.Items.Add("You: " + textTypingPublic.Text);

            for (int i = 0; i < channels.Count; ++i)
            {
                try 
                {
                    channels[i].SendPublicMessage(msg);
                }
                catch
                {
                    string warning = "Message could not be sent to user " + private_list[i].username + " (" + private_list[i].id + ").";
                    listChatroom.Items.Add(warning);
                    chatroom.Add(new Message(Guid.NewGuid(), self.id, warning));
                }
            }

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


        public void SendPublicMessage(Message message)
        {
            foreach (var previousMessage in chatroom)
            {
                if (previousMessage.id == message.id)
                    return;
            }

            User user = new User();

            for (int i = 0; i < private_list.Count; ++i)
            {
                if (message.userId == private_list[i].id)
                {
                    user = private_list[i];
                    break;
                }
            }

            if (user != null)
                listChatroom.Items.Add(user.username + ": " + message.text);

            chatroom.Add(message);
        }


        private void listChatroom_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int selectedIndex = listChatroom.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < chatroom.Count)
            {
                Guid selectedUser = chatroom[selectedIndex].userId;

                int index = -1;
                for (int i = 0; i < private_list.Count; ++i)
                {
                    if (selectedUser == private_list[i].id)
                        index = i;
                }

                if (!private_list[index].online)
                    return;

                selected_conversation = index;
                tabControl.SelectedIndex = 1;
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


        private void listOfConversations_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int selectedIndex = listOfConversations.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < private_list.Count)
            {
                if (!private_list[selectedIndex].online)
                    return;

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


        public void updateContact(Guid Id, bool status)
        {
            for (int i = 0; i < private_list.Count; ++i)
            {
                if (Id == private_list[i].id)
                {
                    if (status == true)
                    {
                        listOfConversations.Items[i] = private_list[i].username + " (online)";
                        if (selected_conversation == i)
                            textTypingPrivate.Enabled = true;
                    }
                    else
                    {
                        listOfConversations.Items[i] = private_list[i].username + " (offline)";
                        if (selected_conversation == i)
                        {
                            textTypingPrivate.Clear();
                            textTypingPrivate.Enabled = false;
                        }
                    }
                    return;
                }
            }
        }


        private void sendButtonPrivate_Click(object sender, EventArgs e)
        {
            string new_message = "You: " + textTypingPrivate.Text;

            try
            {
                channels[selected_conversation].SendPrivateMessage(new Message(Guid.NewGuid(), self.id, textTypingPrivate.Text));
                conversations[selected_conversation].Add(new_message);
                listPrivateConversation.Items.Add(new_message);
            }
            catch
            {
                string warning = "Message could not be sent to the user.";
                conversations[selected_conversation].Add(warning);
                listPrivateConversation.Items.Add(warning);
            }

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


        public void SendPrivateMessage(Message message)
        {
            int index = -1;
            User user = new User();

            for (int i = 0; i < private_list.Count; ++i)
            {
                if (message.userId == private_list[i].id)
                {
                    index = i;
                    user = private_list[i];
                    break;
                }
            }

            if (user != null && index != -1)
            {
                string new_message = user.username + ": " + message.text;
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
            foreach (var thread in NetThread)
                thread.Abort();
            if (updater != null)
                updater.Abort();
        }


        public void CheckUsers()
        {
            while (true)
            {
                Thread.Sleep(1000);
                for (int i = 0; i < private_list.Count; ++i)
                {
                    try
                    {
                        channels[i].UpdateSelf(self.id, DateTime.Now);
                    }
                    finally
                    {
                        User user = private_list[i];

                        if ((DateTime.Now - user.lastStamp).TotalSeconds > 5 && user.online == true)
                        {
                            user.online = false;
                            updateContact(user.id, user.online);
                        }
                        else if ((DateTime.Now - user.lastStamp).TotalSeconds < 5)
                        {
                            user.online = true;
                            updateContact(user.id, user.online);
                        }
                    }
                }
            }
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
                    Thread.Sleep(10);
                    if (peer == null)
                        continue;
                    NetIncomingMessage msg;
                    while ((msg = peer.ReadMessage()) != null)
                    {
                        if (msg.MessageType == NetIncomingMessageType.DiscoveryRequest)
                            netManager.callPeer(msg.SenderEndPoint);
                    }
                }
            }
        }
    }
}

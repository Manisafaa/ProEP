using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using ClientForm.ChatServer;
using NetworkConnect;

namespace ClientForm
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public partial class Form1 : Form, IChatCallback, IPrivateChat
    {
        // Connection to Proxy Server
        ChatClient proxy;

        // The information on the own user.
        User self;

        // The index of the currently selected private conversation, or -1, if there is none selected.
        int selected_conversation = -1;

        // The list of users available for a private conversation.
        List<User> private_list = new List<User>();

        // All the private conversations history.
        List<List<string>> conversations = new List<List<string>>();

        // Channels for peer-to-peer communication.
        List<IPrivateChat> channels = new List<IPrivateChat>();

        // IDs of the user that sent each public message.
        List<Guid> conversationUserIds = new List<Guid>();
        
        public Form1()
        {
            InstanceContext context = new InstanceContext(this);
            proxy = new ChatClient(context);

            InitializeComponent();

            // Initializing graphical interface
            tabControl1.Hide();
            loginButton.Enabled = false;
            sendButtonChat.Enabled = false;
            sendButtonPrivate.Enabled = false;
            listPrivateConversation.Hide();
            textTypingPrivate.Hide();
            backButtonPrivate.Hide();
            sendButtonPrivate.Hide();
        }


        // INTERFACE: IChatCallback

        public void BroadcastMessage(User user, string message)
        {
            // Called by the server to send to the user the message another user sent.

            listChatroom.Items.Add(user.username + ": " + message);
            conversationUserIds.Add(user.id);
        }

        public void DeliverMessage(User user, string message)
        {
            // Called by the server to deliver a private message another user sent.

            int index = -1;

            for (int i = 0; i < private_list.Count; ++i)
            {
                if (private_list[i].id == user.id)
                    index = i;
            }

            if (index == -1)
            {
                index = private_list.Count;
                private_list.Add(user);
                conversations.Add(new List<string>());
                listOfConversations.Items.Add(private_list[index].username);
            }

            string new_message = user.username + ": " + message;
            conversations[index].Add(new_message);

            if (selected_conversation == index)
                listPrivateConversation.Items.Add(new_message);
        }

        public bool OpenHost(User user)
        {
            // Called by the service to try to initialize a peer-to-peer connection to another user's inbox.
 
            for (int i = 0; i < private_list.Count; ++i)
            {
                if (user.id == private_list[i].id)
                    return false;
            }

            Type contract = typeof(IPrivateChat);
            NetPeerTcpBinding binding = new NetPeerTcpBinding("BindingUnsecure");

            IPAddress ip = Address.GetSameSubnetIP(self.IPs, self.SMs, user.IPs, user.SMs);

            Uri address;
            if (ip != null)
                address = new Uri("net.p2p://" + ip + "/inbox");
            else
                return false;

            ServiceHost host = new ServiceHost(this);
            host.AddServiceEndpoint(contract, binding, address);
            ChannelFactory<IPrivateChat> channelFactory = new ChannelFactory<IPrivateChat>(host.Description.Endpoints[0]);
            channels.Add(channelFactory.CreateChannel());
            host.Close();
            channelFactory.Close();

            listOfConversations.Items.Add(user.username);
            private_list.Add(user);
            conversations.Add(new List<string>());
            
            return true;
        }


        // INTERFACE: IPrivateChat
        public void SendMessage(Guid id, string message)
        {
            // Called by another user to send a private message via a peer-to-peer connection.

            int index = -1;

            for (int i = 0; i < private_list.Count; ++i)
            {
                if (id == private_list[i].id)
                    index = i;
            }

            if (index == -1)
                return;

            string new_message = private_list[index].username + ": " + message;
            conversations[index].Add(new_message);

            if (selected_conversation == index)
                listPrivateConversation.Items.Add(new_message);
        }


        // CLASS: Form1
        private void textLogin_TextChanged(object sender, EventArgs e)
        {
            if (textLogin.Text == "")
                loginButton.Enabled = false;
            else
                loginButton.Enabled = true;
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            // Method that subscribes the user on the server.

            Guid id = Guid.NewGuid();
            self = proxy.Subscribe(id, textLogin.Text, Address.GetAllIPs(), Address.GetAllSubnetMasks(Address.GetAllIPs()));

            // User opens own inbox to other users
            foreach (var ip in self.IPs)
            {
                Type contract = typeof(IPrivateChat);
                NetPeerTcpBinding binding = new NetPeerTcpBinding("BindingUnsecure");
                Uri address = new Uri("net.p2p://" + ip + "/inbox");

                ServiceHost host = new ServiceHost(this);
                host.AddServiceEndpoint(contract, binding, address);
                host.Open();
            }

            loginLabel.Hide();
            textLogin.Hide();
            loginButton.Hide();
            tabControl1.Show();
        }

        private void textTypingChat_TextChanged(object sender, EventArgs e)
        {
            if (textTypingChat.Text != "")
                sendButtonChat.Enabled = true;
            else
                sendButtonChat.Enabled = false;
        }

        private void sendButtonChat_Click(object sender, EventArgs e)
        {
            // Method that sends a public message to the server for it to broadcast it.

            listChatroom.Items.Add("You: " + textTypingPrivate.Text);

            proxy.SendPublicMessage(self.id, textTypingChat.Text);
            sendButtonChat.Enabled = false;
            textTypingChat.Clear();
        }

        private void listChatroom_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Method that selects a message on the chatroom to start a private conversation with the sender.

            if (listChatroom.SelectedIndex < 0 || listChatroom.SelectedIndex >= listChatroom.Items.Count)
                return;

            Guid selectedUserId = conversationUserIds.ElementAt(listChatroom.SelectedIndex);

            if (selectedUserId == self.id || selectedUserId == null)
                return;

            int index = -1;
            for (int i = 0; i < private_list.Count; ++i)
            {
                if (private_list[i].id == selectedUserId)
                    index = i;
            }

            if (index == -1)
            {
                User new_user = proxy.ConnectWithUser(self.id, selectedUserId);

                if (new_user == null)
                    return;

                IPAddress ip = Address.GetSameSubnetIP(self.IPs, self.SMs, new_user.IPs, new_user.SMs);

                if (ip != null)
                {
                    new_user.isInSameSubnet = true;

                    Type contract = typeof(IPrivateChat);
                    NetPeerTcpBinding binding = new NetPeerTcpBinding("BindingUnsecure");
                    Uri address = new Uri("net.p2p://" + ip + "/inbox");

                    ServiceHost host = new ServiceHost(this);
                    host.AddServiceEndpoint(contract, binding, address);
                    ChannelFactory<IPrivateChat> channelFactory = new ChannelFactory<IPrivateChat>(host.Description.Endpoints[0]);
                    channels.Add(channelFactory.CreateChannel());
                    host.Close();
                    channelFactory.Close();
                }
                else
                {
                    new_user.isInSameSubnet = false;
                    channels.Add(null);
                }

                index = private_list.Count;
                private_list.Add(new_user);
                conversations.Add(new List<string>());
                listOfConversations.Items.Add(new_user.username);
            }

            selected_conversation = index;

            listPrivateConversation.Items.Clear();
            for (int i = 0; i < conversations[index].Count; ++i)
                listPrivateConversation.Items.Add(conversations[index][i]);

            listOfConversations.Hide();
            listPrivateConversation.Show();
            textTypingPrivate.Show();
            backButtonPrivate.Show();
            sendButtonPrivate.Show();
            tabControl1.SelectedIndex = 1;
        }

        private void listOfConversations_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Method that selects a private ongoing conversation.

            int selectedIndex = listOfConversations.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < listOfConversations.Items.Count)
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

        private void textTypingPrivate_TextChanged(object sender, EventArgs e)
        {
            if (textTypingChat.Text != "")
                sendButtonChat.Enabled = true;
            else
                sendButtonChat.Enabled = false;
        }

        private void sendButtonPrivate_Click(object sender, EventArgs e)
        {
            // Method that sends a message to another user in a private conversation.

            string new_message = "You: " + textTypingPrivate.Text.ToString();
            conversations[selected_conversation].Add(new_message);
            listPrivateConversation.Items.Add(new_message);

            if (private_list[selected_conversation].isInSameSubnet)
                channels[selected_conversation].SendMessage(self.id, textTypingPrivate.Text.ToString());
            else
                proxy.SendPrivateMessage(self.id, textTypingPrivate.Text, private_list[selected_conversation].id);

            sendButtonPrivate.Enabled = false;
            textTypingPrivate.Clear();
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Method that unsubscribes the user from the server.

            proxy.Unsubscribe();
        }
    }
}

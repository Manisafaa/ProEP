﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using ClientForm.ChatServer;

namespace ClientForm
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public partial class Form1 : Form, IChatCallback, IPrivateChat
    {
        public ChatClient proxy;
        User self;
        int selected_conversation = -1;
        List<int> order_of_contacts = new List<int>();
        List<User> private_list = new List<User>();
        List<List<string>> conversations = new List<List<string>>();

        // P2P
        List<IPrivateChat> channels = new List<IPrivateChat>();
        List<ServiceHost> hosts = new List<ServiceHost>();
        List<ChannelFactory<IPrivateChat>> channelFactories = new List<ChannelFactory<IPrivateChat>>();

        public Form1()
        {
            // C/S
            InstanceContext context = new InstanceContext(this);
            proxy = new ChatClient(context);

            // P2P
            //host = new ServiceHost(this);

            InitializeComponent();

            tabControl1.Hide();
            button1.Enabled = false;
            sendButtonChat.Enabled = false;
            sendButtonPrivate.Enabled = false;
            listPrivateConversation.Hide();
            textTypingPrivate.Hide();
            backButtonPrivate.Hide();
            sendButtonPrivate.Hide();
        }

        public void BroadcastMessage(ChatMessage message)
        {
            //Kyrill: int id replaced by Guid id
            listChatroom.Items.Add(message.user.username + " (#" + message.user.id.ToString() + "): " + message.text);
        }

        public void DeliverMessage(ChatMessage message)
        {
            int index = -1;

            for (int i = 0; i < private_list.Count; ++i)
            {
                if (private_list[i].id == message.user.id)
                    index = i;
            }

            if (index == -1)
            {
                index = private_list.Count;
                order_of_contacts.Insert(0, index);
                private_list.Add(message.user);
                conversations.Add(new List<string>());
            }
            else
            {
                order_of_contacts.Remove(index);
                order_of_contacts.Insert(0, index);
                listOfConversations.Items.Remove(private_list[index].username);
            }

            listOfConversations.Items.Insert(0, private_list[index].username);

            //Kyrill: int id replaced by Guid id
            string new_message = message.user.username + " (#" + message.user.id.ToString() + "): " + message.text;
            conversations[index].Add(new_message);

            if (selected_conversation == index)
            {
                listPrivateConversation.Items.Add(new_message);
            }
        }

        public bool OpenHost(User user)
        {
            for (int i = 0; i < private_list.Count; ++i)
            {
                if (user.id == private_list[i].id)
                    return false;
            }
            
            int index = private_list.Count;
            order_of_contacts.Insert(0, index);
            private_list.Add(user);

            // Opening Service
            Type contract = typeof(IPrivateChat);
            NetPeerTcpBinding binding = new NetPeerTcpBinding("BindingUnsecure");
            Uri address;
            //Kyrill: int id replaced by Guid id, so self.id < user.id is not possible anymore
            //if (self.id < user.id)
                address = new Uri("net.p2p://PrivateChat/" + self.id + "x" + user.id);
            //else
            //    address = new Uri("net.p2p://PrivateChat/" + user.id + "x" + self.id);

            hosts.Add(new ServiceHost(this));
            hosts[index].AddServiceEndpoint(contract, binding, address);
            hosts[index].Open();

            // Creating Channel
            channelFactories.Add(new ChannelFactory<IPrivateChat>(hosts[index].Description.Endpoints[0]));
            channels.Add(channelFactories[index].CreateChannel());

            conversations.Add(new List<string>());
            
            return true;
        }

        //Kyrill: int id replaced by Guid id
        public void SendMessage(Guid id, string message)
        {
            int index = -1;

            for (int i = 0; i < private_list.Count; ++i)
            {
                if (id == private_list[i].id)
                    index = i;
            }

            if (index == -1)
            {
                return;
            }

            order_of_contacts.Remove(index);
            order_of_contacts.Insert(0, index);
            listOfConversations.Items.Remove(private_list[index].username);
            listOfConversations.Items.Insert(0, private_list[index].username);

            //Kyrill: int id replaced by Guid id
            string new_message = private_list[index].username +
                                 " (#" + private_list[index].id.ToString() + "): " + message;
            conversations[index].Add(new_message);

            if (selected_conversation == index)
            {
                listPrivateConversation.Items.Add(new_message);
            }
        }

        private void listChatroom_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            String selected_message = listChatroom.SelectedItem.ToString();

            bool start_counting = false;
            //Kyrill: int id replaced by Guid id
            Guid id = Guid.NewGuid();
            for (int i = 0; i < selected_message.Count(); ++i) {
                if (selected_message[i] == ')')
                    break;

                //Kyrill: what is this for ?
                //if (start_counting == true)
                  //  id = 10*id + Convert.ToInt16(selected_message[i].ToString());
                if (selected_message[i] == '#')
                    start_counting = true;
            }

            if (id == self.id)
            {
                return;
            }

            int index = -1;
            for (int i = 0; i < private_list.Count; ++i)
            {
                if (private_list[i].id == id)
                    index = i;
            }

            if (index == -1)
            {
                User new_user = proxy.ConnectWithUser(self.id, id);

                if (new_user == null)
                    return;

                index = private_list.Count;
                order_of_contacts.Insert(0, index);
                private_list.Add(new_user);

                // Opening Service
                Type contract = typeof(IPrivateChat);
                NetPeerTcpBinding binding = new NetPeerTcpBinding("BindingUnsecure");
                // Should configure the same as BindingUnsecure
                Uri address;
                //Kyrill: int id replaced by Guid id, so self.id < user.id is not possible anymore
                //if (self.id < id)
                    address = new Uri("net.p2p://PrivateChat/" + self.id + "x" + id);
                //else
                //    address = new Uri("net.p2p://PrivateChat/" + id + "x" + self.id);

                hosts.Add(new ServiceHost(this));
                hosts[index].AddServiceEndpoint(contract, binding, address);
                hosts[index].Open();

                // Creating Channel
                channelFactories.Add(new ChannelFactory<IPrivateChat>(hosts[index].Description.Endpoints[0]));
                channels.Add(channelFactories[index].CreateChannel());

                conversations.Add(new List<string>());
            }
            else
            {
                order_of_contacts.Remove(index);
                order_of_contacts.Insert(0, index);
                listOfConversations.Items.Remove(private_list[index].username);
            }

            listOfConversations.Items.Insert(0, private_list[index].username);

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

        private void button1_Click(object sender, EventArgs e)
        {
            bool check = true;

            for (int i = 0; i < textBox1.Text.Count(); ++i) {
                if (textBox1.Text[i] == '#' || textBox1.Text[i] == '(' || textBox1.Text[i] == ')')
                    check = false;
            }

            if (check)
            {
                self = proxy.Subscribe(Guid.NewGuid(), textBox1.Text);
                ChatMessage[] chat = proxy.GetLastMessages();
                for (int i = 0; i < chat.Count(); ++i)
                    listChatroom.Items.Add(chat[i].user.username + " (#" + chat[i].user.id.ToString() + "): " + chat[i].text);

                label1.Hide();
                textBox1.Hide();
                button1.Hide();
                tabControl1.Show();
            }
            else
            {
                MessageBox.Show("Symbol '#', '(' and ')' are illegal in usernames.");
                textBox1.Clear();
            }
        }

        private void sendButtonPrivate_Click(object sender, EventArgs e)
        {
            order_of_contacts.Remove(selected_conversation);
            order_of_contacts.Insert(0, selected_conversation);
            listOfConversations.Items.Remove(private_list[selected_conversation].username);
            listOfConversations.Items.Insert(0, private_list[selected_conversation].username);

            string new_message = "You: " + textTypingPrivate.Text.ToString();
            conversations[selected_conversation].Add(new_message);
            listPrivateConversation.Items.Add(new_message);

            channels[selected_conversation].SendMessage(self.id, textTypingPrivate.Text.ToString());
            //proxy.SendPrivateMessage(self.id, textTypingPrivate.Text.ToString(), private_list[selected_conversation].id);
            sendButtonPrivate.Enabled = false;
            textTypingPrivate.Clear();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            selected_conversation = -1;

            listOfConversations.Show();
            listPrivateConversation.Hide();
            textTypingPrivate.Hide();
            backButtonPrivate.Hide();
            sendButtonPrivate.Hide();
        }

        private void tabPrivate_MouseDoubleClick(object sender, MouseEventArgs e)
        {
        }

        private void sendButtonChat_Click(object sender, EventArgs e)
        {
            proxy.SendPublicMessage(self.id, textTypingChat.Text.ToString());
            sendButtonChat.Enabled = false;
            textTypingChat.Clear();
        }

        private void textTypingChat_TextChanged(object sender, EventArgs e)
        {
            if (textTypingChat.Text != "")
                sendButtonChat.Enabled = true;
            else
                sendButtonChat.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (self != null)
                proxy.Unsubscribe(self.id);
        }

        private void listOfConversations_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int selectedIndex = listOfConversations.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < order_of_contacts.Count)
            {
                selected_conversation = order_of_contacts[selectedIndex];
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
            if (textTypingPrivate.Text != "")
                sendButtonPrivate.Enabled = true;
            else
                sendButtonPrivate.Enabled = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                button1.Enabled = false;
            else
                button1.Enabled = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Chat : IChat
    {
        List<ChatMessage> lastMessages = new List<ChatMessage>();
        List<User> users = new List<User>();

        public User Subscribe(string username)
        {
            User new_user = new User();

            new_user.id = users.Count;
            new_user.username = username;
            new_user.callback = OperationContext.Current.GetCallbackChannel<IChatCallback>();

            users.Add(new_user);

            return new_user;
        }

        public List<ChatMessage> GetLastMessages()
        {
            return lastMessages;
        }

        public void SendPublicMessage(int from, string message)
        {
            ChatMessage new_message = new ChatMessage();
            new_message.user = users[from];
            new_message.text = message;

            for (int i = 0; i < users.Count; ++i)
            {
                if (users[i].callback != null)
                    users[i].callback.BroadcastMessage(new_message);
            }

            if (lastMessages.Count > 5)
                lastMessages.RemoveAt(0);
            lastMessages.Add(new_message);
        }

        public User ConnectWithUser(int from, int to)
        {
            User from_user = new User();

            for (int i = 0; i < users.Count; ++i) {
                if (from == users[i].id)
                    from_user = users[i];
            }

            if (users[to].callback != null)
            {
                if (users[to].callback.OpenHost(from_user))
                    return users[to];
            }
            
            return null;
        }

        public void SendPrivateMessage(int from, string message, int to)
        {
            ChatMessage new_message = new ChatMessage();
            new_message.user = users[from];
            new_message.text = message;

            if (users[to].callback != null) {
                users[to].callback.DeliverMessage(new_message);
            }
            /*else
            {
                ChatMessage error_message = new ChatMessage();
                error_message.user = users[to];
                error_message.text = "The user logged out";

                users[from].callback.DeliverMessage(error_message);
            }*/

        }

        public void Unsubscribe(int id)
        {
            if (users[id].callback == OperationContext.Current.GetCallbackChannel<IChatCallback>())
                users[id].callback = null;
        }
    }
}

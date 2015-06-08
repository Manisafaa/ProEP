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


        public User Subscribe(Guid id, string username)
        {
            User new_user = new User();

            new_user.id = id;
            new_user.username = username;
            new_user.callback = OperationContext.Current.GetCallbackChannel<IChatCallback>();

            users.Add(new_user);

            return new_user;
        }


        public List<ChatMessage> GetLastMessages()
        {
            return lastMessages;
        }


        public void SendPublicMessage(Guid from, string message)
        {
            int index = -1;

            for (int i = 0; i < users.Count; ++i)
            {
                if (users[i].id == from)
                    index = i;
            }

            if (index == -1)
                return;

            ChatMessage new_message = new ChatMessage();
            new_message.user = users[index];
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
        

        public User ConnectWithUser(Guid from, Guid to)
        {
            User from_user = new User(), to_user = new User();

            for (int i = 0; i < users.Count; ++i) {
                if (from == users[i].id)
                    from_user = users[i];
                if (to == users[i].id)
                    to_user = users[i];
            }


            if (to_user.callback != null)
            {
                if (to_user.callback.OpenHost(from_user))
                    return to_user;
            }
            
            return null;
        }


        public void Unsubscribe()
        {
            for (int i = 0; i < users.Count; ++i)
            {
                if (users[i].callback == OperationContext.Current.GetCallbackChannel<IChatCallback>())
                    users[i].callback = null;
            }
        }
    }
}

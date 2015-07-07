using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ChatService;

namespace ChatService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple), CallbackBehavior(UseSynchronizationContext = false)]
    public class Service1 : IChat
    {
        List<string> lastMessages = new List<string>();
        List<User> lastSenders = new List<User>();
        Dictionary<Guid, User> users = new Dictionary<Guid, User>();

        public User Subscribe(Guid id, string username, List<IPAddress> ips, List<IPAddress> sms)
        {
            User new_user = new User();
            new_user.id = id;
            new_user.username = username;
            new_user.isInSameSubnet = false;
            new_user.IPs = ips;
            new_user.SMs = sms;
            new_user.callback = OperationContext.Current.GetCallbackChannel<IChatCallback>();

            //for (int i = 0; i < lastMessages.Count; ++i)
            //    new_user.callback.BroadcastMessage(lastSenders[i], lastMessages[i]);

            users.Add(new_user.id, new_user);
            return new_user;
        }

        public void SendPublicMessage(Guid from, string message)
        {
            foreach (var dictValue in users)
            {
                User user = dictValue.Value;
                if (user.callback != null && user.callback != OperationContext.Current.GetCallbackChannel<IChatCallback>())
                    user.callback.BroadcastMessage(users[from], message);
            }

            if (lastMessages.Count > 5)
            {
                lastMessages.RemoveAt(0);
                lastSenders.RemoveAt(0);
            }

            lastMessages.Add(message);
            lastSenders.Add(users[from]);
        }

        public User ConnectWithUser(Guid from, Guid to)
        {
            User from_user = new User();

            if (users[from] != null)
                from_user = users[from];

            if (users[to].callback != null)
            {
                users[to].callback.OpenHost(from_user);
                return users[to];
            }

            return null;
        }

        public void SendPrivateMessage(Guid from, string message, Guid to)
        {
            if (users[to].callback != null)
            {
                users[to].callback.DeliverMessage(users[from], message);
            }
        }

        public void Unsubscribe()
        {
            foreach (var dictValue in users)
            {
                if (dictValue.Value.callback == OperationContext.Current.GetCallbackChannel<IChatCallback>())
                    dictValue.Value.callback = null;
            }
        }
    }
}

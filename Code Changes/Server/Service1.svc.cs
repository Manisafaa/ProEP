using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ChatService;

namespace ChatService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Service1 : IChat
    {
        List<ChatMessage> lastMessages = new List<ChatMessage>();

        //Kyrill: List replaced by dictionary to use Guid
        //List<User> users = new List<User>();
        Dictionary<Guid, User> users = new Dictionary<Guid, User>();
        public User Subscribe(Guid id, string username)
        {
            User new_user = new User();

            //Kyrill: List replaced by dictionary to use Guid
            new_user.id = id;//users.Count;
            new_user.username = username;
            new_user.callback = OperationContext.Current.GetCallbackChannel<IChatCallback>();

            //Kyrill: List replaced by dictionary to use Guid
            users.Add(new_user.id, new_user);

            return new_user;
        }

        public List<ChatMessage> GetLastMessages()
        {
            return lastMessages;
        }

        public void SendPublicMessage(Guid from, string message)
        {
            ChatMessage new_message = new ChatMessage();
            new_message.user = users[from];
            new_message.text = message;

            //Kyrill: List replaced by dictionary to use Guid
            /*
            for (int i = 0; i < users.Count; ++i)
            {
                if (users[i].callback != null)
                    users[i].callback.BroadcastMessage(new_message);
            }*/


            foreach (var dictValue in users)
            {
                User user = dictValue.Value;
                if (user.callback != null)
                    user.callback.BroadcastMessage(new_message);
            }

            if (lastMessages.Count > 5)
                lastMessages.RemoveAt(0);
            lastMessages.Add(new_message);
        }

        public User ConnectWithUser(Guid from, Guid to)
        {
            User from_user = new User();
            //Kyrill: List replaced by dictionary to use Guid
            /*
                        for (int i = 0; i < users.Count; ++i) {
                            if (from == users[i].id)
                                from_user = users[i];
                        }
                        */
            if (users[from] != null)
                from_user = users[from];

            if (users[to].callback != null)
            {
                if (users[to].callback.OpenHost(from_user))
                    return users[to];
            }

            return null;
        }

        public void SendPrivateMessage(Guid from, string message, Guid to)
        {
            ChatMessage new_message = new ChatMessage();
            new_message.user = users[from];
            new_message.text = message;

            if (users[to].callback != null)
            {
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

        public void Unsubscribe(Guid id)
        {
            if (users[id].callback == OperationContext.Current.GetCallbackChannel<IChatCallback>())
                users[id].callback = null;
        }
    }
}

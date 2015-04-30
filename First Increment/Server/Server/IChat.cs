using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Server
{
    [ServiceContract(Namespace = "Server", CallbackContract = typeof(IChatCallback))]
    public interface IChat
    {
        [OperationContract]
        User Subscribe(string username);

        [OperationContract]
        List<ChatMessage> GetLastMessages();

        [OperationContract(IsOneWay = true)]
        void SendPublicMessage(int from, string message);

        [OperationContract]
        User ConnectWithUser(int from, int to);

        [OperationContract(IsOneWay = true)]
        void SendPrivateMessage(int from, string message, int to);

        [OperationContract(IsOneWay = true)]
        void Unsubscribe(int id);
    }

    [ServiceContract(Namespace = "Server", CallbackContract = typeof(IChatCallback))]
    public interface IChatCallback
    {
        [OperationContract(IsOneWay = true)]
        void BroadcastMessage(ChatMessage message);

        [OperationContract(IsOneWay = true)]
        void DeliverMessage(ChatMessage message);

        [OperationContract]
        bool OpenHost(User from);
    }


    [DataContract]
    public class ChatMessage
    {
        [DataMember]
        public User user { get; set; }

        [DataMember]
        public string text { get; set; }
    }


    [DataContract]
    public class User
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string username { get; set; }

        [DataMember]
        public IChatCallback callback { get; set; }
    }
}
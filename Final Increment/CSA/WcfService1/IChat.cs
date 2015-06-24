using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ChatService
{
    [ServiceContract(Namespace = "Server", CallbackContract = typeof(IChatCallback))]
    public interface IChat
    {
        //Kyrill: added Guid id to Subscribe so that the client can create its own id

        
        [OperationContract]
        User Subscribe(Guid id, string username);

        [OperationContract]
        List<ChatMessage> GetLastMessages();

        [OperationContract(IsOneWay = true)]
        void SendPublicMessage(Guid from, string message);

        [OperationContract]
        User ConnectWithUser(Guid from, Guid to);

        [OperationContract(IsOneWay = true)]
        void SendPrivateMessage(Guid from, string message, Guid to);

        [OperationContract(IsOneWay = true)]
        void Unsubscribe();
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
        //Kyrill: int id replaced by Guid id
        public Guid id { get; set; }

        [DataMember]
        public string username { get; set; }

        [DataMember]
        public IChatCallback callback { get; set; }
    }
}
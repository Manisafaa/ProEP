using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Net;

namespace ClientForm
{
    [ServiceContract]
    public interface IPrivateChat
    {
        [OperationContract(IsOneWay = true)]
        void answerPeer(User user);

        [OperationContract(IsOneWay=true)]
        void SendPrivateMessage(Message message);

        [OperationContract(IsOneWay = true)]
        void SendPublicMessage(Message message);

        [OperationContract(IsOneWay = true)]
        void UpdateSelf(Guid id, DateTime lastUpdate);
    }


    [DataContract]
    public class User
    {
        public User() {}

        public User(Guid theId, string theUsername, IPAddress theIP, int thePort)
        {
            id = theId;
            username = theUsername;
            IP = theIP;
            Port = thePort;
        }

        [DataMember]
        public Guid id { get; set; }

        [DataMember]
        public string username { get; set; }

        [DataMember]
        public IPAddress IP { get; set; }

        [DataMember]
        public IPAddress subnetMask { get; set; }

        [DataMember]
        public int Port { get; set; }

        [DataMember]
        public DateTime lastStamp { get; set; }

        [DataMember]
        public bool online { get; set; }
    }


    [DataContract]
    public class Message
    {
        public Message() {}

        public Message(Guid theId, Guid theUserId, string theText)
        {
            id = theId;
            userId = theUserId;
            text = theText;
        }

        [DataMember]
        public Guid id { get; set; }

        [DataMember]
        public Guid userId { get; set; }

        [DataMember]
        public string text { get; set; }
    }
}

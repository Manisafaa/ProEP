using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ChatService
{
    [ServiceContract(Namespace = "Server", CallbackContract = typeof(IChatCallback))]
    public interface IChat
    {
        [OperationContract]
        User Subscribe(Guid id, string username, List<IPAddress> ips, List<IPAddress> sms);

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
        void BroadcastMessage(User sender, string message);

        [OperationContract(IsOneWay = true)]
        void DeliverMessage(User sender, string message);

        [OperationContract(IsOneWay = true)]
        void OpenHost(User from);
    }


    [DataContract]
    public class User
    {
        [DataMember]
        public Guid id { get; set; }

        [DataMember]
        public string username { get; set; }

        [DataMember]
        public bool isInSameSubnet { get; set; }

        [DataMember]
        public List<IPAddress> IPs { get; set; }

        [DataMember]
        public List<IPAddress> SMs { get; set; }

        [DataMember]
        public IChatCallback callback { get; set; }
    }
}
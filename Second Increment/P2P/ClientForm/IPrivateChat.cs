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
        void SendPrivateMessage(Guid id, string message);

        [OperationContract(IsOneWay = true)]
        void SendPublicMessage(Guid from, string message);
    }


    [DataContract]
    public class User
    {
        [DataMember]
        public Guid id { get; set; }

        [DataMember]
        public string username { get; set; }

        [DataMember]
        public IPAddress IP { get; set; }

        [DataMember]
        public int Port { get; set; }
    }
}

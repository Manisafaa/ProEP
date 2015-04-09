using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Cerver
{
    [ServiceContract(Namespace = "Cerver", CallbackContract = typeof(IShatCallback))]
    public interface IShat
    {
        [OperationContract]
        List<ShatMessage> GetLastMessages();

        [OperationContract(IsOneWay = true)]
        void SendMessage(ShatMessage message);

        [OperationContract(IsOneWay = true)]
        void Unsubscribe();
    }


    public interface IShatCallback
    {
        [OperationContract(IsOneWay=true)]
        void BroadcastMessage(ShatMessage message);
    }


    [DataContract]
    public class ShatMessage
    {
        [DataMember]
        string username { get; set; }

        [DataMember]
        string text { get; set; }
    }
}

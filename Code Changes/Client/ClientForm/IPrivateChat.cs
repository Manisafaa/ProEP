using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ClientForm
{
    [ServiceContract]
    public interface IPrivateChat
    {
        [OperationContract(IsOneWay=true)]
        void SendMessage(Guid id, string message);
    }
}

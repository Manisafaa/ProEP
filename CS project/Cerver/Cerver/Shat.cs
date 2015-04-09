using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Cerver
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Shat : IShat
    {
        List<ShatMessage> lastMessages = new List<ShatMessage>();
        List<IShatCallback> callbacks = new List<IShatCallback>();

        public List<ShatMessage> GetLastMessages()
        {
            callbacks.Add(OperationContext.Current.GetCallbackChannel<IShatCallback>());

            return lastMessages;
        }

        public void SendMessage(ShatMessage message)
        {
            for (int i = 0; i < callbacks.Count; ++i)
                callbacks[i].BroadcastMessage(message);

            if (lastMessages.Count > 5)
                lastMessages.RemoveAt(0);
            lastMessages.Add(message);
        }

        public void Unsubscribe()
        {
            for (int i = 0; i < callbacks.Count; ++i)
            {
                if (callbacks[i] == OperationContext.Current.GetCallbackChannel<IShatCallback>())
                    callbacks.RemoveAt(i);
            }
        }
    }
}

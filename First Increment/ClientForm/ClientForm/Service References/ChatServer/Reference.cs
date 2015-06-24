﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClientForm.ChatServer {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="User", Namespace="http://schemas.datacontract.org/2004/07/ChatService")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(ClientForm.ChatServer.ChatMessage[]))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(ClientForm.ChatServer.ChatMessage))]
    public partial class User : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private object callbackField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid idField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string usernameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public object callback {
            get {
                return this.callbackField;
            }
            set {
                if ((object.ReferenceEquals(this.callbackField, value) != true)) {
                    this.callbackField = value;
                    this.RaisePropertyChanged("callback");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid id {
            get {
                return this.idField;
            }
            set {
                if ((this.idField.Equals(value) != true)) {
                    this.idField = value;
                    this.RaisePropertyChanged("id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string username {
            get {
                return this.usernameField;
            }
            set {
                if ((object.ReferenceEquals(this.usernameField, value) != true)) {
                    this.usernameField = value;
                    this.RaisePropertyChanged("username");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ChatMessage", Namespace="http://schemas.datacontract.org/2004/07/ChatService")]
    [System.SerializableAttribute()]
    public partial class ChatMessage : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string textField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ClientForm.ChatServer.User userField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string text {
            get {
                return this.textField;
            }
            set {
                if ((object.ReferenceEquals(this.textField, value) != true)) {
                    this.textField = value;
                    this.RaisePropertyChanged("text");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ClientForm.ChatServer.User user {
            get {
                return this.userField;
            }
            set {
                if ((object.ReferenceEquals(this.userField, value) != true)) {
                    this.userField = value;
                    this.RaisePropertyChanged("user");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="Server", ConfigurationName="ChatServer.IChat", CallbackContract=typeof(ClientForm.ChatServer.IChatCallback))]
    public interface IChat {
        
        [System.ServiceModel.OperationContractAttribute(Action="Server/IChat/Subscribe", ReplyAction="Server/IChat/SubscribeResponse")]
        ClientForm.ChatServer.User Subscribe(System.Guid id, string username);
        
        [System.ServiceModel.OperationContractAttribute(Action="Server/IChat/Subscribe", ReplyAction="Server/IChat/SubscribeResponse")]
        System.Threading.Tasks.Task<ClientForm.ChatServer.User> SubscribeAsync(System.Guid id, string username);
        
        [System.ServiceModel.OperationContractAttribute(Action="Server/IChat/GetLastMessages", ReplyAction="Server/IChat/GetLastMessagesResponse")]
        ClientForm.ChatServer.ChatMessage[] GetLastMessages();
        
        [System.ServiceModel.OperationContractAttribute(Action="Server/IChat/GetLastMessages", ReplyAction="Server/IChat/GetLastMessagesResponse")]
        System.Threading.Tasks.Task<ClientForm.ChatServer.ChatMessage[]> GetLastMessagesAsync();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="Server/IChat/SendPublicMessage")]
        void SendPublicMessage(System.Guid from, string message);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="Server/IChat/SendPublicMessage")]
        System.Threading.Tasks.Task SendPublicMessageAsync(System.Guid from, string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="Server/IChat/ConnectWithUser", ReplyAction="Server/IChat/ConnectWithUserResponse")]
        ClientForm.ChatServer.User ConnectWithUser(System.Guid from, System.Guid to);
        
        [System.ServiceModel.OperationContractAttribute(Action="Server/IChat/ConnectWithUser", ReplyAction="Server/IChat/ConnectWithUserResponse")]
        System.Threading.Tasks.Task<ClientForm.ChatServer.User> ConnectWithUserAsync(System.Guid from, System.Guid to);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="Server/IChat/SendPrivateMessage")]
        void SendPrivateMessage(System.Guid from, string message, System.Guid to);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="Server/IChat/SendPrivateMessage")]
        System.Threading.Tasks.Task SendPrivateMessageAsync(System.Guid from, string message, System.Guid to);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="Server/IChat/Unsubscribe")]
        void Unsubscribe();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="Server/IChat/Unsubscribe")]
        System.Threading.Tasks.Task UnsubscribeAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IChatCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="Server/IChat/BroadcastMessage")]
        void BroadcastMessage(ClientForm.ChatServer.ChatMessage message);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="Server/IChat/DeliverMessage")]
        void DeliverMessage(ClientForm.ChatServer.ChatMessage message);
        
        [System.ServiceModel.OperationContractAttribute(Action="Server/IChat/OpenHost", ReplyAction="Server/IChat/OpenHostResponse")]
        bool OpenHost(ClientForm.ChatServer.User from);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IChatChannel : ClientForm.ChatServer.IChat, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ChatClient : System.ServiceModel.DuplexClientBase<ClientForm.ChatServer.IChat>, ClientForm.ChatServer.IChat {
        
        public ChatClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public ChatClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public ChatClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ChatClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ChatClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public ClientForm.ChatServer.User Subscribe(System.Guid id, string username) {
            return base.Channel.Subscribe(id, username);
        }
        
        public System.Threading.Tasks.Task<ClientForm.ChatServer.User> SubscribeAsync(System.Guid id, string username) {
            return base.Channel.SubscribeAsync(id, username);
        }
        
        public ClientForm.ChatServer.ChatMessage[] GetLastMessages() {
            return base.Channel.GetLastMessages();
        }
        
        public System.Threading.Tasks.Task<ClientForm.ChatServer.ChatMessage[]> GetLastMessagesAsync() {
            return base.Channel.GetLastMessagesAsync();
        }
        
        public void SendPublicMessage(System.Guid from, string message) {
            base.Channel.SendPublicMessage(from, message);
        }
        
        public System.Threading.Tasks.Task SendPublicMessageAsync(System.Guid from, string message) {
            return base.Channel.SendPublicMessageAsync(from, message);
        }
        
        public ClientForm.ChatServer.User ConnectWithUser(System.Guid from, System.Guid to) {
            return base.Channel.ConnectWithUser(from, to);
        }
        
        public System.Threading.Tasks.Task<ClientForm.ChatServer.User> ConnectWithUserAsync(System.Guid from, System.Guid to) {
            return base.Channel.ConnectWithUserAsync(from, to);
        }
        
        public void SendPrivateMessage(System.Guid from, string message, System.Guid to) {
            base.Channel.SendPrivateMessage(from, message, to);
        }
        
        public System.Threading.Tasks.Task SendPrivateMessageAsync(System.Guid from, string message, System.Guid to) {
            return base.Channel.SendPrivateMessageAsync(from, message, to);
        }
        
        public void Unsubscribe() {
            base.Channel.Unsubscribe();
        }
        
        public System.Threading.Tasks.Task UnsubscribeAsync() {
            return base.Channel.UnsubscribeAsync();
        }
    }
}

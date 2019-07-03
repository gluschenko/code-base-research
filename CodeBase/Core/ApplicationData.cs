using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CodeBase
{
    [DataContract]
    public class ApplicationData
    {
        [DataMember]
        public bool AutoUpdate = true;
        [DataMember]
        public bool SendData = false;
        [DataMember]
        public int UpdateInterval = 60;
        [DataMember]
        public string ReceiverURL = "";
        [DataMember]
        public string UserID = "";
        [DataMember]
        public string Token = "";
        [DataMember]
        public Project[] Projects = new Project[0];
    }
}

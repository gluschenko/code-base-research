using System.Runtime.Serialization;
using System.Windows;

namespace CodeBase.Core
{
    [DataContract]
    public class ApplicationData
    {
        [DataMember]
        public double WindowWidth;
        [DataMember]
        public double WindowHeight;
        [DataMember]
        public WindowState WindowState;
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

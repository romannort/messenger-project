using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

namespace iMessenger
{
    [Serializable]
    public class Message
    {
        public String Text { get; set; }
        public String SenderName { get; set; }
        public IPAddress SenderIP { get; set; }
        public String ConferenceNumber { get; set; }
        public List<String> Receivers { get; set; }
        public MessageType Type { get; set; }

        public String GetMessageString()
        {

            var messageString = "[" + DateTime.Now.ToString("HH:mm:ss") + "] ";
            switch (Type)
            {
                case MessageType.Common:
                    return messageString + SenderName + ": " + Text;
                case MessageType.JoinCommon:
                    return  messageString + " <SYSTEM>: " + SenderName + " going online";
                case MessageType.LeaveCommon:
                    return messageString  + " <SYSTEM>: " + SenderName + " has gone offline";
                case MessageType.ChangeName:
                    return messageString + " <SYSTEM>: " + SenderName + " changed nickname to " + Text;
                case MessageType.Echo:
                    return messageString + " <SYSTEM>: Echo from " + SenderName;
                case MessageType.Conference:
                    return messageString + SenderName + ": " + Text;
                case MessageType.LeaveConference:
                    return messageString + " <SYSTEM>: " + SenderName + " has left this conference";
                case MessageType.JoinConference:
                    return messageString + " <SYSTEM>: " + SenderName + " joined conference";
                default:
                    return "ERROR!!!";
            }
        }

        public static Message Deserialize( Byte[] buffer)
        {
            IFormatter formatter = new BinaryFormatter();
            var s = new MemoryStream(buffer);
            var m = (Message)formatter.Deserialize(s);
            s.Close();
            return m;
        }
        public static Byte[] Serialize(Message m)
        {
            IFormatter formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            formatter.Serialize(stream, m);
            var buffer = stream.ToArray();
            stream.Close();
            
            return buffer;
        }
    }

    public enum MessageType { Common, System, Conference, ChangeName, JoinCommon, Echo, LeaveCommon, LeaveConference, JoinConference}
}

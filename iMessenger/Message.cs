using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

namespace iMessenger
{
    /// <summary>
    /// Encapsulates data related to transfered message.
    /// </summary>
    [Serializable]
    public class Message
    {
        /// <summary>
        /// Message text
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// Name of sender.
        /// </summary>
        public String SenderName { get; set; }

        /// <summary>
        /// Sender's IP address
        /// </summary>
        public IPAddress SenderIP { get; set; }

        /// <summary>
        /// Name of conference if message have type MessageType.Conference
        /// </summary>
        public String ConferenceNumber { get; set; }

        /// <summary>
        /// List of receivers nicknames. All other users can't see this message.
        /// </summary>
        public ICollection<String> Receivers { get; set; }

        /// <summary>
        /// Type of message
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        /// Generates displayed message string.
        /// </summary>
        /// <returns> String with text for display in message window. </returns>
        public String GetMessageString()
        {
            String messageString = "[" + DateTime.Now.ToString("HH:mm:ss") + "] ";
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

        /// <summary>
        /// Deserialize byte array to Message object
        /// </summary>
        /// <param name="buffer"> Byte array with raw data. </param>
        /// <returns> Message object </returns>
        public static Message Deserialize( Byte[] buffer)
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream s = new MemoryStream(buffer);
            Message m = (Message)formatter.Deserialize(s);
            s.Close();
            return m;
        }

        /// <summary>
        /// Serialize Message objcet to byte array
        /// </summary>
        /// <param name="m"> Message object </param>
        /// <returns> Byte array. </returns>
        public static Byte[] Serialize(Message m)
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, m);
            Byte[] buffer = stream.ToArray();
            stream.Close();
            return buffer;
        }
    }

    #region MessageTypes

    public enum MessageType
    {
        Common, 
        System, 
        Conference, 
        ChangeName, 
        JoinCommon, 
        Echo, 
        LeaveCommon, 
        LeaveConference, 
        JoinConference
    }
    
    #endregion
}

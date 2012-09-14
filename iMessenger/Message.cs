using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace iMessenger
{
    [Serializable]
    public class Message
    {
        public String Text { get; set; }
        public static String[] ReceiverName; // array of user if conference, null if broadcast ( offline, online )
        public String SenderName { get; set; }
        public String SenderIP { get; set; }
        public MessageType Type { get; set; }    
        
        public Message()
        {
            ReceiverName = new String[1];
        }

        public String getMessageString()
        {
            return "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + SenderName + ": " + Text;  
        }

        public static Message Deserialize( Byte[] buffer)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream s = new MemoryStream(buffer);
            Message m = (Message)formatter.Deserialize(s);
            s.Close();
            return m;
        }
        public static Byte[] Serialize( Message m)
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, m);
            Byte[] buffer = stream.ToArray();
            stream.Close();
            
            return buffer;
        }

        public static string GenerateSystemMessage(string text)
        {
            return "[" + DateTime.Now.ToString("HH:mm:ss") + "] <SYSTEM>: " + text;
        }

        public static void AddInRecievers(String ip)
        {
            Array.Resize(ref ReceiverName, ReceiverName.Length + 1);
            ReceiverName[ReceiverName.Length - 1] = ip;
        }
    }

    public enum MessageType { Text, System, Status, ChangeName, Joined, Echo, LogOut }
}

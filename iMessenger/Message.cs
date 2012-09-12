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
        public String[] ReceiverName { get; set; } // array of user if conference, null if broadcast ( offline, online )
        public String SenderName { get; set; }
        public String Type { get; set; }    // type of message- > "text", "system" ( log in, out, change nick, etc), "status"( change status, etc)
        
        public String getMessageString()
        {
            return "[" + DateTime.Now.ToString("hh:mm:ss") + "] " + SenderName + ": " + Text;  
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
        
    }
}

﻿using System;
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
        public MessageType Type { get; set; }    
        
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
        
    }

    public enum MessageType { Text = 0, System, Status, ChangeName, Joined, Echo, LogOut }
}

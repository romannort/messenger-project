﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Net;

namespace iMessenger
{
    [Serializable]
    public class Message
    {
        public String Text { get; set; }
        public String SenderName { get; set; }
        public IPAddress SenderIP { get; set; }
        public List<String> Receivers { get; set; }
        public MessageType Type { get; set; }    
        
        public Message()
        {
        }

        public String getMessageString()
        {

            String messageString = "[" + DateTime.Now.ToString("HH:mm:ss") + "] ";
            switch (Type)
            {
                case MessageType.Text:
                    return messageString + SenderName + ": " + Text;
                case MessageType.Joined:
                    return  messageString + " <SYSTEM>: " + SenderName + " joined conference";
                case MessageType.LogOut:
                    return messageString  + " <SYSTEM>: " + SenderName + " has left conference";
                case MessageType.ChangeName:
                    return messageString + " <SYSTEM>: " + SenderName + " changed nickname to " + Text;
                case MessageType.Echo:
                    return messageString + " <SYSTEM>: Echo from " + SenderName;
                case MessageType.Conference:
                    return messageString + " <SYSTEM>: Test-test-test Conference";
                default:
                    return "ERROR!!!";
            }
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

    public enum MessageType { Text, System, Conference, ChangeName, Joined, Echo, LogOut }
}

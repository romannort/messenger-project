
using System;

namespace iMessenger
{
    public class MsgReceiveEventArgs
    {
        private readonly Message message;

        public MsgReceiveEventArgs(Message message)
        {
            this.message = message;
        }

        public MsgReceiveEventArgs( Byte[] serializedMessage)
        {
            message = Message.Deserialize(serializedMessage);
        }

        public Message Message 
        { 
            get
            {
                return message;
            } 
        }
    }
}

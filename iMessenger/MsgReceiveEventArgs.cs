using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iMessenger
{
    public class MsgReceiveEventArgs
    {
        private readonly Message message;

        public MsgReceiveEventArgs(Message aMessage)
        {
            message = aMessage;
        }

        public Message Message { get { return message; } }
    }
}

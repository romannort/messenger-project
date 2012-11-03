using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace iMessenger
{
    public static class MessageManager
    {
        public static event EventHandler<MsgReceiveEventArgs> NewMessage;

        public static void OnNewMessage(MsgReceiveEventArgs e)
        {
            EventHandler<MsgReceiveEventArgs> temp = Interlocked.CompareExchange(ref NewMessage, null, null);
            if (temp != null)
            {
                temp(null, e);
            }
        }
    }
}

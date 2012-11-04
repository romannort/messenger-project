using System;
using System.Threading;

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

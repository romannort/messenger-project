using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace iMessenger
{
    public sealed class Receiver
    {
        private static readonly Receiver current = new Receiver();
        private UdpClient receiveClient;
        private IPEndPoint receiveEndPoint;
        private Thread receivingThread;


        private Receiver()
        {
            ConfigureReceiver();            
        }

        public static Receiver Current
        {
            get
            {
                return current;
            }
        }

        private void ConfigureReceiver()
        {
            try
            {
                receiveEndPoint = new IPEndPoint(IPAddress.Any, 1800);
                receiveClient = new UdpClient();
                receiveClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                receiveClient.ExclusiveAddressUse = false;
                receiveClient.Client.Bind(receiveEndPoint);
            }
            catch (SocketException e)
            {
               
            }
        }
        
        public void Start()
        {
            receivingThread = new Thread(ReceiveMessages);
            receivingThread.Start();
        }

        private void ReceiveMessages()
        {
            while ( AnalyzeReceivedData() )
            {
                
            }
            Thread.CurrentThread.Abort();
            throw new Exception("Receiving interrupted.");
        }

        private Boolean AnalyzeReceivedData()
        {
            MessageManager.OnNewMessage( new MsgReceiveEventArgs((receiveClient.Receive(ref receiveEndPoint))));
            return true;
        }
    }
}

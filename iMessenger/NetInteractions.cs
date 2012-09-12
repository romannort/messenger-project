using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace iMessenger
{
    class NetInteraction
    {
        private static UdpClient receiveClient = new UdpClient(1800);
        private static IPEndPoint receiveEndPint = new IPEndPoint(IPAddress.Any, 0);
        public static  MainWindow window;

        public NetInteraction()
        {
            Thread receivingThread = new Thread(ReceiveMessages);
            receivingThread.Start();
        }
        public void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    Byte[] receivedData = receiveClient.Receive(ref receiveEndPint);
                    Message message = Message.Deserialize(receivedData);

                    // change later
                    if (message.Text.Contains(" logged out.") )
                    {
                        break;
                    }
                    else
                    {
                        window.ShowMessage(message);
                    }
                }
                Thread.CurrentThread.Abort();
                Environment.Exit(0x0);
            }
            catch (ThreadAbortException ex)
            {
                Environment.Exit(0x0);
            }

        }

        static public void SendMessage(Byte[] data)
        {
            if (data == null) return;   // add exception later

            UdpClient sendClient = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 1800);
            sendClient.Send(data, data.Length, endPoint);
            sendClient.Close();

        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iMessenger
{
    public  class Core
    {
        private  UdpClient receiveClient = new UdpClient(1800);
        private  IPEndPoint receiveEndPoint = new IPEndPoint(IPAddress.Any, 0);
        public   MainWindow window;


        public Core(MainWindow window)
        {
            //window = new MainWindow();
            this.window = window;
        }

        public  void StartReceive()
        {
            Thread receivingThread = new Thread(ReceiveMessages);
            receivingThread.Start(); 
        }
        public  string GetUserIP()
        {
            string UserName = "";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.First(ip => ip.AddressFamily.ToString() == "InterNetwork").ToString();

        }

        public  void SendMessage(Byte[] data)
        {
            try{
                UdpClient sendClient = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 1800);
                sendClient.Send(data, data.Length, endPoint);
                sendClient.Close();
            }catch( NullReferenceException e){

            }
        }

        public  void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    Byte[] receivedData = receiveClient.Receive(ref receiveEndPoint);
                    Message message = Message.Deserialize(receivedData);

                    // change later
                    if (message.Text.Contains(" logged out.") && message.SenderName == MainWindow.UserName)
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

    }
}

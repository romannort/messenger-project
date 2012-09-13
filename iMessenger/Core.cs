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
    public class Core
    {
        private  UdpClient receiveClient = new UdpClient(1800);
        private  IPEndPoint receiveEndPoint = new IPEndPoint(IPAddress.Any, 0);
        public   MainWindow window;
        public  String UserName { get; set; }


        public Core(MainWindow window)
        {
            this.window = window;
            UserName = GetUserIP();
        }

        public  void StartReceiving()
        {
            Thread receivingThread = new Thread(ReceiveMessages);
            receivingThread.Start(); 
        }
        public  string GetUserIP()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.First(ip => ip.AddressFamily.ToString() == "InterNetwork").ToString();
        }

        public void SendMessage(Message m)
        {
            try{
                Byte[] data = Message.Serialize(m);
                UdpClient sendClient = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 1800);
                sendClient.Send(data, data.Length, endPoint);
                sendClient.Close();
            }catch( NullReferenceException e){

            }
        }

        public void ReceiveMessages()
        {
            try
            {
                while ( AnalyzeReceivedData() ) { }
                Thread.CurrentThread.Abort();
                Environment.Exit(0x0);
            }
            catch (ThreadAbortException ex)
            {
                Environment.Exit(0x0);
            }

        }

        private Boolean AnalyzeReceivedData(){

            Message message = Message.Deserialize(receiveClient.Receive(ref receiveEndPoint));

            if (message.Text.Contains("logged out.") && message.SenderName == UserName && message.Type == MessageType.System)
            {
                return false;
            }
            else if (message.Text.Contains("joined conference.") && message.SenderName != UserName && message.Type == MessageType.System)
            {
                window.AddAtConnectList(message.SenderName);
                SendMessage(window.GenerateMessage("Hello!", MessageType.System));
            }
            else if (message.Text == "Hello!" && message.Type == MessageType.System)
            {
                window.AddAtConnectList(message.SenderName);
                return true;
            }
            else if (message.Text.Contains("changed nickname to") && message.SenderName != UserName && message.Type == MessageType.System)
            {
                string newnick = message.Text;
                newnick.Replace(" changed nickname to ", "");
                newnick.Replace(message.SenderName, "");
                window.ChangeConnectList(message.SenderName, newnick);
                return true;
            }
            else if (message.Text.Contains("logged out.") && message.SenderName != UserName && message.Type == MessageType.System)
            {
                window.ReplaceConnectList(message.SenderName);
            }
            window.ShowMessage(message);
            return true;
        }
        

    }
}

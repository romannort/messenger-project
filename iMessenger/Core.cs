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
        private UdpClient receiveClient;
        private  IPEndPoint receiveEndPoint = new IPEndPoint(IPAddress.Any, 1800);
        public   MainWindow window;
        public  String UserName { get; set; }


        public Core(MainWindow window)
        {
            try{
                this.window = window;
                UserName = GetUserIP();

                receiveClient = new UdpClient();
                receiveClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                receiveClient.ExclusiveAddressUse = false;
                receiveClient.Client.Bind(receiveEndPoint);
                
            }catch( SocketException e ){

            }
            
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
                sendClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

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

            Message message;
            try{
                message = Message.Deserialize( receiveClient.Receive(ref receiveEndPoint));
            }catch( NullReferenceException e){
                return true;
            }
            
            LogHelper.WriteLog(message);
            window.ShowMessage(message);

            if (message.Text.Contains("logged out.") && message.SenderName == UserName && message.Type == MessageType.System)
            {
                return false;
            }
            else if (message.Text.Contains("joined conference.")  && message.Type == MessageType.System)
            {
                window.AddAtConnectList(message.SenderName);
                if ( message.SenderName != UserName)
                    SendMessage(window.GenerateMessage("Hello!", MessageType.System));
            }
            else if (message.Text.Contains("changed nickname to") &&  message.Type == MessageType.System)
            {
                String newnick = message.Text.Replace(" changed nickname to ", "").Replace(message.SenderName, "");
                newnick = newnick.Trim();
                window.ChangeConnectList(message.SenderName, newnick);
                return true;
            }
            else if (message.Text.Contains("logged out.") && message.SenderName != UserName && message.Type == MessageType.System)
            {
                window.ReplaceConnectList(message.SenderName);
            }
            if (message.Text == "Hello!" && message.Type == MessageType.System && message.SenderName != UserName)
            {
                window.AddAtConnectList(message.SenderName);
            }
            
            return true;
        }

        
    }
}

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
        public IPAddress UserIP;


        public Core(MainWindow window)
        {
            try{
                this.window = window;
                UserName = "User#" + System.DateTime.Now.ToString("ddHHmmss");
                UserIP = GetUserIP();

                ConfigureReceiver();
                StartReceiving();
            }catch( SocketException e ){

            }
            
        }

        private void ConfigureReceiver()
        {
            receiveClient = new UdpClient();
            receiveClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            receiveClient.ExclusiveAddressUse = false;
            receiveClient.Client.Bind(receiveEndPoint);
        }
        private void StartReceiving()
        {
            Thread receivingThread = new Thread(ReceiveMessages);
            receivingThread.Start(); 
        }
        public IPAddress GetUserIP()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.First(ip => ip.AddressFamily.ToString() == "InterNetwork");
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
            }
            catch( NullReferenceException e)
            {
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

            //LogHelper.WriteLog(message);

            switch (message.Type)
            {
                case MessageType.LogOut:
                    {
                        window.ReplaceConnectList(message.SenderName);
                        break;
                    }
                case MessageType.Joined:
                    {
                        window.AddAtConnectList(message.SenderName, false);
                        if (message.SenderName != UserName)
                            SendMessage(window.GenerateMessage("", MessageType.Echo));
                        break;
                    }
                case MessageType.ChangeName:
                    {
                        window.ChangeConnectList(message.SenderName, message.Text);
                        break;
                    }
                case MessageType.Echo:
                    {
                        if (message.SenderName != UserName)
                            window.AddAtConnectList(message.SenderName, false);
                        return true;
                    }
                case MessageType.Text:
                    {
                        if (message.Receivers.Contains(UserName) == false)
                        {
                            return true;
                        }
                        break;
                    }
            }
            window.ShowMessage(message);
            return true;
        }        
    }
}

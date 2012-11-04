using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;


namespace iMessenger
{
    public class Core
    {
        public MainWindow Window;
        public String UserName { get; set; }
        public IPAddress UserIP;

        public Core(MainWindow window)
        {
            Window = window;
            UserName = "User#" + DateTime.Now.ToString("ddHHmmss");
            UserIP = GetUserIP();
            MessageManager.NewMessage += OnMessageReceive;
            Receiver.Current.Start();
        }

        public IPAddress GetUserIP()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.First(ip => ip.AddressFamily.ToString() == "InterNetwork");
        }

        public void SendMessage(Message m)
        {
            Byte[] data = Message.Serialize(m);
            UdpClient sendClient = new UdpClient();
            sendClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 1800);
            sendClient.Send(data, data.Length, endPoint);
            sendClient.Close();
        }
        
        private void OnMessageReceive(object sender, MsgReceiveEventArgs e)
        {
            switch (e.Message.Type)
            {
                case MessageType.LeaveCommon:
                    {
                        Window.ReplaceConnectList(e.Message.SenderName);
                        break;
                    }
                case MessageType.JoinCommon:
                    {
                        Window.AddAtConnectList(e.Message.SenderName);
                        if (e.Message.SenderName != UserName)
                        {
                            SendMessage(Window.GenerateMessage(String.Empty , MessageType.Echo));
                        }
                        break;
                    }
                case MessageType.ChangeName:
                    {
                        Window.ChangeConnectList(e.Message.SenderName, e.Message.Text);
                        break;
                    }
                case MessageType.Echo:
                    {
                        if (e.Message.SenderName != UserName)
                        {
                            Window.AddAtConnectList(e.Message.SenderName);
                        }
                        return;
                    }
                default:
                    {
                        if (e.Message.Receivers.Contains(UserName) == false)
                        {
                            return;
                        }
                        break;
                    }
            }
            Window.ShowMessage(e.Message);
        }
    }
}

﻿using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace iMessenger
{
    public class Core
    {
        /// <summary>
        /// Reference to application MainWindow
        /// </summary>
        public MainWindow Window;
        private UdpClient _receiveClient;
        private IPEndPoint _receiveEndPoint = new IPEndPoint(IPAddress.Any, 1800);
        public String UserName { get; set; }
        public IPAddress UserIP;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="window"> Pointer at MainWindow </param>
        public Core(MainWindow window)
        {
            Window = window;
            UserName = "User#" + DateTime.Now.ToString("ddHHmmss");
            UserIP = GetUserIP();
            MessageManager.NewMessage += OnMessageReceive;

            ConfigureReceiver();
            StartReceiving();
        }

        /// <summary>
        /// Configures receiver
        /// </summary>
        private void ConfigureReceiver()
        {
            _receiveClient = new UdpClient();
            _receiveClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _receiveClient.ExclusiveAddressUse = false;
            _receiveClient.Client.Bind(_receiveEndPoint);
        }

        /// <summary>
        /// Starts receiving
        /// </summary>
        private void StartReceiving()
        {
            Thread receivingThread = new Thread(ReceiveMessages);
            receivingThread.Start(); 
        }

        /// <summary>
        /// Gets user IP
        /// </summary>
        /// <returns> Current IP </returns>
        public IPAddress GetUserIP()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.First(ip => ip.AddressFamily.ToString() == "InterNetwork");
        }

        /// <summary>
        /// Sends message
        /// </summary>
        /// <param name="m"> Message for sending </param>
        public void SendMessage(Message m)
        {
            byte[] data = Message.Serialize(m);
            UdpClient sendClient = new UdpClient();
            sendClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 1800);
            sendClient.Send(data, data.Length, endPoint);
            sendClient.Close();
        }

        /// <summary>
        /// Receiving messages
        /// </summary>
        public void ReceiveMessages()
        {
            while (AnalyzeReceivedData()) { }                                   //ЭТО ЧЁ ЗА КОСТЫЛЬ?
            Thread.CurrentThread.Abort();                                       //|
            Environment.Exit(0x0);                                              //|
        }                                                                       //|
                                                                                //|
        /// <summary>                                                           //|
        /// Analysing of received data                                          //|
        /// </summary>                                                          //|
        private bool AnalyzeReceivedData()                                      //|
        {                                                                       //|
            //LogHelper.WriteLog(message);                                      //|
            MessageManager.OnNewMessage(new MsgReceiveEventArgs(Message.Deserialize(_receiveClient.Receive(ref _receiveEndPoint))));
            return true;    //<---------------------------------------------------|
        }

        /// <summary>
        /// Processing of received message
        /// </summary>
        /// <param name="sender"> Pointer at </param>
        /// <param name="e"> MsgReceiveEvent arguments </param>
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
                            SendMessage(Window.GenerateMessage("", MessageType.Echo));
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

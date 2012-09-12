﻿using System;
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
        private UdpClient receiveClient = new UdpClient(1800);
        private IPEndPoint receiveEndPoint = new IPEndPoint(IPAddress.Any, 0);
        MainWindow window;

        
        public Core()
        {
            window = new MainWindow() { core = this };
            Thread receivingThread = new Thread(ReceiveMessages);
            receivingThread.Start();
        }

        public string GetUserName()
        {
            string UserName = "";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
                if (ip.AddressFamily.ToString() == "InterNetwork")
                    UserName = ip.ToString();
            return UserName;
        }

        public void SendMessage(String data)
        {
            if (data == null) return;   // add exception later

            UdpClient sendClient = new UdpClient();
            Byte[] message = Encoding.ASCII.GetBytes(data);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 1800);
            sendClient.Send(message, message.Length, endPoint);
            sendClient.Close();
        }

        public void SendMessage(Byte[] data)
        {
            try{
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
                while (true)
                {
                    Byte[] receivedData = receiveClient.Receive(ref receiveEndPoint);
                    Message message = Message.Deserialize(receivedData);

                    // change later
                    if (message.Text.Contains(" logged out.") && message.SenderName == window.UserName)
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

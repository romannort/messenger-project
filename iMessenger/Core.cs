using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace iMessenger
{
    public class Core
    {
        /// <summary>
        /// Reference to application MainWindow
        /// </summary>
        public MainWindow Window;

        /// <summary>
        /// Information about User
        /// </summary>
        public readonly User User = new User();

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="window"> Pointer at MainWindow </param>
        public Core(MainWindow window)
        {
            Window = window;
            User.Name = "User#" + DateTime.Now.ToString("ddHHmmss");
            User.IP = GetUserIP();
            MessageManager.NewMessage += OnMessageReceive;
            Receiver.Current.Start();
        }

        /// <summary>
        /// Configures receiver
        /// </summary>

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
            Byte[] data = Message.Serialize(m);
            UdpClient sendClient = new UdpClient();
            sendClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 1800);
            sendClient.Send(data, data.Length, endPoint);
            sendClient.Close();
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
                        Window.AddToConnectList(e.Message.SenderName);
                        if (e.Message.SenderName != User.Name)
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
                        if (e.Message.SenderName != User.Name)
                        {
                            Window.AddToConnectList(e.Message.SenderName);
                        }
                        return;
                    }
                default:
                    {
                        if (e.Message.Receivers.Contains(User.Name) == false)
                        {
                            return;
                        }
                        break;
                    }
            }
            Window.ShowMessage(e.Message);
            LogHelper.Current.WriteLog(e.Message);
        }

        /// <summary>
        /// Event handler to process all unhandled errors.
        /// </summary>
        /// <param name="sender"> Event sender </param>
        /// <param name="e"> Event arguments </param>
        public void OnErrorRaised(object sender, DispatcherUnhandledExceptionEventArgs e )
        {
            e.Handled = true;
            ErrorWindow errorWindow = new ErrorWindow {ErrorMessage = {Content = e.Exception.Message}};

            errorWindow.Show();
            errorWindow.Closed += Window.OnErrorWindowClosed;
        }
    }
}

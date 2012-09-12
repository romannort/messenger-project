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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public String UserName { get; set; }

        // Move to Net Classes
        //private UdpClient receiveClient = new UdpClient(1800);
        //private IPEndPoint receiveEndPint = new IPEndPoint(IPAddress.Any, 0);
        // 


        public MainWindow()
        {
            InitializeComponent();
            NetInteraction.window = this;
        }

        private void Chat_Loaded(object sender, RoutedEventArgs e)
        {
            // Decrease new line margin 
            ChatArea.Document.Blocks.Add( new Paragraph( new Run( UserName + " joined conference.")));

            // Move to Net classes ?
            //Thread receivingThread = new Thread(ReceiveMessages);
            //receivingThread.Start();
            //

            NetInteraction net = new NetInteraction();

            MessageBox.Focus();
        }

        
        /// <summary>
        ///  Move to Net classes ?
        /// </summary>
        //private void ReceiveMessages()
        //{
        //    try
        //    {
        //        while (true)
        //        {
        //            Byte[] receivedData = receiveClient.Receive(ref receiveEndPint);
        //            Message message = Message.Deserialize(receivedData);

        //            // change later
        //            if (message.Text.Contains(" logged out.") && message.SenderName == UserName)
        //            {
        //                break;
        //            }
        //            else
        //            {
        //                ShowMessage(message);
        //            }
        //        }
        //        Thread.CurrentThread.Abort();
        //        Environment.Exit(0x0);
        //    }
        //    catch (ThreadAbortException ex)
        //    {
        //        Environment.Exit(0x0);
        //    }

        //}

        // Move to Net Classes
        
        //private void SendMessage(String data) 
        //{
        //    if (data == null) return;   // add exception later

        //    UdpClient sendClient = new UdpClient();
        //    Byte[] message = Encoding.ASCII.GetBytes(data);
        //    IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 1800);
        //    sendClient.Send(message, message.Length, endPoint);
        //    sendClient.Close();


        //}

        /// <summary>
        /// Test serialization and sending
        /// </summary>
        /// <param name="data"></param>
        //private void SendMessage(Byte[] data)
        //{
        //    if (data == null) return;   // add exception later

        //    UdpClient sendClient = new UdpClient();
        //    IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 1800);
        //    sendClient.Send(data, data.Length, endPoint);
        //    sendClient.Close();

        //}

        /// <summary>
        /// Move to Net classes
        /// </summary>
        /// <param name="message"></param>
        public void ShowMessage( Message message)
        {
            Dispatcher.Invoke((ThreadStart)delegate
            {
                // Decrease new line margin 
                ChatArea.Document.Blocks.Add(new Paragraph(new Run(message.getMessageString() )));
            });
            // logging ?
            // feedback ?
        }
       

        private void Chat_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            String data = UserName + " logged out.";

            NetInteraction.SendMessage( Message.Serialize(new Message(){ Text = data }));
            Environment.Exit(0x0);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            NetInteraction.SendMessage(Message.Serialize(GetMessage()));
        }

        private void MessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NetInteraction.SendMessage(Message.Serialize(GetMessage()));
            }
        }
        
        /// <summary>
        /// Gets message text from MessageBox input
        /// </summary>
        /// <returns></returns>
        private String GetMessageText()
        {
            if (!String.IsNullOrEmpty(MessageBox.Text))
            {
                String data = MessageBox.Text;
                MessageBox.Clear();
                MessageBox.Focus();
                return data;
            }  
            return null;
        }


        // Rename to GenerateMessage ?
        /// <summary>
        /// Generates Message object
        /// </summary>
        /// <returns></returns>
        private Message GetMessage(){

            return new Message()
            {
                SenderName = UserName,
                ReceiverName = null,
                Text = GetMessageText(),
                Time = DateTime.Now
            };
        }
        
    }
    
}

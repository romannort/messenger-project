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
        private Core core;


        public MainWindow(Core a)
        {
            core = a;
            InitializeComponent();
            NetInteraction.window = this;
        }

        private void Chat_Loaded(object sender, RoutedEventArgs e)
        {
            // Decrease new line margin
            NickBox.Text = core.GetUserName();
            UserName = NickBox.Text;
            core.SendMessage(Message.Serialize(GenerateMessage(UserName + " joined conference.")));

            NetInteraction net = new NetInteraction();

            MessageBox.Focus();
        }

        // Move to Net Classes
        //    UdpClient sendClient = new UdpClient();
        //    Byte[] message = Encoding.ASCII.GetBytes(data);
        //    IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 1800);
        //    sendClient.Send(message, message.Length, endPoint);
        //    sendClient.Close();

        public void ShowMessage(Message message)
        //}
        {
            Dispatcher.Invoke((ThreadStart)delegate
            {
                // Decrease new line margin 
                ChatArea.Document.Blocks.Add(new Paragraph(new Run(message.getMessageString())));
            });
        }
       

        private void Chat_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            String data = UserName + " logged out.";

            
            core.SendMessage(Message.Serialize(GenerateMessage(data)));
            Environment.Exit(0x0);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(MessageBox.Text))
                core.SendMessage(Message.Serialize(GenerateMessage(GetMessageText())));
        }

        private void MessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendButton_Click(sender, new RoutedEventArgs());
            }
        }
        
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


        private Message GenerateMessage(string data)
        {
            return new Message()
            {
                SenderName = UserName,
                ReceiverName = null,
                Text = data,
                Time = DateTime.Now
            };
        }

        private void NickBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(NickBox.Text))
            {
                NickBox.Text = UserName;
            }
            else
            {
                core.SendMessage(Message.Serialize(GenerateMessage(UserName + " change nickname to " + NickBox.Text)));
                UserName = NickBox.Text;
            }
        }
        
    }
    
}

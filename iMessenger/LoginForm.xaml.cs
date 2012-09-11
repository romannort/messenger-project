using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Net.Sockets;
using System.Net;

namespace iMessenger
{
    /// <summary>
    /// Interaction logic for LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        public LoginForm()
        {
            InitializeComponent();
            NickName.Focus();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            StartChat();
        }

        private void StartChat()
        {
            if ( String.IsNullOrEmpty( NickName.Text ) == false)
            {
                UdpClient sendClient = new UdpClient();
                Byte[] messageText = Encoding.ASCII.GetBytes(NickName.Text + " joined conference.");
                IPEndPoint sendEndPoint = new IPEndPoint(IPAddress.Broadcast, 1800);
                sendClient.Send(messageText, messageText.Length, sendEndPoint);
                sendClient.Close();

                this.Hide();

                MainWindow chat = new MainWindow() { UserName = NickName.Text };
                chat.Show();
            }
        }
    }
}

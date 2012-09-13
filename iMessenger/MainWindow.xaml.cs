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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        
        public Core Core { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Chat_Loaded(object sender, RoutedEventArgs e)
        {
            try{
                NickBox.Text = Core.UserName;
                Core.SendMessage( GenerateMessage(Core.UserName + " joined conference.", MessageType.System));
                Core.StartReceiving();
                MessageBox.Focus();
               
    
            }catch( NullReferenceException exeption){

            }
        }

    
        public void ShowMessage(Message message)
        {
            Dispatcher.Invoke((ThreadStart)delegate
            {
                ChatArea.Document.Blocks.Add(new Paragraph(new Run(message.getMessageString())));
                ChatArea.ScrollToEnd();
            });
        }
       

        private void Chat_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            String data = Core.UserName + " logged out.";
            Core.SendMessage(GenerateMessage(data, MessageType.Text));
            Environment.Exit(0x0);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(MessageBox.Text))
                Core.SendMessage(GenerateMessage(GetMessageText(), MessageType.Text));
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

        public Message GenerateMessage(String data, MessageType Type)
        {
            return new Message()
            {
                SenderName = Core.UserName,
                ReceiverName = null,
                Text = data,
                Type = Type
            };
        }
        

        private void NickBox_LostFocus(object sender, RoutedEventArgs e)
        {
            NicknameChanging();
        }
        
        private void NicknameChanging(){
            if (!String.IsNullOrEmpty(NickBox.Text) && NickBox.Text != Core.UserName)
            {
                Core.SendMessage(GenerateMessage(Core.UserName + " changed nickname to " + NickBox.Text, MessageType.System));
                Core.UserName = NickBox.Text;
            }
            NickBox.Text = Core.UserName;
        }

        public void AddAtConnectList(string newNick)
        {
            Dispatcher.Invoke((ThreadStart)delegate
            {
                //if (ConnectList.Items.IndexOf( newNick ) == -1 )
                    ConnectList.Items.Add(newNick);
            });
        }

        public void ChangeConnectList(string oldNick, string newNick)
        {
            try{
                Dispatcher.Invoke((ThreadStart)delegate
                {
                    ConnectList.Items.RemoveAt(ConnectList.Items.IndexOf(oldNick));
                    ConnectList.Items.Add(newNick);
                });
            }catch(ArgumentOutOfRangeException e)
            {

            }            
        }

        public void ReplaceConnectList(string oldNick)
        {
            ConnectList.Items.RemoveAt(ConnectList.Items.IndexOf(oldNick));
        }
        
       
    }
    
}

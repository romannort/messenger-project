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

        
        public Core Core { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Chat_Loaded(object sender, RoutedEventArgs e)
        {
            try{
                NickBox.Text = Core.UserName;
                Core.SendMessage( GenerateMessage("", MessageType.Joined));
                
                MessageBox.Focus();    

    
            }catch( NullReferenceException exeption){

            }
        }

    
        public void ShowMessage(Message message)
        {
            try{
                Dispatcher.Invoke((ThreadStart)delegate
                {
                    Run run;
                    RichTextBox rtb;
                    String mText = message.getMessageString();
                    switch (message.Type)
                    {
                        case MessageType.Common:
                            {
                                rtb = (RichTextBox)((Grid)((TabItem)Tabs.Items[0]).Content).Children[1];
                                rtb.Document.Blocks.Add(new Paragraph(new Run(mText)));
                                rtb.ScrollToEnd();
                                break;
                            }
                        case MessageType.Conference:
                            {
                                String ToCompare = message.Text.Remove(8);
                                for (int i = 1; i < Tabs.Items.Count-1; i++)
                                {
                                    if (((TabItem)Tabs.Items[i]).Tag.ToString() == ToCompare)
                                    {
                                        rtb = (RichTextBox)((Grid)((TabItem)Tabs.Items[i]).Content).Children[1];
                                        rtb.Document.Blocks.Add(new Paragraph(new Run(mText)));
                                        rtb.ScrollToEnd();
                                        return;
                                    }
                                }
                                CreateTab(message);
                                rtb = (RichTextBox)((Grid)((TabItem)Tabs.Items[Tabs.Items.Count-2]).Content).Children[1];
                                rtb.Document.Blocks.Add(new Paragraph(new Run(mText)));
                                rtb.ScrollToEnd();
                                break;
                            }
                        default:
                            {
                                CheckBox item;
                                ListBox lb;
                                for (int i = 0; i < Tabs.Items.Count - 1; i++)
                                {
                                    lb = (ListBox)((Grid)((TabItem)Tabs.Items[i]).Content).Children[0];
                                    for (int j = 0; j < lb.Items.Count; j++)
                                    {
                                        item = (CheckBox)lb.Items[j];
                                        if (item.Content.ToString() == message.SenderName && item.IsChecked == true)
                                        {
                                            run = new Run(mText);
                                            run.Foreground = Brushes.DarkGreen;
                                            run.FontStyle = FontStyles.Italic;
                                            rtb = (RichTextBox)((Grid)((TabItem)Tabs.Items[i]).Content).Children[1];
                                            rtb.Document.Blocks.Add(new Paragraph(run));
                                            rtb.ScrollToEnd();
                                        }
                                    }
                                }
                                break;
                            }
                    }

                });
            }catch( NullReferenceException e){

            }
            
        }
       
        private void Chat_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Core.SendMessage(GenerateMessage("", MessageType.LogOut));
            Environment.Exit(0x0);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(MessageBox.Text))
            {
                if(Tabs.SelectedIndex == 0)
                    Core.SendMessage(GenerateMessage(GetMessageText(), MessageType.Common));
                else
                    Core.SendMessage(GenerateMessage(((TabItem)Tabs.SelectedItem).Tag.ToString()+GetMessageText(), MessageType.Conference));
            }
        }

        private void MessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendButton_Click(sender, new RoutedEventArgs());
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
                SenderIP = Core.UserIP,
                Text = data,
                Type = Type,
                Receivers = GetReceiversList()
            };
        }
        
        private void NickBox_LostFocus(object sender, RoutedEventArgs e)
        {
            NicknameChanging();
        }
        
        private void NicknameChanging()
        {
            if (!String.IsNullOrEmpty(NickBox.Text) && NickBox.Text != Core.UserName)
            {
                CheckBox item = new CheckBox();
                bool flag = true;
                for (int i = 0; i < ConnectList0.Items.Count; i++)
                {
                    item = (CheckBox)ConnectList0.Items[i];
                    if (item.Content.ToString() == NickBox.Text)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    Core.SendMessage(GenerateMessage(NickBox.Text, MessageType.ChangeName));
                    Core.UserName = NickBox.Text;
                    return;
                }
                else
                {
                    Dispatcher.Invoke((ThreadStart)delegate
                    {
                        Run run = new Run("This nickname is already in use!");
                        run.Foreground = Brushes.Red;
                        ChatArea0.Document.Blocks.Add(new Paragraph(run));
                        ChatArea0.ScrollToEnd();
                    });
                }
            }
            NickBox.Text = Core.UserName;
        }

        public void AddAtConnectList(string newNick)
        {
            Dispatcher.Invoke((ThreadStart)delegate
            {
                CheckBox item = new CheckBox();
                for (int i = 0; i < ConnectList0.Items.Count; i++)
                {
                    item = (CheckBox)ConnectList0.Items[i];
                    if (item.Content.ToString() == newNick)
                        return;
                }
                item = new CheckBox();
                item.Content = newNick;
                item.IsChecked = true;
                byte[] RGB = new byte[3]; 
                Random r = new Random();
                r.NextBytes(RGB);
                for(int i=0; i<3; i++)
                    RGB[i] = (byte)(RGB[i] % 128);
                item.Foreground = new SolidColorBrush(Color.FromRgb(RGB[0], RGB[1], RGB[2]));
                if (newNick == Core.UserName)
                    item.IsEnabled = false;
                ConnectList0.Items.Add(item);
                CheckBox ToFor;
                for (int i = 1; i < Tabs.Items.Count - 1; i++)
                {
                    ToFor = new CheckBox();
                    ToFor.Content = item.Content;
                    ToFor.IsChecked = false;
                    ToFor.Foreground = item.Foreground;
                    ((ListBox)((Grid)((TabItem)Tabs.Items[i]).Content).Children[0]).Items.Add(ToFor);
                }
            });
        }

        public void ChangeConnectList(string oldNick, string newNick)
        {
            try
            {
                Dispatcher.Invoke((ThreadStart)delegate
                {
                    CheckBox item;
                    for (int i = 0; i < Tabs.Items.Count - 1; i++)
                    {
                        for (int j = 0; j < ((ListBox)((Grid)((TabItem)Tabs.Items[i]).Content).Children[0]).Items.Count; j++)
                        {
                            item = (CheckBox)((ListBox)((Grid)((TabItem)Tabs.Items[i]).Content).Children[0]).Items[j];
                            if (item.Content.ToString() == oldNick)
                            {
                                ((ListBox)((Grid)((TabItem)Tabs.Items[i]).Content).Children[0]).Items.Remove(item);
                                item.Content = newNick;
                                ((ListBox)((Grid)((TabItem)Tabs.Items[i]).Content).Children[0]).Items.Insert(j, item);
                                break;
                            }
                        }
                    }
                });
            }catch(ArgumentOutOfRangeException e)
            {

            }            
        }

        public void ReplaceConnectList(string oldNick)
        {
            Dispatcher.Invoke((ThreadStart)delegate
            {
                CheckBox item;
                for (int i = 0; i < Tabs.Items.Count - 1; i++)
                {
                    for (int j = 0; j < ((ListBox)((Grid)((TabItem)Tabs.Items[i]).Content).Children[0]).Items.Count; j++)
                    {
                        item = (CheckBox)((ListBox)((Grid)((TabItem)Tabs.Items[i]).Content).Children[0]).Items[j];
                        if (item.Content.ToString() == oldNick)
                        {
                            ((ListBox)((Grid)((TabItem)Tabs.Items[i]).Content).Children[0]).Items.Remove(item);
                            ConnectList0.Items.Remove(item);
                            break;
                        }
                    }
                }
            });
        }

        private void NickBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NicknameChanging();
                MessageBox.Focus();
            }
        }

        private List<String> GetReceiversList()
        {
            List<String> receiversList = new List<String>();
            Dispatcher.Invoke((ThreadStart)delegate
            {
                foreach (CheckBox a in ((ListBox)((Grid)((TabItem)Tabs.Items[Tabs.SelectedIndex]).Content).Children[0]).Items)
                {
                    if (a.IsChecked == true)
                    {
                        receiversList.Add(a.Content.ToString());
                    }
                }   
            });
            return receiversList;
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Tabs.SelectedIndex == Tabs.Items.Count - 1)
            {
                CreateTab(null);
                Tabs.SelectedItem = (TabItem)Tabs.Items[Tabs.Items.Count - 2];
            }
        }

        private void CreateTab(Message message)
        {
            TabItem tabItem = new TabItem();
            tabItem.Background = ((TabItem)Tabs.Items[0]).Background;
            tabItem.Foreground = ((TabItem)Tabs.Items[0]).Foreground;
            Grid grid = new Grid();
            grid.Margin = new Thickness(0, -1, 0, 26);
            RichTextBox rtb = new RichTextBox();
            rtb.Name = "ChatArea" + (Tabs.Items.Count - 1).ToString();
            rtb.HorizontalAlignment = HorizontalAlignment.Left;
            rtb.Margin = new Thickness(0, 0, 0, -24);
            rtb.Width = 417;
            rtb.AllowDrop = false;
            rtb.IsReadOnly = true;
            rtb.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            rtb.AcceptsReturn = false;
            rtb.IsUndoEnabled = false;
            rtb.Resources = ChatArea0.Resources;

            ListBox lb = new ListBox();
            lb.Name = "ConnectList" + (Tabs.Items.Count - 1).ToString();
            lb.HorizontalAlignment = HorizontalAlignment.Left;
            lb.Margin = new Thickness(424, 0, 0, -24);
            lb.Width = 204;
            CheckBox cb;
            for (int i = 0; i < ConnectList0.Items.Count; i++)
            {
                cb = new CheckBox();
                cb.Content = ((CheckBox)ConnectList0.Items[i]).Content;
                cb.Foreground = ((CheckBox)ConnectList0.Items[i]).Foreground;
                if (message != null && message.Receivers.Contains(cb.Content.ToString()))
                    cb.IsChecked = true;
                if (i == 0)
                {
                    cb.IsChecked = true;
                    cb.IsEnabled = false;
                }
                lb.Items.Add(cb);
            }
            grid.Children.Add(lb);
            grid.Children.Add(rtb);
            tabItem.Content = grid;
            if(message == null)
                tabItem.Tag = System.DateTime.Now.ToString("ddHHmmss");
            else
                tabItem.Tag = message.Text.Remove(8);
            tabItem.Header = "Conference #" + tabItem.Tag;
            Tabs.Items.Insert(Tabs.Items.Count - 1, tabItem);
//            Tabs.SelectedIndex = Tabs.Items.Count - 2;
        }

    }
    
}
